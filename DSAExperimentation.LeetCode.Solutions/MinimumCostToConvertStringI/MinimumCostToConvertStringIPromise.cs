namespace DSAExperimentation.LeetCode.Solutions.MinimumCostToConvertStringI;

// LC 2976's promise about its input: source and target are as long as each other, every rule has its
// original, its change and its cost, every letter is a lowercase English one, and no rule changes a letter
// into itself. LeetCode's judge never poses an input that breaks it. Both strategies read target[i] beside
// source[i] and cost[i] beside original[i], so a target shorter than its source, or fewer costs than rules,
// runs them off the end of an array. Generated cases do not always keep the promise, so a harness that runs
// them asks this first.
internal static class MinimumCostToConvertStringIPromise
{
    public static bool IsKeptBy(
        MinimumCostToConvertStringISolution.SourceText source,
        MinimumCostToConvertStringISolution.TargetText target,
        (char[] Original, char[] Changed, int[] Cost) rules)
    {
        var isEachRuleWhole = rules.Original.Length == rules.Changed.Length && rules.Changed.Length == rules.Cost.Length;
        var letters = source.Text.Concat(target.Text).Concat(rules.Original).Concat(rules.Changed);
        var isEachRuleAChange = rules.Original.Zip(rules.Changed).All(rule => rule.First != rule.Second);

        return source.Text.Length == target.Text.Length && isEachRuleWhole && isEachRuleAChange && letters.All(char.IsAsciiLetterLower);
    }
}
