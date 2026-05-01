using System.Net;
using Monitor_Link_Rede.Models;

namespace Monitor_Link_Rede.Services;

public sealed class ConnectivityMonitorService
{
    private readonly HttpClient _httpClient;
    private readonly IPreferenceService _preferenceService;
    private readonly LogStorageService _storage;
    private readonly SemaphoreSlim _runGate = new(1, 1);

    private CancellationTokenSource? _cts;
    private Task? _worker;
    private DateTime? _offlineStart;
    private bool? _lastState;

    public NetworkMonitorState State { get; } = new();

    public event Func<Task>? StateChanged;

    public ConnectivityMonitorService(HttpClient httpClient, IPreferenceService preferenceService, LogStorageService storage)
    {
        _httpClient = httpClient;
        _preferenceService = preferenceService;
        _storage = storage;
    }

    public async Task StartAsync()
    {
        await _runGate.WaitAsync();
        try
        {
            if (State.IsMonitoring)
            {
                return;
            }

            await RestorePendingDropContextAsync();

            _cts = new CancellationTokenSource();
            State.IsMonitoring = true;
            State.LastMessage = "Monitoramento ativo";
            await NotifyAsync();

            _worker = RunLoopAsync(_cts.Token);
        }
        finally
        {
            _runGate.Release();
        }
    }

    public async Task StopAsync()
    {
        await _runGate.WaitAsync();
        try
        {
            if (!State.IsMonitoring)
            {
                return;
            }

            _cts?.Cancel();
            if (_worker is not null)
            {
                try
                {
                    await _worker;
                }
                catch (OperationCanceledException)
                {
                }
            }

            State.IsMonitoring = false;
            State.LastMessage = "Monitoramento parado";
            await NotifyAsync();
        }
        finally
        {
            _runGate.Release();
        }
    }

    private async Task RestorePendingDropContextAsync()
    {
        var all = await _storage.GetAllAsync();
        var lastEvent = all.OrderByDescending(x => x.Timestamp).FirstOrDefault();

        if (lastEvent is null)
        {
            _lastState = null;
            _offlineStart = null;
            return;
        }

        if (lastEvent.EventType == "QUEDA")
        {
            _lastState = false;
            _offlineStart = lastEvent.Timestamp;
            State.LastMessage = "Retomando monitoramento com queda pendente";
            return;
        }

        _lastState = true;
        _offlineStart = null;
    }

    private async Task RunLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var isOnline = await CheckInternetAccessAsync(token);
                State.LastCheckTime = DateTime.Now;
                State.IsOnline = isOnline;
                await HandleTransitionAsync(isOnline);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                State.LastMessage = $"Erro de verificacao: {ex.Message}";
            }

            await NotifyAsync();
            var delay = TimeSpan.FromSeconds(_preferenceService.GetCheckIntervalSeconds());
            await Task.Delay(delay, token);
        }
    }

    private async Task HandleTransitionAsync(bool isOnline)
    {
        if (_lastState is null)
        {
            _lastState = isOnline;
            State.LastMessage = isOnline ? "Conexao online" : "Conexao offline";
            if (!isOnline)
            {
                _offlineStart = DateTime.Now;
            }
            return;
        }

        if (_lastState == isOnline)
        {
            State.LastMessage = isOnline ? "Conexao online" : "Conexao offline";
            return;
        }

        _lastState = isOnline;
        var now = DateTime.Now;

        if (!isOnline)
        {
            _offlineStart = now;
            var drop = new NetworkEventRecord
            {
                Timestamp = now,
                EventType = "QUEDA",
                IsOnline = false,
                Description = "Falha na conexao com a internet"
            };

            await _storage.AddAsync(drop);
            State.LastMessage = "Queda registrada";
            return;
        }

        TimeSpan? offlineDuration = null;
        if (_offlineStart is not null)
        {
            offlineDuration = now - _offlineStart.Value;
        }

        var restored = new NetworkEventRecord
        {
            Timestamp = now,
            EventType = "RETORNO",
            IsOnline = true,
            Description = "Conexao restabelecida",
            OfflineDuration = offlineDuration
        };

        await _storage.AddAsync(restored);
        _offlineStart = null;
        State.LastMessage = "Retorno registrado";
    }

    private async Task<bool> CheckInternetAccessAsync(CancellationToken token)
    {
        if (Connectivity.Current.NetworkAccess != Microsoft.Maui.Networking.NetworkAccess.Internet)
        {
            return false;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://clients3.google.com/generate_204");
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(4));

        try
        {
            using var response = await _httpClient.SendAsync(request, timeoutCts.Token);
            return response.StatusCode == HttpStatusCode.NoContent || response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task NotifyAsync()
    {
        if (StateChanged is null)
        {
            return;
        }

        await StateChanged.Invoke();
    }
}

