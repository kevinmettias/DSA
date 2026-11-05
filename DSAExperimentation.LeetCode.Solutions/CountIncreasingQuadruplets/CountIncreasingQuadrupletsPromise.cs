namespace DSAExperimentation.LeetCode.Solutions.CountIncreasingQuadruplets;

// LC 2552's promise about its input: "nums is a permutation" of 1 to n, every value once. LeetCode's judge
// never poses an input that breaks it. The Fenwick sweep indexes its tree by value, so a value past n
// falls outside it. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class CountIncreasingQuadrupletsPromise
{
    public static bool IsKeptBy(int[] nums) => nums.Order().SequenceEqual(Enumerable.Range(1, nums.Length));
}
