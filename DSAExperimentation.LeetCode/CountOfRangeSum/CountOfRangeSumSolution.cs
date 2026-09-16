using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountOfRangeSum;

// LeetCode 327. Count of Range Sum: count the index pairs i < j whose prefix-sum
// difference prefix[j]-prefix[i] falls in [lower, upper].
//
// The two strategies differ only in how they count, for each prefix[j], how many
// earlier prefix[i] already seen fall in [prefix[j]-upper, prefix[j]-lower] - a
// direct O(n^2) scan over every pair, or coordinate-compressing the prefix sums and
// sweeping left-to-right through this repo's own FenwickTree<int, SumOperation<int>>
// (a Binary Indexed Tree of counts), with each query's compressed bounds found via
// BinarySearch.LowerBound/UpperBound. Same coordinate-compression-plus-Fenwick-sweep
// shape CountOfSmallerNumbersAfterSelfSolution uses for LC 315, generalized from a
// single one-sided query to a two-sided [lower, upper] range.
internal static class CountOfRangeSumSolution
{
    // The textbook baseline: build the prefix-sum array once, then compare every
    // pair directly. O(n^2) time, no extra space beyond the prefix array.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int CountByPairwisePrefixScan(int[] nums, int lower, int upper)
    {
        var prefix = BuildPrefixSums(nums);
        var count = 0;

        for (var a = 0; a < prefix.Length; a++)
        {
            for (var b = a + 1; b < prefix.Length; b++)
            {
                var rangeSum = prefix[b] - prefix[a];

                if (rangeSum >= lower && rangeSum <= upper)
                {
                    count++;
                }
            }
        }

        return count;
    }

    // Coordinate-compress the prefix sums, then sweep left to right through a
    // FenwickTree of counts: before inserting prefix[j], query how many earlier
    // prefix[i] already inserted lie in [prefix[j]-upper, prefix[j]-lower].
    public static int CountByFenwickSweep(int[] nums, int lower, int upper)
    {
        var prefix = BuildPrefixSums(nums);
        var sortedDistinct = prefix.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<long>(sortedDistinct);
        var tree = new FenwickTree<int, SumOperation<int>>(sortedDistinct.Length);
        var count = 0;
        var index = new RangeSumIndex(sequence, tree);

        foreach (var prefixSum in prefix)
        {
            count += AccumulatePrefixSumContribution(index, prefixSum, lower, upper);
        }

        return count;
    }

    private static int AccumulatePrefixSumContribution(RangeSumIndex index, long prefixSum, int lower, int upper)
    {
        var loRank = BinarySearch.LowerBound(index.Sequence, prefixSum - upper);
        var hiRank = BinarySearch.UpperBound(index.Sequence, prefixSum - lower) - 1;

        var contribution = loRank <= hiRank ? index.Tree.Query(loRank, hiRank) : 0;

        var insertRank = BinarySearch.LowerBound(index.Sequence, prefixSum);
        index.Tree.Add(insertRank, 1);

        return contribution;
    }

    private static long[] BuildPrefixSums(int[] nums)
    {
        var prefix = new long[nums.Length + 1];

        for (var i = 0; i < nums.Length; i++)
        {
            prefix[i + 1] = prefix[i] + nums[i];
        }

        return prefix;
    }

    private readonly record struct RangeSumIndex(ArraySequence<long> Sequence, FenwickTree<int, SumOperation<int>> Tree);
}
