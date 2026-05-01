using Monitor_Link_Rede.Models;

namespace Monitor_Link_Rede.Services;

public interface IPreferenceService
{
    int GetCheckIntervalSeconds();
    void SetCheckIntervalSeconds(int seconds);
    PeriodFilter GetChartFilter();
    void SetChartFilter(PeriodFilter filter);
    PeriodFilter GetHistoryFilter();
    void SetHistoryFilter(PeriodFilter filter);
}
