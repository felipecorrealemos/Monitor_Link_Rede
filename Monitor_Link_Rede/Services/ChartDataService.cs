using System.Globalization;
using Monitor_Link_Rede.Models;

namespace Monitor_Link_Rede.Services;

public sealed class ChartDataService
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    private readonly LogStorageService _storageService;

    public ChartDataService(LogStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<ChartDatasetResult> BuildDatasetAsync(PeriodFilter filter)
    {
        var data = await _storageService.GetAllAsync();
        var filtered = ApplyFilter(data, filter).ToList();

        var grouped = filtered
            .GroupBy(x => BuildGroupKey(x.Timestamp, filter))
            .OrderBy(x => x.Min(r => r.Timestamp))
            .ToList();

        return new ChartDatasetResult
        {
            PeriodKeys = grouped.Select(g => g.Key).ToArray(),
            Labels = grouped.Select(g => BuildGroupLabel(g.First().Timestamp, filter)).ToArray(),
            DropByPeriod = grouped.Select(g => g.Count(r => r.EventType == "QUEDA")).ToArray(),
            OfflineCountByPeriod = grouped.Select(g => g.Count(r => !r.IsOnline)).ToArray(),
            OfflineMinutesByPeriod = grouped.Select(g => g.Where(r => r.OfflineDuration.HasValue).Sum(r => r.OfflineDuration!.Value.TotalMinutes)).ToArray()
        };
    }

    private static IEnumerable<NetworkEventRecord> ApplyFilter(IReadOnlyList<NetworkEventRecord> data, PeriodFilter filter)
    {
        var now = DateTime.Now;

        return filter switch
        {
            PeriodFilter.Today => data.Where(x => x.Timestamp.Date == now.Date),
            PeriodFilter.Last7Days => data.Where(x => x.Timestamp >= now.AddDays(-7)),
            PeriodFilter.Last30Days => data.Where(x => x.Timestamp >= now.AddDays(-30)),
            _ => data
        };
    }

    private static string BuildGroupLabel(DateTime date, PeriodFilter filter)
    {
        return filter switch
        {
            PeriodFilter.Today => date.ToString("ddd HH:mm", PtBr),
            PeriodFilter.Last7Days => date.ToString("ddd dd/MM", PtBr),
            PeriodFilter.Last30Days => date.ToString("ddd dd/MM", PtBr),
            _ => date.ToString("yyyy-MM")
        };
    }

    private static string BuildGroupKey(DateTime date, PeriodFilter filter)
    {
        return filter switch
        {
            PeriodFilter.Today => date.ToString("yyyy-MM-ddTHH:mm"),
            PeriodFilter.Last7Days => date.ToString("yyyy-MM-dd"),
            PeriodFilter.Last30Days => date.ToString("yyyy-MM-dd"),
            _ => date.ToString("yyyy-MM")
        };
    }
}
