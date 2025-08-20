namespace DeviceMonitoringAPI.Models;

public class DeviceActivity
{
    public Guid Id { get; set; }    // Guid - Globally Unique Identifier
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Version { get; set; } = string.Empty;
}

