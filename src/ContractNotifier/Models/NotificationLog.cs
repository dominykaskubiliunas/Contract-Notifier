/// <summary>
/// Model for a single notification log entry as loaded from notifications.json.
/// Properties map to the JSON fields (notified_on, reason). Instances represent
/// when a notification was sent and the reason for it and are used by the
/// notification lookup when evaluating whether a contract should be re-notified.
/// </summary>
public class NotificationEntry
{
    public string notified_on { get; set; } = "";
    public string reason { get; set; } = "";
}