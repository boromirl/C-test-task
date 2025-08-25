using System.Collections.Concurrent;
using System.Text.Json;
using DeviceMonitoringAPI.Data;
using DeviceMonitoringAPI.Models;

namespace DeviceMonitoringAPI.Services;

// Хелпер класс для десериализации данных
public class BackupData
{
    public DateTime Timestamp { get; set; }
    public ConcurrentDictionary<string, List<DeviceActivity>> DeviceActivities { get; set; } = new();
}


public interface IBackupService
{
    Task<string> CreateBackup();
    Task<string> RestoreFromLatestBackup();
}

public class BackupService : IBackupService
{
    private readonly ILogger<BackupService> _logger;

    public BackupService(ILogger<BackupService> logger)
    {
        _logger = logger;
    }

    public async Task<string> CreateBackup()
    {
        try
        {
            // Создаем папку для бэкапа
            var backupDir = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            // Создаем файл бекапа с timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFile = Path.Combine(backupDir, $"backup_{timestamp}.json");

            // Сериализация данных в JSON
            var dataToBackup = new
            {
                Timestamp = DateTime.Now,
                DeviceActivities = DataContext.DeviceActivities
            };

            var jsonData = JsonSerializer.Serialize(dataToBackup, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Запись в файл
            await File.WriteAllTextAsync(backupFile, jsonData);

            _logger.LogInformation("Backup created: {BackupFile}", backupFile);
            return backupFile;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating backup");
            throw;
        }
    }

    public async Task<string> RestoreFromLatestBackup()
    {
        try
        {
            var backupDir = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
            if (!Directory.Exists(backupDir))
            {
                throw new FileNotFoundException("Backup directory does not exist");
            }

            // Получаем самый новый бэкап файл
            var backupFile = Directory.GetFiles(backupDir, "backup_*.json")
                .OrderByDescending(f => f)
                .FirstOrDefault();

            if (backupFile == null)
            {
                throw new FileNotFoundException("No backup files found");
            }

            // Читаем и десериализуем бэкап
            var jsonData = await File.ReadAllTextAsync(backupFile);
            var backupData = JsonSerializer.Deserialize<BackupData>(jsonData);

            if (backupData?.DeviceActivities == null)
            {
                throw new InvalidOperationException("Backup file is invalid or empty");
            }

            // Заменяем текущие данные на бэкап
            DataContext.DeviceActivities.Clear();
            foreach (var item in backupData.DeviceActivities)
            {
                DataContext.DeviceActivities[item.Key] = item.Value;
            }

            _logger.LogInformation("Data restored from backup: {BackupFile}", backupFile);
            return backupFile;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error restoring from backup");
            throw;
        }
    }
}