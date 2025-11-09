using System;
using System.IO;

public static class Constants
{

    public const string CONFIG_FILE_PATH           = "inputs/config.json";
    public const string CONTRACTS_FILE_PATH        = "inputs/contracts.json";
    public const string NOTIFICATION_LOG_FILE_PATH = "inputs/notification_log.json";

    public static string ResolvePath(string relativePath) =>
        Path.Combine(AppContext.BaseDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));


    public const string DATE_FORMAT = "yyyy-MM-dd";
    
    public const string KEY_ID                     = "id";
    public const string KEY_REASON                 = "reason";
    public const string KEY_NOTIFIED_ON            = "notified_on";
    public const string KEY_PREVIOUS_NOTIFICATION  = "previous_notification";
    public const string KEY_DAYS_TO_EXPIRY         = "days_to_expiry";
    public const string KEY_RENEWAL_DATE           = "renewal_date";
    public const string KEY_SOFTWARE_NAME          = "software_name";
    public const string KEY_OWNER                  = "owner";
    public const string KEY_ORGANIZATION           = "organization";
    public const string KEY_ANNUAL_COST            = "annual_cost_eur";
    public const string KEY_MIN_ANNUAL_COST        = "min_annual_cost";
    public const string KEY_RULES                  = "rules";
    public const string KEY_PRIORITY               = "priority";

    public const string NO_MATCH         = "No Match";
    public const string ALREADY_NOTIFIED = "Already Notified";
}