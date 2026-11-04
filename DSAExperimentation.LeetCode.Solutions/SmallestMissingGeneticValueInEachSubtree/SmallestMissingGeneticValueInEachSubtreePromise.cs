namespace DSAExperimentation.LeetCode.Solutions.SmallestMissingGeneticValueInEachSubtree;

// LC 2003's promise about its input: every node has a genetic value, and they are distinct.
// LeetCode's judge never poses an input that breaks it. A value repeated makes the ancestor chain
// stop where the rescan does not. Generated cases do not always keep the promise, so a harness that
// runs them asks this first.
internal static class SmallestMissingGeneticValueInEachSubtreePromise
{
    public static bool IsKeptBy(int[] parents, int[] nums) => parents.Length == nums.Length && nums.Distinct().Count() == nums.Length;
}
