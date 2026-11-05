using DSAExperimentation.LeetCode.Solutions.ShortestPathInBinaryMatrix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShortestPathInBinaryMatrix;

// Harness only. See ShortestPathInBinaryMatrixSolution for the two strategies: the
// same 8-directional BFS over the BCL's Queue<T> and over this repo's own
// Queue<TElement>.
public sealed partial class ShortestPathInBinaryMatrixSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0, 0], [0, 1]], -1 },
            { [[0, 0, 0], [0, 1, 0], [0, 0, 0]], 4 },
            { [[0, 0, 0], [0, 0, 0], [0, 0, 0]], 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ShortestPathBinaryMatrixByBclQueue_Cases_ReturnsClearPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ShortestPathInBinaryMatrixSolution.ShortestPathBinaryMatrixByBclQueue(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ShortestPathBinaryMatrixByQueueFrontier_Cases_ReturnsClearPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ShortestPathInBinaryMatrixSolution.ShortestPathBinaryMatrixByQueueFrontier(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ShortestPathBinaryMatrixByGridShortestPath_Cases_ReturnsClearPathCellCount(
        int[][] grid, int expected) =>
        Assert.Equal(expected, ShortestPathInBinaryMatrixSolution.ShortestPathBinaryMatrixByGridShortestPath(grid));
}
