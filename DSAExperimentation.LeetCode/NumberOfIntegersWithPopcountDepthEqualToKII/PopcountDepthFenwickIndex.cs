using System.Numerics;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.NumberOfIntegersWithPopcountDepthEqualToKII;

// Tracks, for every popcount-depth value the problem's own "0 <= k <= 5"
// bound can ever ask about, one point-update/range-sum
// FenwickTree<int, SumOperation<int>> over "does index j currently hold a
// value at this depth" - so a [1, l, r, k] query is one Query(l, r) on the
// k-th tree, and a [2, idx, val] update touches at most two trees (Add(-1) on
// the old depth, Add(+1) on the new one) instead of anything being rescanned.
// This answers LC 3624 alone - the depth bound baked into PopcountDepthBounds.MaxTrackedDepth
// comes from this problem's own constraint, not a general property of
// popcount-depth - which is why it lives beside the solution rather than in
// Domain.
internal sealed class PopcountDepthFenwickIndex(
    FenwickTree<int, SumOperation<int>>[] treesByDepth, int[] depthByIndex)
{
    public static PopcountDepthFenwickIndex Build(long[] nums)
    {
        var treesByDepth = new FenwickTree<int, SumOperation<int>>[PopcountDepthBounds.MaxTrackedDepth + 1];

        for (var depth = 0; depth <= PopcountDepthBounds.MaxTrackedDepth; depth++)
        {
            treesByDepth[depth] = new FenwickTree<int, SumOperation<int>>(nums.Length);
        }

        var depthByIndex = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            depthByIndex[i] = Depth(nums[i]);
            treesByDepth[depthByIndex[i]].Add(i, 1);
        }

        return new PopcountDepthFenwickIndex(treesByDepth, depthByIndex);
    }

    public int Count(int left, int right, int depth) => treesByDepth[depth].Query(left, right);

    public void Update(int index, long value)
    {
        var newDepth = Depth(value);
        var oldDepth = depthByIndex[index];

        if (newDepth == oldDepth)
        {
            return;
        }

        treesByDepth[oldDepth].Add(index, -1);
        treesByDepth[newDepth].Add(index, 1);
        depthByIndex[index] = newDepth;
    }

    private static int Depth(long x)
    {
        var depth = 0;

        while (x != 1)
        {
            x = BitOperations.PopCount((ulong)x);
            depth++;
        }

        return depth;
    }
}
