using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.MinimumCostToPartitionABinaryString;

// LeetCode 3864. Minimum Cost to Partition a Binary String: s's '1's are
// "sensitive" elements. A segment of length L with X sensitive elements costs
// flatCost if X == 0, else L * X * encCost; an even-length segment may instead
// be split into its two equal halves, at the sum of their own (recursively
// chosen) costs. Return the minimum achievable total for the whole string.
//
// Because a segment can only ever split into two EQUAL halves, the whole
// recursion is bounded by s.Length's 2-adic valuation, not by log2(s.Length) -
// it is exactly a segment-tree-shaped divide and conquer, with no overlapping
// subproblems to memoize (each (left, right) pair is visited at most once), so
// both strategies below are plain recursion rather than routed through
// Memoizer.
//
// MinCostByFenwickRangeSum is the composed strategy: every recursive call
// needs "how many sensitive elements sit in [left, right]", which is exactly a
// range-sum query, so it builds this repo's own
// DataStructures.FenwickTree.FenwickTree<int, SumOperation<int>> over s once
// and answers every call's sensitive-count in O(log n) via Query, rather than
// rescanning the segment. Building that Fenwick tree is real input
// construction, so per the hoisted-overload convention (see
// OpenTheLockSolution) it is charged to a prepared-input overload a benchmark
// can build once in [GlobalSetup].
//
// MinCostByLinearScanRecursion is the textbook baseline this has to justify
// itself against: the same recursion, but counting sensitive elements with a
// direct O(length) scan of s every time - deliberately BCL-only (a bare
// string index loop).
internal static class MinimumCostToPartitionABinaryStringSolution
{
    public static long MinCostByLinearScanRecursion(string s, int encCost, int flatCost) =>
        LinearScanRecursion(s, left: 0, right: s.Length - 1, encCost, flatCost);

    private static long LinearScanRecursion(string s, int left, int right, int encCost, int flatCost)
    {
        var length = right - left + 1;
        var sensitiveCount = CountSensitive(s, left, right);
        var wholeCost = SegmentCost(length, sensitiveCount, encCost, flatCost);

        if (length % 2 != 0)
        {
            return wholeCost;
        }

        var mid = left + length / 2 - 1;
        var splitCost = LinearScanRecursion(s, left, mid, encCost, flatCost)
            + LinearScanRecursion(s, mid + 1, right, encCost, flatCost);

        return Math.Min(wholeCost, splitCost);
    }

    private static int CountSensitive(string s, int left, int right)
    {
        var count = 0;
        for (var i = left; i <= right; i++)
        {
            if (s[i] == '1')
            {
                count++;
            }
        }

        return count;
    }

    public static long MinCostByFenwickRangeSum(string s, int encCost, int flatCost)
    {
        var sensitive = new int[s.Length];
        for (var i = 0; i < s.Length; i++)
        {
            sensitive[i] = s[i] == '1' ? 1 : 0;
        }

        var ones = new FenwickTree<int, SumOperation<int>>(sensitive);

        return MinCostByFenwickRangeSum(ones, s.Length, encCost, flatCost);
    }

    public static long MinCostByFenwickRangeSum(
        FenwickTree<int, SumOperation<int>> sensitiveCounts, int length, int encCost, int flatCost) =>
        FenwickRangeSumRecursion(sensitiveCounts, left: 0, right: length - 1, encCost, flatCost);

    private static long FenwickRangeSumRecursion(
        FenwickTree<int, SumOperation<int>> sensitiveCounts, int left, int right, int encCost, int flatCost)
    {
        var length = right - left + 1;
        var sensitiveCount = sensitiveCounts.Query(left, right);
        var wholeCost = SegmentCost(length, sensitiveCount, encCost, flatCost);

        if (length % 2 != 0)
        {
            return wholeCost;
        }

        var mid = left + length / 2 - 1;
        var splitCost = FenwickRangeSumRecursion(sensitiveCounts, left, mid, encCost, flatCost)
            + FenwickRangeSumRecursion(sensitiveCounts, mid + 1, right, encCost, flatCost);

        return Math.Min(wholeCost, splitCost);
    }

    private static long SegmentCost(int length, int sensitiveCount, int encCost, int flatCost) =>
        sensitiveCount == 0 ? flatCost : (long)length * sensitiveCount * encCost;
}
