using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ReversePairs;

// LeetCode 493. Reverse Pairs: count index pairs i < j with nums[i] > 2 * nums[j].
//
// The naive strategy is the textbook O(n^2) pairwise scan; the composed strategy
// coordinate-compresses nums (as long, since 2*value can overflow a 32-bit int) via
// BinarySearch.LowerBound/UpperBound over the sorted distinct values, then sweeps
// left-to-right through a FenwickTree<int, SumOperation<int>> (this repo's own Binary
// Indexed Tree) - before inserting nums[j]'s own rank, UpperBound locates the first
// rank strictly greater than 2*nums[j], and a single Query over [rank, end] counts
// every earlier nums[i] that forms a reverse pair with it. Same coordinate-
// compression-plus-Fenwick-sweep shape CountOfSmallerNumbersAfterSelf and
// CountOfRangeSum use, generalized to a value-dependent (not fixed) split point.
internal static class ReversePairsSolution
{
    private const long ReversePairMultiplier = 2L;

    // The textbook O(n^2) baseline: every pair, compared directly. Deliberately
    // written without this repo's primitives - it is the arm the Fenwick sweep has
    // to justify itself against.
    public static int CountByPairwiseScan(int[] nums)
    {
        var count = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if ((long)nums[i] > ReversePairMultiplier * nums[j])
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Coordinate-compression-plus-Fenwick sweep, O(n log n) overall.
    public static int CountByFenwickTreeSweep(int[] nums)
    {
        var sortedDistinct = nums.Select(value => (long)value).Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var count = 0;

        foreach (var value in nums)
        {
            var firstGreaterRank = BinarySearch.UpperBound(sequence, ReversePairMultiplier * value);

            if (firstGreaterRank < sortedDistinct.Length)
            {
                count += tree.Query(firstGreaterRank, sortedDistinct.Length - 1);
            }

            var rank = BinarySearch.LowerBound(sequence, (long)value);
            tree.Add(rank, 1);
        }

        return count;
    }
}
