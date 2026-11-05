namespace DSAExperimentation.LeetCode.Solutions.ConstructStringWithMinimumCost;

// LC 3213's promise about its input: words and costs have one entry each per word. LeetCode's judge
// never poses an input that breaks it. Both strategies read a word's cost at the word's index.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class ConstructStringWithMinimumCostPromise
{
    public static bool IsKeptBy(string target, string[] words, int[] costs) => words.Length == costs.Length;
}
