namespace DeviceMonitoringAPI.Models;

using System.Text.Json.Serialization;

public class DeviceActivity
{
    [JsonPropertyName("_id")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    public Guid ActivityId { get; set; } = Guid.NewGuid();
}

