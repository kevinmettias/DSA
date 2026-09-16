using DSAExperimentation.LeetCode.FindKthLargestXorCoordinateValue;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindKthLargestXorCoordinateValue;

// Harness only. Both selection strategies are
// FindKthLargestXorCoordinateValueSolution's; this file pins them to all four of
// LeetCode's published examples - including rank == 4, the rank the original coverage
// omitted, whose answer is the 0 that the corner cancellation produces - plus two
// degenerate shapes where one dimension is a single cell.
public sealed partial class FindKthLargestXorCoordinateValueTests
{
    public static TheoryData<int[][], int, int> Examples =>
        new()
        {
            { [[5, 2], [1, 6]], 1, 7 },
            { [[5, 2], [1, 6]], 2, 5 },
            { [[5, 2], [1, 6]], 3, 4 },
            { [[5, 2], [1, 6]], 4, 0 },
            { [[8]], 1, 8 },
            { [[1, 2, 3]], 1, 3 },
            { [[1, 2, 3]], 2, 1 },
            { [[1, 2, 3]], 3, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthLargestValueByFullSort_LeetCodeExamples_ReturnsCorrectRank(
        int[][] matrix, int rank, int expected)
    {
        var actual = FindKthLargestXorCoordinateValueSolution.KthLargestValueByFullSort(matrix, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthLargestValueBySizeKHeap_LeetCodeExamples_ReturnsCorrectRank(
        int[][] matrix, int rank, int expected)
    {
        var actual = FindKthLargestXorCoordinateValueSolution.KthLargestValueBySizeKHeap(matrix, rank);

        Assert.Equal(expected, actual);
    }
}
