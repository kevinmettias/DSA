namespace DSAExperimentation.LeetCode.Solutions.NumberOfWaysToReorderArrayToGetSameBST;

// LC 1569's promise about its input: nums is a permutation of 1 to n. LeetCode's judge never poses
// an input that breaks it. A repeated value has no place in a search tree, and the strategies each
// put it somewhere else. Generated cases do not always keep the promise, so a harness that runs
// them asks this first.
internal static class NumberOfWaysToReorderArrayToGetSameBSTPromise
{
    public static bool IsKeptBy(int[] nums)
    {
        var oneToN = Enumerable.Range(1, nums.Length);

        return nums.Order().SequenceEqual(oneToN);
    }
}
