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

        if (string.IsNullOrEmpty(activity.Id))
        {
            return BadRequest("Device ID is required.");
        }

        var deviceId = activity.Id;
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
            DeviceId = kvp.Key
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
}