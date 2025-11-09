/// <summary>
/// Model for a single contract record as loaded from contracts.json.
/// Properties map to the JSON fields (id, software_name, owner, organization,
/// annual_cost_eur, renewal_date, etc.). This class is intended to be
/// populated by deserializing contracts.json and provides helper methods to:
/// - compute days until renewal (ComputeDaysToExpiry),
/// - get commonly used fields (GetId, GetAnnualCost, GetDaysToExpiry, ...),
/// - and store the previous notification reason (SetPreviousNotification/GetPreviousNotification).
/// </summary>
public class Contract
{

    public int id { get; set; }
    public string software_name { get; set; } = "Unknown Software Name";
    public string owner { get; set; } = "Unknown Owner";
    public string organization { get; set; } = "Unknown Organization";
    public decimal annual_cost_eur { get; set; }
    public string renewal_date { get; set; } = string.Empty;
    public int days_to_expiry { get; set; }
    public string prevNotification { get; set; } = string.Empty;

    public int ComputeDaysToExpiry(string currentDate)
    {
        if (string.IsNullOrEmpty(renewal_date))
        {
            return int.MaxValue;
        }

        DateTime current = DateTime.ParseExact(currentDate, Constants.DATE_FORMAT, null);
        DateTime renewal = DateTime.ParseExact(renewal_date, Constants.DATE_FORMAT, null);
        days_to_expiry = (renewal - current).Days;
        return days_to_expiry;
    }

    public void SetPreviousNotification(string notification)
    {
        prevNotification = notification;
    }

    public string GetId()
    {
        return id.ToString();
    }

    public int GetAnnualCost()
    {
        return (int)annual_cost_eur;
    }

    public string GetRenewalDate()
    {
        return renewal_date;
    }

    public string GetSoftwareName()
    {
        return software_name;
    }

    public string GetOrganization()
    {
        return organization;
    }

    public int GetDaysToExpiry()
    {
        return days_to_expiry;
    }

    public string GetPreviousNotification()
    {
        return prevNotification;
    }
}
