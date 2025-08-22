/*
    In-memory data storage.
    Thread-safety with CouncurrentDictionary
*/

using System.Collections.Concurrent;
using DeviceMonitoringAPI.Models;

namespace DeviceMonitoringAPI.Data;

public static class DataContext
{
    // In-memory словарь, который хранит DeviceActivities
    public static readonly ConcurrentDictionary<string, List<DeviceActivity>> DeviceActivities = new();
}