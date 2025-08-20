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

        // Если входящее activity не имеет Id, создаём новую activity.
        if (activity.Id == Guid.Empty)
        {
            activity.Id = Guid.NewGuid();
        }

        // Пытаемся добавить activity в словарь
        // Это так же может обновить существующуе значение, если Id уже существует
        DataContext.DeviceActivities[activity.Id] = activity;

        _logger.LogInformation("Received and stored activity for device: {DeviceId}", activity.Id);

        return Ok(activity);    // возвращаем HTTP 200 OK и объект activity c Id
    }

    // GET endpoint: api/DeviceActivity
    // Этот endpoint возвращает все DeviceActivities без Id
    [HttpGet]
    public ActionResult<IEnumerable<DeviceActivity>> Get()
    {
        return Ok(DataContext.DeviceActivities.Values);
    }
}