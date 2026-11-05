namespace DSAExperimentation.LeetCode.Solutions.MinimumCostToConvertStringII;

// LC 2977's promise about its input: source and target are as long as each other, every rule has its
// original, its change and its cost, each rule's original and change are as long as each other and differ,
// and every string holds lowercase English letters only. LeetCode's judge never poses an input that breaks
// it. Both strategies read target beside source, cost[i] beside original[i], and a rule's change over as
// many letters as its original, so a target shorter than its source, or fewer costs than rules, runs them off
// the end of an array. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class MinimumCostToConvertStringIIPromise
{
    public static bool IsKeptBy(SourceText source, TargetText target, (string[] Original, string[] Changed, int[] Cost) rules)
    {
        var isEachRuleWhole = rules.Original.Length == rules.Changed.Length && rules.Changed.Length == rules.Cost.Length;
        var texts = rules.Original.Concat(rules.Changed).Append(source.Text).Append(target.Text);
        var isEachRuleAChange = rules.Original.Zip(rules.Changed).All(rule => rule.First.Length == rule.Second.Length && rule.First != rule.Second);

        return source.Text.Length == target.Text.Length && isEachRuleWhole && isEachRuleAChange
            && texts.All(text => text.All(char.IsAsciiLetterLower));
    }
}
