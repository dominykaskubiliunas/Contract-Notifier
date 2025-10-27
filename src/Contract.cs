using System;
using System.Collections.Generic;
using System.Text.Json;

public class Contract
{
    private readonly Dictionary<string, object> data;

    public Contract(Dictionary<string, object> data)
    {
        this.data = data;
    }

    public int ComputeDaysToExpiry(string currentDate)
    {
        DateTime current = DateTime.ParseExact(currentDate, Constants.DATE_FORMAT, null);
        DateTime renewal = DateTime.ParseExact(data[Constants.KEY_RENEWAL_DATE].ToString(), Constants.DATE_FORMAT, null);
        int days = (renewal - current).Days;
        data[Constants.KEY_DAYS_TO_EXPIRY] = days;
        return days;
    }

    public void SetPreviousNotification(string notification)
    {
        data[Constants.KEY_PREVIOUS_NOTIFICATION] = notification;
    }

    public string GetId()
    {
        return ((JsonElement)data[Constants.KEY_ID]).GetInt32().ToString();
    }

    public int GetAnnualCost()
    {
        return (int)((JsonElement)data[Constants.KEY_ANNUAL_COST]).GetSingle();
    }

    public string GetRenewalDate()
    {
        return ((JsonElement)data[Constants.KEY_RENEWAL_DATE]).GetString();
    }

    public string GetSoftwareName()
    {
        return ((JsonElement)data[Constants.KEY_SOFTWARE_NAME]).GetString();
    }

    public string GetOrganization()
    {
        return ((JsonElement)data[Constants.KEY_ORGANIZATION]).GetString();
    }

    public int GetDaysToExpiry()
    {
        return data.ContainsKey(Constants.KEY_DAYS_TO_EXPIRY) ? Convert.ToInt32(data[Constants.KEY_DAYS_TO_EXPIRY]) : 0;
    }

    public string GetPreviousNotification()
    {
        return data.ContainsKey(Constants.KEY_PREVIOUS_NOTIFICATION)
            ? data[Constants.KEY_PREVIOUS_NOTIFICATION]?.ToString()
            : null;
    }
}
