using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Orchestrator
{
    private readonly Dictionary<string, object> _config;
    private readonly List<Contract> _contracts;

    private string _currentDatetime = "";
    private Config _decisionRules;
    private Dictionary<string, NotificationEntry> _notifications =
        new Dictionary<string, NotificationEntry>();
    private readonly List<string> _currentNotifications = new List<string>();

    public Orchestrator(string configInputPath, string contractsInputPath)
    {
        var json = File.ReadAllText(configInputPath);
        _config = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
            ?? new Dictionary<string, object>();
                
        var contractsJson = File.ReadAllText(contractsInputPath);        
        _contracts = JsonSerializer.Deserialize<List<Contract>>(contractsJson) ?? new List<Contract>();

        var rules = GetFromConfig<List<Dictionary<string, object>>>(_config, Constants.KEY_RULES)
                    ?? new List<Dictionary<string, object>>();
        var priority = GetFromConfig<List<string>>(_config, Constants.KEY_PRIORITY)
                       ?? new List<string>();

        _decisionRules = new Config(rules, priority);
    }

    public void AddCurrentDatetimeStr(string currentDatetimeStr)
    {
        _currentDatetime = currentDatetimeStr;
    }

    public void LoadNotifications(string notificationFilePath = Constants.NOTIFICATION_LOG_FILE_PATH)
    {
        var jsonNotifications = File.ReadAllText(notificationFilePath);
        _notifications = JsonSerializer.Deserialize<Dictionary<string, NotificationEntry>>(jsonNotifications) ?? new Dictionary<string, NotificationEntry>();
    }

    public void ClearCurrentNotifications()
    {
        _currentNotifications.Clear();
    }

    public void CheckContracts()
    {
        if (_contracts == null || _contracts.Count == 0)
        {
            Console.WriteLine("No contracts found");
            return;
        }
        foreach (Contract currentContract in _contracts)
        {
            currentContract.ComputeDaysToExpiry(_currentDatetime);

            var (isNotified, reason) = Evaluator.EvaluateContract(
                currentContract, _decisionRules, _notifications);

            if (isNotified)
            {
                _notifications[currentContract.GetId()] = new NotificationEntry { notified_on = _currentDatetime, reason = reason};

                var output = Serializer.NotificationOutput(currentContract, reason).ToString();
                _currentNotifications.Add(output);
            }
        }

        foreach (var entry in _currentNotifications)
        {
            Console.WriteLine(entry);
        }
        SaveJson(_notifications, Constants.NOTIFICATION_LOG_FILE_PATH);
    }

    // ---------- helpers ----------

    private static void SaveJson<T>(T data, string path)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    private static TOut? GetFromConfig<TOut>(Dictionary<string, object> cfg, string key)
    {
        if (!cfg.TryGetValue(key, out var obj) || obj is null) return default;
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<TOut>(json);
    }
}