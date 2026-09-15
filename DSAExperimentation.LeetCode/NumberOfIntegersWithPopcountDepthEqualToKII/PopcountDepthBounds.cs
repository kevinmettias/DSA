namespace DSAExperimentation.LeetCode.NumberOfIntegersWithPopcountDepthEqualToKII;

// The largest popcount depth LC 3624 can ever ask about. The constraint says
// 0 <= k <= 5, and 5 is also a provable upper bound on any depth the real data can
// reach: nums[i] <= 10^15 fits in 50 bits, so popcount(nums[i]) <= 50, and the
// popcount chain from any value <= 50 reaches 1 in at most 4 further steps. Index
// [0, MaxTrackedDepth] therefore covers every depth a value within the problem's
// own bounds can have.
//
// Owned here rather than in PopcountDepthFenwickIndex because the benchmark draws
// its random k from the same bound - the bound is the problem's, not the index's.
internal static class PopcountDepthBounds
{
    public const int MaxTrackedDepth = 5;
}
