namespace DSAExperimentation.LeetCode.Solutions.ArrayPartition;

// LC 561's promise about its input: nums holds 2n integers, an even count, so every one of them is
// paired. LeetCode's judge never poses an input that breaks it. Given an odd count, the strategies
// leave a different integer unpaired. Generated cases do not always keep the promise, so a harness
// that runs them asks this first.
internal static class ArrayPartitionPromise
{
    public static bool IsKeptBy(int[] nums) => nums.Length % ArrayPartitionSolution.PairSize == 0;
}
