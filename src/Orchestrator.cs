using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Orchestrator
{
    private readonly Dictionary<string, object> _config;
    private readonly List<Dictionary<string, object>> _contracts;

    private string _currentDatetime = "";
    private DecisionRules _decisionRules;
    private Dictionary<string, Dictionary<string, string>> _notifications =
        new Dictionary<string, Dictionary<string, string>>();
    private readonly List<string> _currentNotifications = new List<string>();

    public Orchestrator(string configInputPath, string contractsInputPath)
    {
        var json = File.ReadAllText(configInputPath);
        _config = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
            ?? new Dictionary<string, object>();
                
        var contractsJson = File.ReadAllText(contractsInputPath);
        _contracts = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(contractsJson)
                ?? new List<Dictionary<string, object>>();
            
        var rules = GetFromConfig<List<Dictionary<string, object>>>(_config, Constants.KEY_RULES)
                    ?? new List<Dictionary<string, object>>();
        var priority = GetFromConfig<List<string>>(_config, Constants.KEY_PRIORITY)
                       ?? new List<string>();

        _decisionRules = new DecisionRules(rules, priority);
    }

    public void AddCurrentDatetimeStr(string currentDatetimeStr)
    {
        _currentDatetime = currentDatetimeStr;
    }

    public void LoadNotifications(string notificationFilePath = Constants.NOTIFICATION_LOG_FILE_PATH)
    {
        var json_notifications = File.ReadAllText(notificationFilePath);
        _notifications = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json_notifications)
                    ?? new Dictionary<string, Dictionary<string, string>>();
    }

    public void ClearCurrentNotifications()
    {
        _currentNotifications.Clear();
    }

    public void CheckContracts()
    {
        foreach (var contractData in _contracts)
        {
            var currentContract = new Contract(contractData);
            currentContract.ComputeDaysToExpiry(_currentDatetime);

            var (isNotified, reason) = Evaluator.EvaluateContract(
                currentContract, _decisionRules, _notifications);

            if (isNotified)
            {
                _notifications[currentContract.GetId()] =
                    Serializer.NotificationLogEntry(_currentDatetime, reason);

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