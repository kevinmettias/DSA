using DSAExperimentation.LeetCode.ShortestPathInBinaryMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathInBinaryMatrix;

// Harness only. See ShortestPathInBinaryMatrixSolution for the two strategies: the
// same 8-directional BFS over the BCL's Queue<T> and over this repo's own
// Queue<TElement>.
public sealed partial class ShortestPathInBinaryMatrixTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 1], [1, 0]], 2 },
            { [[0, 0, 0], [1, 1, 0], [1, 1, 0]], 4 },
            { [[1, 0, 0], [1, 1, 0], [1, 1, 0]], -1 },
            { [[0]], 1 },
            { [[1]], -1 },
            { [[0, 0], [0, 1]], -1 },
            { [[0, 0, 0], [0, 1, 0], [0, 0, 0]], 4 },
            { [[0, 0, 0], [0, 0, 0], [0, 0, 0]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestPathBinaryMatrixByBclQueue_LeetCodeExamples_ReturnsClearPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ShortestPathInBinaryMatrixSolution.ShortestPathBinaryMatrixByBclQueue(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShortestPathBinaryMatrixByQueueFrontier_LeetCodeExamples_ReturnsClearPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ShortestPathInBinaryMatrixSolution.ShortestPathBinaryMatrixByQueueFrontier(grid));
}
