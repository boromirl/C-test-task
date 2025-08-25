using DeviceMonitoringAPI.Data;
using DeviceMonitoringAPI.Models;
using Microsoft.AspNetCore.Mvc;
using DeviceMonitoringAPI.Services;

namespace DeviceMonitoringAPI.Controllers;

[ApiController]
[Route("api/[controller]")] // Путь будет /api/DeviceActivity
public class DeviceActivityController : ControllerBase
{
    private readonly ILogger<DeviceActivityController> _logger;
    private readonly IBackupService _backupService;

    public DeviceActivityController(ILogger<DeviceActivityController> logger, IBackupService backupService)
    {
        _logger = logger;
        _backupService = backupService;
    }

    /// <summary>
    /// POST реквест на добавление новой сессии
    /// </summary>
    /// <param name="activity">Данные о сессии</param>
    /// <returns>Bad Request при неудачном добавлении, OK + добавленные данные при удачном добавлении</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public IActionResult Post([FromBody] DeviceActivity activity)
    {
        // Валидация реквеста (базовая)
        // Если тело реквеста пустое или десереализация не удается
        if (activity == null)
        {
            _logger.LogWarning("Received null activity data");
            return BadRequest("Activity data is null.");
        }

        if (string.IsNullOrEmpty(activity.DeviceId))
        {
            _logger.LogWarning("Received activity without device ID");
            return BadRequest("Device ID is required.");
        }

        var deviceId = activity.DeviceId;
        // Если пока не записей об этом устройстве, создаем список под новое устройство
        if (!DataContext.DeviceActivities.ContainsKey(deviceId))
        {
            DataContext.DeviceActivities[deviceId] = [];
        }

        // Собственно, добавление новой записи в словарь
        DataContext.DeviceActivities[deviceId].Add(activity);

        _logger.LogInformation("Received activity for device: {DeviceId}", deviceId);

        return Ok(activity);    // возвращаем HTTP 200 OK и объект activity c Id
    }

    /// <summary>
    /// Получение списка устройств
    /// </summary>
    /// <returns>Список устройств</returns> 
    [HttpGet("devices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DeviceActivity>> GetDevices()
    {
        // kvp - key value pair
        var devices = DataContext.DeviceActivities.Select(kvp => new
        {
            deviceId = kvp.Key
        }).ToList();

        return Ok(devices);
    }

    /// <summary>
    /// Получение списка сессий по ID устройства
    /// </summary>
    /// <param name="deviceId">ID устройства</param>
    /// <returns>Список сессий устройства OK 200, если устройство существует. Not found, если устройства не существует</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    [HttpGet("{deviceId}")]
    public ActionResult<IEnumerable<DeviceActivity>> GetActivitiesByDeviceId(string deviceId)
    {
        // 
        if (DataContext.DeviceActivities.TryGetValue(deviceId, out var activities))
        {
            return Ok(activities);
        }

        // если нет, возвращаем страницу 404
        _logger.LogWarning("Device with received deviceId not found.");
        return NotFound();
    }

    /// <summary>
    /// Удаляет выбранную сессию
    /// </summary>
    /// <param name="activityId">ID удаляемой сессии</param>
    /// <returns>No content при удачном удалении, not found, если сессии с таким id нет</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)] // 204 успешное удаление
    [ProducesResponseType(StatusCodes.Status404NotFound)]  // 404 запись не найдена
    [HttpDelete("activity/{activityId:guid}")] // :guid constraints гарантирует, что URL парамент имеет валидный Guid
    public IActionResult DeleteActivity(Guid activityId)
    {
        // Поиск activity среди всех записей об устройствах
        // На данный момент реализовано самым простым способом
        foreach (var device in DataContext.DeviceActivities)
        {
            var activityToDelete = device.Value.FirstOrDefault(a => a.ActivityId == activityId);
            if (activityToDelete != null)
            {
                // Собственно, удаление activity
                device.Value.Remove(activityToDelete);
                _logger.LogInformation("Deleted activity {ActivityId} for device {DeviceId}", activityId, device.Key);

                // Если было удалено последнее activity у устройства, удаляем устройство из словаря
                if (device.Value.Count == 0)
                {
                    DataContext.DeviceActivities.TryRemove(device.Key, out _);
                    _logger.LogInformation("Removed empty device entry: {DeviceId}", device.Key);
                }

                return NoContent(); // HTTP 204 - успешно, но не нужно возвращать ничего
            }
        }
        // Если activity не найдено, возвращаем 404
        _logger.LogWarning($"DeleteActivity unsuccessfull. Activity with ID {activityId} not found.");
        return NotFound($"Activity with ID {activityId} not found.");
    }

    /// <summary>
    /// Создание бэкапа данных в файле на сервере
    /// </summary>
    /// <returns>Ok при удачном создании, 500 при неудачном.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("backup")]
    public async Task<IActionResult> CreateBackup()
    {
        try
        {
            var backupPath = await _backupService.CreateBackup();
            return Ok(new
            {
                Message = "Backup created successfully",
                FilePath = backupPath,
                Timestamp = DateTime.Now
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating backup");
            return StatusCode(500, "Failed to create backup");
        }
    }

    /// <summary>
    /// Восстановление из ближайшего бэкапа
    /// </summary>
    /// <returns>Ok при удачном восстановлении, NotFound если нет бэкап файлов, 500 при ошибке восстановления</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("restore")]
    public async Task<IActionResult> RestoreFromBackup()
    {
        try
        {
            var backupPath = await _backupService.RestoreFromLatestBackup();
            return Ok(new
            {
                Message = "Data restored successfully",
                FilePath = backupPath,
                Timestamp = DateTime.Now
            });
        }
        catch (FileNotFoundException e)
        {
            _logger.LogWarning(e, "Backup file not found");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error restoring from backup");
            return StatusCode(500, "Failed to restore from backup");
        }
    }
}