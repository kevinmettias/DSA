using DSAExperimentation.LeetCode.Solutions.FindKthLargestXorCoordinateValue;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindKthLargestXorCoordinateValue;

// Harness only. Both selection strategies are
// FindKthLargestXorCoordinateValueSolution's; this file pins them to rank == 4 on
// LeetCode's example matrix - the rank its published examples omit, whose answer is
// the 0 that the corner cancellation produces - plus two degenerate shapes where one
// dimension is a single cell. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class FindKthLargestXorCoordinateValueSolutionTests
{
    public static TheoryData<int[][], int, int> Cases =>
        new()
        {
            { [[5, 2], [1, 6]], 4, 0 },
            { [[8]], 1, 8 },
            { [[1, 2, 3]], 1, 3 },
            { [[1, 2, 3]], 2, 1 },
            { [[1, 2, 3]], 3, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void KthLargestValueByFullSort_Cases_ReturnsCorrectRank(
        int[][] matrix, int rank, int expected)
    {
        var actual = FindKthLargestXorCoordinateValueSolution.KthLargestValueByFullSort(matrix, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void KthLargestValueBySizeKHeap_Cases_ReturnsCorrectRank(
        int[][] matrix, int rank, int expected)
    {
        var actual = FindKthLargestXorCoordinateValueSolution.KthLargestValueBySizeKHeap(matrix, rank);

        Assert.Equal(expected, actual);
    }
}
