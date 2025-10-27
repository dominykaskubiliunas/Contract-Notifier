using System.Collections.Generic;

public static class Serializer
{

    public static string NotificationOutput(Contract contract, string reason)
    {
        return $"Software: {contract.GetSoftwareName()}, " +
           $"Organization: {contract.GetOrganization()}, " +
           $"Annual Cost: {contract.GetAnnualCost()}, " +
           $"Renewal Date: {contract.GetRenewalDate()}, " +
           $"Reason: {reason}";
    }

    public static Dictionary<string, string> NotificationLogEntry(string notifiedOn, string reason)
    {
        return new Dictionary<string, string>
        {
            [Constants.KEY_NOTIFIED_ON] = notifiedOn,
            [Constants.KEY_REASON] = reason
        };
    }
}