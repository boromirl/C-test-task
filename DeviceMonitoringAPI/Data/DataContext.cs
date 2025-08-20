/*
    In-memory data storage.
    Thread-safety with CouncurrentDictionary
*/

using System.Collections.Concurrent;
using DeviceMonitoringAPI.Models;

namespace DeviceMonitoringAPI.Data;

public static class DataContext
{
    // In-memory словарь, который хранит DeviceActivities, используя Id в 
    // качестве ключа
    public static readonly ConcurrentDictionary<Guid, DeviceActivity> DeviceActivities = new();
}