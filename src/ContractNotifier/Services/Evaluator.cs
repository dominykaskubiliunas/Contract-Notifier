/// <summary>
/// Decide whether a Contract should trigger a notification by combining decision rules and the notification log.
/// Calls NotificationCheck to populate any previous reason on the Contract and compares priority to determine
/// if a new notification is required.
/// </summary>
public static class Evaluator
{
    public static bool NotificationCheck(
        Dictionary<string, NotificationEntry> notificationLog,
        Contract contract)
    {
        if (notificationLog == null || notificationLog.Count == 0)
            return false;

        var id = contract.GetId();
        if (!notificationLog.ContainsKey(id))
            return false;

        if (notificationLog[id] != null)
        {
            var prevReason = notificationLog[id].reason;
            contract.SetPreviousNotification(prevReason);
        }
        return true;
    }

    public static (bool shouldNotify, string Reason) EvaluateContract(
        Contract contract,
        Config decisionRules,
        Dictionary<string, NotificationEntry> notificationLog)
    {
        var topReason = decisionRules.FindTopReason(contract);

        if (topReason == Constants.NO_MATCH)
            return (false, Constants.NO_MATCH);

        var wasNotified = NotificationCheck(notificationLog, contract);

        if (!wasNotified)
            return (true, topReason);

        var prev = contract.GetPreviousNotification();

        if (string.IsNullOrEmpty(prev))
            return (true, topReason);

        int prevIdx = decisionRules.Priority.IndexOf(prev);
        int newIdx = decisionRules.Priority.IndexOf(topReason);

        if (prevIdx < 0) prevIdx = int.MaxValue;
        if (newIdx < 0) newIdx = int.MaxValue;

        if (prevIdx > newIdx)
            return (true, topReason);
        else
            return (false, Constants.ALREADY_NOTIFIED);
    }
}