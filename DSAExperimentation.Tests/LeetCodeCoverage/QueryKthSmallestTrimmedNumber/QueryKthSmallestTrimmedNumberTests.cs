using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueryKthSmallestTrimmedNumber;

// LeetCode 2343. Query Kth Smallest Trimmed Number: for each query, trim every number to its last
// `trim` digits (all numbers are equal length, so comparing equal-length digit suffixes
// lexicographically is the same order as comparing them numerically) and report which ORIGINAL
// index lands at position k in trimmed order. Composes this repo's own MergeSort over an index
// array - ThreeSumWithMultiplicityTests' exact "sort indices, not values, via
// ArrayIndexedSequence<int> + IComparer<int>" shape - with a comparer over each index's trimmed
// suffix. MergeSort.cs's own doc comment states the merge step is stable (its `<= 0` tie rule),
// which is exactly the "smaller original index wins on a trimmed-value tie" rule LeetCode's own
// definition of "smallest trimmed number" requires, with zero extra tie-breaking bookkeeping.
public sealed partial class QueryKthSmallestTrimmedNumberTests
{
    [Fact]
    public void AnswerQueries_LeetCodeExampleOne_ReturnsExpectedIndices()
    {
        string[] nums = ["102", "473", "251", "814"];
        int[][] queries = [[1, 1], [2, 3], [4, 2], [1, 2]];

        Assert.Equal([2, 2, 1, 0], AnswerQueries(nums, queries));
    }

    [Fact]
    public void AnswerQueries_LeetCodeExampleTwo_ReturnsExpectedIndices()
    {
        string[] nums = ["24", "37", "96", "04"];
        int[][] queries = [[2, 1], [2, 2]];

        Assert.Equal([3, 0], AnswerQueries(nums, queries));
    }

    [Fact]
    public void AnswerQueries_TieAfterTrimming_BreaksTowardSmallerOriginalIndex()
    {
        string[] nums = ["11", "11"];
        int[][] queries = [[1, 1], [2, 1]];

        Assert.Equal([0, 1], AnswerQueries(nums, queries));
    }

    private static int[] AnswerQueries(string[] nums, int[][] queries)
        => queries.Select(q => AnswerQuery(nums, k: q[0], trim: q[1])).ToArray();

    private static int AnswerQuery(string[] nums, int k, int trim)
    {
        var indices = Enumerable.Range(0, nums.Length).ToArray();
        var comparer = Comparer<int>.Create((a, b) => CompareTrimmed(nums[a], nums[b], trim));

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(indices), comparer);

        return indices[k - 1];
    }

    private static int CompareTrimmed(string first, string second, int trim)
        => string.CompareOrdinal(first[^trim..], second[^trim..]);
}
