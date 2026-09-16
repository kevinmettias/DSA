using DSAExperimentation.LeetCode.RelativeSortArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RelativeSortArray;

// Harness only. Both orderings are RelativeSortArraySolution's; this file pins them
// to LeetCode's published examples plus the two edges the original test never
// covered - an arr1 with nothing ranked at all, and an arr1 with nothing unranked.
public sealed partial class RelativeSortArrayTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            {
                [2, 3, 1, 3, 2, 4, 6, 7, 9, 2, 19],
                [2, 1, 4, 3, 9, 6],
                [2, 2, 2, 1, 4, 3, 3, 9, 6, 7, 19]
            },
            {
                [28, 6, 22, 8, 44, 17],
                [22, 28, 8, 6],
                [22, 28, 8, 6, 17, 44]
            },
            {
                [5, 3, 9, 1],
                [100],
                [1, 3, 5, 9]
            },
            {
                [4, 4, 1, 1, 7],
                [7, 1, 4],
                [7, 1, 1, 4, 4]
            },
            {
                [1],
                [1],
                [1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RelativeSortByLinearScanComparer_LeetCodeExamples_RanksByArr2ThenAppendsRemainderAscending(
        int[] arr1, int[] arr2, int[] expected)
    {
        var actual = RelativeSortArraySolution.RelativeSortByLinearScanComparer(arr1, arr2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RelativeSortByHashMapMergeSort_LeetCodeExamples_RanksByArr2ThenAppendsRemainderAscending(
        int[] arr1, int[] arr2, int[] expected)
    {
        var actual = RelativeSortArraySolution.RelativeSortByHashMapMergeSort(arr1, arr2);

        Assert.Equal(expected, actual);
    }
}
