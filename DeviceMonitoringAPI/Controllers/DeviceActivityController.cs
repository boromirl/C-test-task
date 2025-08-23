using DeviceMonitoringAPI.Data;
using DeviceMonitoringAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceMonitoringAPI.Controllers;

[ApiController]
[Route("api/[controller]")] // Путь будет /api/DeviceActivity
public class DeviceActivityController : ControllerBase
{
    private readonly ILogger<DeviceActivityController> _logger;

    public DeviceActivityController(ILogger<DeviceActivityController> logger)
    {
        _logger = logger;
    }

    // POST endpoint: api/DeviceActivity
    // Этот endpoint получает JSON объект и добавляем в наше in-memory хранилище
    // [FromBody] атрибут говорит фреймворку десереализировать JSON объект из тела
    // реквеста в DeviceActivity объект.
    [HttpPost]
    public IActionResult Post([FromBody] DeviceActivity activity)
    {
        // Валидация реквеста (базовая)
        // Если тело реквеста пустое или десереализация не удается
        if (activity == null)
        {
            return BadRequest("Activity data is null.");
        }

        if (string.IsNullOrEmpty(activity.DeviceId))
        {
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

    // 
    // 
    [HttpGet("devices")]
    public ActionResult<IEnumerable<DeviceActivity>> GetDevices()
    {
        // kvp - key value pair
        var devices = DataContext.DeviceActivities.Select(kvp => new
        {
            deviceId = kvp.Key
        }).ToList();

        return Ok(devices);
    }

    [HttpGet("{deviceId}")]
    public ActionResult<IEnumerable<DeviceActivity>> GetActivitiesByDeviceId(string deviceId)
    {
        // 
        if (DataContext.DeviceActivities.TryGetValue(deviceId, out var activities))
        {
            return Ok(activities);
        }

        // если нет, возвращаем страницу 404
        return NotFound();
    }

    [HttpDelete("activity/{activityId:guid}")] // :guid constraints гарантирует, что URL парамент имеет валидный Guid
    public IActionResult DeleteActivity(Guid activityId)
    {
        // Поиск activity среди всех записей об устройствах
        // На данный момент реализовано самым простым способом
        foreach(var device in DataContext.DeviceActivities){
            var activityToDelete = device.Value.FirstOrDefault(a => a.ActivityId == activityId);
            if (activityToDelete != null)
            {
                // Собственно, удаление activity
                device.Value.Remove(activityToDelete);
                _logger.LogInformation("Deleted activity {ActivityId} for device {DeviceId}", activityId, device.Key);

                // Если было удалено последнее activity у устройства, удаляем устройство из словаря
                if(device.Value.Count == 0)
                {
                    DataContext.DeviceActivities.TryRemove(device.Key, out _);
                    _logger.LogInformation("Removed empty device entry: {DeviceId}", device.Key);
                }

                return NoContent(); // HTTP 204 - успешно, но не нужно возвращать ничего
            }
        }
        // Если activity не найдено, возвращаем 404
        return NotFound($"Activity with ID {activityId} not found.");
    }
}