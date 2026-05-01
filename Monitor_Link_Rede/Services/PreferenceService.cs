using Monitor_Link_Rede.Models;

namespace Monitor_Link_Rede.Services;

public sealed class PreferenceService : IPreferenceService
{
    private const string CheckIntervalKey = "check_interval_seconds";
    private const string ChartFilterKey = "selected_chart_period_filter";
    private const string HistoryFilterKey = "selected_history_period_filter";

    public int GetCheckIntervalSeconds() => Math.Clamp(Preferences.Default.Get(CheckIntervalKey, 10), 3, 300);

    public void SetCheckIntervalSeconds(int seconds) => Preferences.Default.Set(CheckIntervalKey, Math.Clamp(seconds, 3, 300));

    public PeriodFilter GetChartFilter() => GetFilter(ChartFilterKey);

    public void SetChartFilter(PeriodFilter filter) => Preferences.Default.Set(ChartFilterKey, filter.ToString());

    public PeriodFilter GetHistoryFilter() => GetFilter(HistoryFilterKey);

    public void SetHistoryFilter(PeriodFilter filter) => Preferences.Default.Set(HistoryFilterKey, filter.ToString());

    private static PeriodFilter GetFilter(string key)
    {
        var raw = Preferences.Default.Get(key, PeriodFilter.Last7Days.ToString());
        return Enum.TryParse<PeriodFilter>(raw, true, out var filter) ? filter : PeriodFilter.Last7Days;
    }
}
