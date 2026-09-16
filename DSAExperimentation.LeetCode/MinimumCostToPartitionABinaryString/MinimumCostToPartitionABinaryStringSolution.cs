using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.MinimumCostToPartitionABinaryString;

// LeetCode 3864. Minimum Cost to Partition a Binary String: the string's '1's
// are "sensitive" elements. A segment of length L with X sensitive elements
// costs flatCost if X == 0, else L * X * encCost; an even-length segment may
// instead be split into its two equal halves, at the sum of their own
// (recursively chosen) costs. Return the minimum achievable total for the whole
// string.
//
// Because a segment can only ever split into two EQUAL halves, the whole
// recursion is bounded by binaryString.Length's 2-adic valuation, not by
// log2 of its length - it is exactly a segment-tree-shaped divide and conquer,
// with no overlapping subproblems to memoize (each (left, right) pair is visited
// at most once), so both strategies below are plain recursion rather than routed
// through Memoizer.
//
// MinCostByFenwickRangeSum is the composed strategy: every recursive call
// needs "how many sensitive elements sit in [left, right]", which is exactly a
// range-sum query, so it builds this repo's own
// DataStructures.FenwickTree.FenwickTree<int, SumOperation<int>> over
// binaryString once and answers every call's sensitive-count in O(log n) via
// Query, rather than rescanning the segment. Building that Fenwick tree is real
// input construction, so per the hoisted-overload convention (see
// OpenTheLockSolution) it is charged to a prepared-input overload a benchmark
// can build once in [GlobalSetup].
//
// MinCostByLinearScanRecursion is the textbook baseline this has to justify
// itself against: the same recursion, but counting sensitive elements with a
// direct O(length) scan of the string every time - deliberately BCL-only (a
// bare string index loop).
internal static class MinimumCostToPartitionABinaryStringSolution
{
    public static long MinCostByLinearScanRecursion(string binaryString, int encCost, int flatCost) =>
        MinCostOfRange(
            (Left: 0, Right: binaryString.Length - 1),
            encCost,
            flatCost,
            new LinearScanSensitiveCount(binaryString));

    public static long MinCostByFenwickRangeSum(string binaryString, int encCost, int flatCost)
    {
        var sensitive = new int[binaryString.Length];
        for (var i = 0; i < binaryString.Length; i++)
        {
            var isSensitive = binaryString[i] == '1';
            sensitive[i] = isSensitive ? 1 : 0;
        }

        var ones = new FenwickTree<int, SumOperation<int>>(sensitive);

        return MinCostByFenwickRangeSum(ones, binaryString.Length, encCost, flatCost);
    }

    public static long MinCostByFenwickRangeSum(
        FenwickTree<int, SumOperation<int>> sensitiveCounts, int length, int encCost, int flatCost) =>
        MinCostOfRange(
            (Left: 0, Right: length - 1),
            encCost,
            flatCost,
            new FenwickRangeSumSensitiveCount(sensitiveCounts));

    // The single question a segment asks of the input - how many sensitive elements
    // does it hold - with both endpoints named together, so a count can never be taken
    // over a range handed in the wrong way round. Answering it is the one step the two
    // strategies differ in; what a count is used for belongs to the recursion below.
    private interface ISensitiveCount
    {
        int CountIn(int left, int right);
    }

    // The segment is one range, not two loose ends: every use of one endpoint is a
    // use of the other, and each half of an even-length split is another range of it.
    // The two strategies above differ only in how a range's sensitive count is
    // answered, so that one step arrives as countSensitive while the split recursion
    // itself is written once.
    private static long MinCostOfRange(
        (int Left, int Right) range, int encCost, int flatCost, ISensitiveCount countSensitive)
    {
        var (left, right) = range;
        var length = right - left + 1;
        var sensitiveCount = countSensitive.CountIn(left, right);
        var wholeCost = SegmentCost(length, sensitiveCount, encCost, flatCost);

        if (length % 2 != 0)
        {
            return wholeCost;
        }

        var mid = left + length / 2 - 1;
        var splitCost = MinCostOfRange((Left: left, Right: mid), encCost, flatCost, countSensitive)
            + MinCostOfRange((Left: mid + 1, Right: right), encCost, flatCost, countSensitive);

        return Math.Min(wholeCost, splitCost);
    }

    // The baseline arm's answer: a direct O(length) scan of the string it was built
    // over, so the whole recursion re-reads the segment every time it needs the count.
    private sealed class LinearScanSensitiveCount(string binaryString) : ISensitiveCount
    {
        public int CountIn(int left, int right)
        {
            var count = 0;
            for (var i = left; i <= right; i++)
            {
                if (binaryString[i] == '1')
                {
                    count++;
                }
            }

            return count;
        }
    }

    // The composed arm's answer: one range-sum query against this repo's Fenwick tree,
    // built once over binaryString and then answering every call in O(log n).
    private sealed class FenwickRangeSumSensitiveCount(FenwickTree<int, SumOperation<int>> sensitiveCounts)
        : ISensitiveCount
    {
        public int CountIn(int left, int right) => sensitiveCounts.Query(left, right);
    }

    private static long SegmentCost(int length, int sensitiveCount, int encCost, int flatCost) =>
        sensitiveCount == 0 ? flatCost : EncryptionCost(length, sensitiveCount, encCost);

    // A segment holding at least one sensitive element costs its length times that
    // count times the per-element encoding cost.
    private static long EncryptionCost(int length, int sensitiveCount, int encCost) =>
        (long)length * sensitiveCount * encCost;
}
