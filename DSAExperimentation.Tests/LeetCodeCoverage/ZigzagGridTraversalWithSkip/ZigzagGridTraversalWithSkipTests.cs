using DSAExperimentation.LeetCode.ZigzagGridTraversalWithSkip;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ZigzagGridTraversalWithSkip;

// Harness only. Both traversal strategies are ZigzagGridTraversalWithSkipSolution's
// - this file just pins them to LeetCode's published examples.
public sealed partial class ZigzagGridTraversalWithSkipTests
{
    public static TheoryData<int[][], int[]> Examples =>
        new()
        {
            { [[1, 2], [3, 4]], [1, 4] },
            { [[2, 1], [2, 1], [2, 1]], [2, 1, 2] },
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], [1, 3, 5, 7, 9] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TraverseByIndexFormula_LeetCodeExamples_ReturnsSkippedZigzagOrder(int[][] grid, int[] expected) =>
        Assert.Equal(expected, ZigzagGridTraversalWithSkipSolution.TraverseByIndexFormula(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TraverseByRowStack_LeetCodeExamples_ReturnsSkippedZigzagOrder(int[][] grid, int[] expected) =>
        Assert.Equal(expected, ZigzagGridTraversalWithSkipSolution.TraverseByRowStack(grid));
}
