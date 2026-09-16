using DSAExperimentation.LeetCode.PathWithMinimumEffort;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathWithMinimumEffort;

// Harness only. Both strategies are PathWithMinimumEffortSolution's - this file
// just pins them to LeetCode's three published examples, plus a single-cell grid
// (no step is ever taken, so the effort is zero) and a rectangular grid, since
// the problem's m x n bound is not square and the benchmark only ever measures
// square workloads.
public sealed partial class PathWithMinimumEffortTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [
                    [1, 2, 2],
                    [3, 8, 2],
                    [5, 3, 5],
                ],
                2
            },
            {
                [
                    [1, 2, 3],
                    [3, 8, 4],
                    [5, 3, 5],
                ],
                1
            },
            {
                [
                    [1, 2, 1, 1, 1],
                    [1, 2, 1, 2, 1],
                    [1, 2, 1, 2, 1],
                    [1, 2, 1, 2, 1],
                    [1, 1, 1, 2, 1],
                ],
                0
            },
            {
                [[5]],
                0
            },
            {
                [
                    [1, 10, 6, 7, 9, 10, 4, 9],
                ],
                9
            },
            {
                [
                    [3],
                    [9],
                    [4],
                ],
                6
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumEffortPathByBinarySearchFloodFill_LeetCodeExamples_ReturnsSmallestBottleneckEffort(
        int[][] heights, int expected) =>
        Assert.Equal(expected, PathWithMinimumEffortSolution.MinimumEffortPathByBinarySearchFloodFill(heights));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumEffortPathByHeapDijkstra_LeetCodeExamples_ReturnsSmallestBottleneckEffort(
        int[][] heights, int expected) =>
        Assert.Equal(expected, PathWithMinimumEffortSolution.MinimumEffortPathByHeapDijkstra(heights));
}
