using System.Text.Json;

/// <summary>
/// Configuration and decision rules used to pick a notification reason for a Contract.
/// 
/// Detailed behavior:
/// - Data source: rules are expected to be deserialized from a JSON file into List<Dictionary<string, object>>.
///   Each rule dictionary typically contains JsonElement values (objects produced by System.Text.Json).
/// - Priority: a List<string> where a lower index means higher priority. Rules are sorted by this priority on construction.
/// - Expected rule keys (checked/used by this class):
///     - Constants.KEY_REASON (string)          : the reason identifier returned when the rule matches.
///     - Constants.KEY_DAYS_TO_EXPIRY (number)  : numeric threshold compared against contract.GetDaysToExpiry().
///     - Constants.KEY_MIN_ANNUAL_COST (number) : numeric threshold compared against contract.GetAnnualCost().
/// - SortRulesByPriority: orders incoming rules so FindTopReason can return the highest-priority match first.
/// - RuleMatches: evaluates each condition present in a rule. All conditions are conjunctive (AND).
///   If a condition key is missing the condition is skipped. Malformed values or exceptions are caught and logged,
///   and the rule is treated as not matching.
/// - FindTopReason: iterates sorted rules, returns the first rule's reason (ToString()) where RuleMatches == true,
///   otherwise returns Constants.NO_MATCH.
/// 
/// Notes and caveats:
/// - This implementation assumes rule values are JsonElement when coming from JSON; invalid casts will be caught at runtime.
/// - The class logs exceptions to Console and returns conservative defaults (no match) on errors.
/// - Priority lookup treats unknown reasons as lowest priority (Priority.Count).
/// - If you want stronger type-safety, consider deserializing rules into a typed model instead of Dictionary<string, object>.
/// </summary>
public class Config
{
    public List<string> Priority { get; private set; }
    public List<Dictionary<string, object>> Rules { get; private set; }

    public Config(List<Dictionary<string, object>> rules, List<string> priority)
    {
        Priority = priority;
        Rules = SortRulesByPriority(rules);
    }

    private List<Dictionary<string, object>> SortRulesByPriority(List<Dictionary<string, object>> rules)
    {
        return rules.OrderBy(rule =>
        {
            string ruleType = rule.ContainsKey(Constants.KEY_REASON) ? rule[Constants.KEY_REASON]?.ToString() : "";
            int index = Priority.IndexOf(ruleType);
            return index >= 0 ? index : Priority.Count;
        }).ToList();
    }

    private bool RuleMatches(Dictionary<string, object> rule, Contract contract)
    {
        try
        {
            if (rule.ContainsKey(Constants.KEY_DAYS_TO_EXPIRY))
            {
                if (contract.GetDaysToExpiry() > ((JsonElement)rule[Constants.KEY_DAYS_TO_EXPIRY]).GetInt32())
                    return false;
            }

            if (rule.ContainsKey(Constants.KEY_MIN_ANNUAL_COST))
            {
                if (contract.GetAnnualCost() < ((JsonElement)rule[Constants.KEY_MIN_ANNUAL_COST]).GetInt32())
                    return false;
            }

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error);
            Console.WriteLine("Contract doesn't have needed features to evaluate according to decision rules");
            return false;
        }
    }

    public string FindTopReason(Contract contract)
    {
        foreach (var rule in Rules)
        {
            if (RuleMatches(rule, contract) && rule[Constants.KEY_REASON] != null)
            {
                return rule[Constants.KEY_REASON].ToString();
            }
        }
        return Constants.NO_MATCH;
    }
}