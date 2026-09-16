using DSAExperimentation.LeetCode.ZeroOneMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ZeroOneMatrix;

// LeetCode 542. 01 Matrix. See ZeroOneMatrixSolution for the two strategies: a
// naive per-cell BFS baseline, and this repo's own multi-source BFS composed over
// Queue<TElement>.
public sealed class ZeroOneMatrixTests
{
    public static TheoryData<int[][], int[][]> Examples()
    {
        var examples = new TheoryData<int[][], int[][]>
        {
            { [[0, 0, 0], [0, 1, 0], [0, 0, 0]], [[0, 0, 0], [0, 1, 0], [0, 0, 0]] },
            { [[0, 0, 0], [0, 1, 0], [1, 1, 1]], [[0, 0, 0], [0, 1, 0], [1, 2, 1]] },
            { [[0]], [[0]] },
        };

        return examples;
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void UpdateMatrixByPerCellBfs_ReturnsDistanceToNearestZero(int[][] mat, int[][] expected)
        => Assert.Equal(expected, ZeroOneMatrixSolution.UpdateMatrixByPerCellBfs(mat));

    [Theory]
    [MemberData(nameof(Examples))]
    public void UpdateMatrixByMultiSourceBfs_ReturnsDistanceToNearestZero(int[][] mat, int[][] expected)
        => Assert.Equal(expected, ZeroOneMatrixSolution.UpdateMatrixByMultiSourceBfs(mat));
}
