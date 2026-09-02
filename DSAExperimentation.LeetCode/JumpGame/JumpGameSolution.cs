namespace DSAExperimentation.LeetCode.JumpGame;

// LeetCode 55. Jump Game: starting at index 0, nums[i] is the farthest you may
// jump forward from index i - can you reach the last index?
//
// The two strategies differ in how far ahead they look: an O(n^2) DP that
// marks every index reachable from each already-reachable index, or an O(n)
// greedy pass that only ever tracks the single farthest index reached so
// far. No repo primitive applies - this is a pure reachability scan over the
// array itself, the same "no stronger reusable primitive" shape already
// established for GasStation.
internal static class JumpGameSolution
{
    // The textbook DP: mark every index reachable from each already-reachable
    // index. Deliberately written without this repo's primitives - it is the
    // arm the greedy strategy below has to justify itself against.
    public static bool CanJumpByForwardReachabilityDp(int[] nums)
    {
        var reachable = new bool[nums.Length];
        reachable[0] = true;

        for (var i = 0; i < nums.Length; i++)
        {
            if (!reachable[i])
            {
                continue;
            }

            var maxStep = Math.Min(nums[i], nums.Length - 1 - i);

            for (var step = 1; step <= maxStep; step++)
            {
                reachable[i + step] = true;
            }
        }

        return reachable[^1];
    }

    // Track only the farthest index reached so far, stopping the scan the
    // moment the current index would exceed it.
    public static bool CanJumpByGreedyFarthestReach(int[] nums)
    {
        var reach = 0;

        for (var i = 0; i < nums.Length && i <= reach; i++)
        {
            reach = Math.Max(reach, i + nums[i]);
        }

        return reach >= nums.Length - 1;
    }
}
