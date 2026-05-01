namespace Monitor_Link_Rede.Models;

public sealed class NetworkEventRecord
{
    public DateTime Timestamp { get; set; }
    public string EventType { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
    public string Description { get; set; } = string.Empty;
    public TimeSpan? OfflineDuration { get; set; }
}
