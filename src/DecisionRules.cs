using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public class DecisionRules
{
    public List<string> Priority { get; private set; }
    public List<Dictionary<string, object>> Rules { get; private set; }

    public DecisionRules(List<Dictionary<string, object>> rules, List<string> priority)
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
            if (RuleMatches(rule, contract))
            {
                return rule[Constants.KEY_REASON].ToString();
            }
        }
        return Constants.NO_MATCH;
    }
}