namespace DSAExperimentation.LeetCode.Solutions.JumpGameII;

// LC 45's promise about its input: the last index can be reached. LeetCode's judge never poses an
// input that breaks it. The breadth-first distances have no distance to an index nothing reaches.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class JumpGameIIPromise
{
    public static bool IsKeptBy(int[] nums)
    {
        var farthest = 0;

        for (var index = 0; index < nums.Length && index <= farthest; index++)
        {
            farthest = Math.Max(farthest, index + nums[index]);
        }

        return farthest >= nums.Length - 1;
    }
}
