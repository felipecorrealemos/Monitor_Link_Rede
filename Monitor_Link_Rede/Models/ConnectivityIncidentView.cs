namespace Monitor_Link_Rede.Models;

public sealed class ConnectivityIncidentView
{
    public DateTime DropTime { get; set; }
    public DateTime? ReturnTime { get; set; }
    public TimeSpan? OfflineDuration { get; set; }
}
