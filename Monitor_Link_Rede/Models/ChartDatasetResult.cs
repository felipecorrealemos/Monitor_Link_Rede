namespace Monitor_Link_Rede.Models;

public sealed class ChartDatasetResult
{
    public string[] Labels { get; set; } = [];
    public string[] PeriodKeys { get; set; } = [];
    public int[] DropByPeriod { get; set; } = [];
    public double[] OfflineMinutesByPeriod { get; set; } = [];
    public int[] OfflineCountByPeriod { get; set; } = [];
}
