namespace Monitor_Link_Rede.Models;

public sealed class NetworkMonitorState
{
    public bool IsMonitoring { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastCheckTime { get; set; }
    public string LastMessage { get; set; } = "Monitoramento parado";
}
