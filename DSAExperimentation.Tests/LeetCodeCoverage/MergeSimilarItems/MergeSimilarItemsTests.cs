using DSAExperimentation.LeetCode.MergeSimilarItems;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeSimilarItems;

// Harness only. Both strategies live in MergeSimilarItemsSolution - including the
// quadratic linear scan, which used to exist as an unasserted benchmark baseline
// that only counted its results instead of returning them - and this file pins
// them to the same examples so a failure names the strategy that broke.
public sealed class MergeSimilarItemsTests
{
    public static TheoryData<int[][], int[][], (int Value, int Weight)[]> Examples =>
        new()
        {
            // LeetCode example 1.
            {
                [[1, 1], [4, 5], [3, 8]],
                [[3, 1], [1, 5]],
                [(1, 6), (3, 9), (4, 5)]
            },

            // LeetCode example 2: every value appears in both lists.
            {
                [[1, 1], [3, 2], [2, 3]],
                [[2, 1], [3, 2], [1, 3]],
                [(1, 4), (2, 4), (3, 4)]
            },

            // LeetCode example 3: one shared value, one unique to each side.
            {
                [[1, 3], [2, 2]],
                [[7, 1], [2, 2], [1, 4]],
                [(1, 7), (2, 4), (7, 1)]
            },

            // Disjoint values only, arriving out of order, so the sort has to do
            // all the ordering work.
            {
                [[9, 2], [4, 1]],
                [[6, 3], [2, 7]],
                [(2, 7), (4, 1), (6, 3), (9, 2)]
            },

            // One side empty - the other list passes straight through, sorted.
            {
                [[5, 4], [1, 2]],
                [],
                [(1, 2), (5, 4)]
            },

            // A single value on each side, shared.
            {
                [[1, 1]],
                [[1, 1]],
                [(1, 2)]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByLinearScan_LeetCodeExamples_SumsSharedWeightsAscendingByValue(
        int[][] items1, int[][] items2, (int Value, int Weight)[] expected) =>
        Assert.Equal(expected, MergeSimilarItemsSolution.MergeByLinearScan(items1, items2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByHashMapAndMergeSort_LeetCodeExamples_SumsSharedWeightsAscendingByValue(
        int[][] items1, int[][] items2, (int Value, int Weight)[] expected) =>
        Assert.Equal(expected, MergeSimilarItemsSolution.MergeByHashMapAndMergeSort(items1, items2));
}
