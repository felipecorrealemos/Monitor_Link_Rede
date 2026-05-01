using System.Text.Json;
using Monitor_Link_Rede.Models;

namespace Monitor_Link_Rede.Services;

public sealed class LogStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly string _filePath;

    public LogStorageService()
    {
        _filePath = Path.Combine(FileSystem.Current.AppDataDirectory, "network-events.json");
    }

    public async Task<IReadOnlyList<NetworkEventRecord>> GetAllAsync()
    {
        await _gate.WaitAsync();
        try
        {
            await EnsureFileExistsAsync();
            await using var stream = File.OpenRead(_filePath);
            var data = await JsonSerializer.DeserializeAsync<List<NetworkEventRecord>>(stream, JsonOptions);
            return (data ?? []).OrderByDescending(x => x.Timestamp).ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task AddAsync(NetworkEventRecord record)
    {
        await _gate.WaitAsync();
        try
        {
            await EnsureFileExistsAsync();
            var data = await ReadInternalAsync();
            data.Add(record);
            await WriteAtomicAsync(data);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task EnsureFileExistsAsync()
    {
        var folder = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        if (!File.Exists(_filePath))
        {
            await File.WriteAllTextAsync(_filePath, "[]");
        }
    }

    private async Task<List<NetworkEventRecord>> ReadInternalAsync()
    {
        await using var stream = File.OpenRead(_filePath);
        var data = await JsonSerializer.DeserializeAsync<List<NetworkEventRecord>>(stream, JsonOptions);
        return data ?? [];
    }

    private async Task WriteAtomicAsync(List<NetworkEventRecord> records)
    {
        var tempFile = _filePath + ".tmp";
        await using (var stream = File.Create(tempFile))
        {
            await JsonSerializer.SerializeAsync(stream, records, JsonOptions);
        }

        File.Move(tempFile, _filePath, true);
    }
}
