using DSAExperimentation.LeetCode.Solutions.MergeSimilarItems;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MergeSimilarItems;

// Harness only. Both strategies live in MergeSimilarItemsSolution - including the
// quadratic linear scan, which used to exist as an unasserted benchmark baseline
// that only counted its results instead of returning them - and this file pins
// them to the same cases so a failure names the strategy that broke.
public sealed partial class MergeSimilarItemsSolutionTests
{
    public static TheoryData<int[][], int[][], (int Value, int Weight)[]> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void MergeByLinearScan_Cases_SumsSharedWeightsAscendingByValue(
        int[][] items1, int[][] items2, (int Value, int Weight)[] expected)
    {
        var actual = MergeSimilarItemsSolution.MergeByLinearScan(items1, items2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MergeByHashMapAndMergeSort_Cases_SumsSharedWeightsAscendingByValue(
        int[][] items1, int[][] items2, (int Value, int Weight)[] expected)
    {
        var actual = MergeSimilarItemsSolution.MergeByHashMapAndMergeSort(items1, items2);

        Assert.Equal(expected, actual);
    }
}
