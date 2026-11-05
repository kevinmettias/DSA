using DSAExperimentation.LeetCode.Solutions.RelativeSortArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RelativeSortArray;

// Harness only. Both orderings are RelativeSortArraySolution's; this file pins them
// to the two edges the original test never
// covered - an arr1 with nothing ranked at all, and an arr1 with nothing unranked.
public sealed partial class RelativeSortArraySolutionTests
{
    public static TheoryData<int[], int[], int[]> Cases =>
        new()
        {
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
    [MemberData(nameof(Cases))]
    public void RelativeSortByLinearScanComparer_Cases_RanksByArr2ThenAppendsRemainderAscending(
        int[] arr1, int[] arr2, int[] expected)
    {
        var actual = RelativeSortArraySolution.RelativeSortByLinearScanComparer(arr1, arr2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void RelativeSortByHashMapMergeSort_Cases_RanksByArr2ThenAppendsRemainderAscending(
        int[] arr1, int[] arr2, int[] expected)
    {
        var actual = RelativeSortArraySolution.RelativeSortByHashMapMergeSort(arr1, arr2);

        Assert.Equal(expected, actual);
    }
}
