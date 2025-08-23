namespace DeviceMonitoringAPI.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Хранит данные о сессии
/// </summary>
public class DeviceActivity
{
    /// <summary>
    /// ID устройства
    /// </summary>
    [JsonPropertyName("_id")]
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Время включения
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Время выключения
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Версия установленного приложения
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Уникальное ID сессии
    /// </summary>
    public Guid ActivityId { get; set; } = Guid.NewGuid();
}

