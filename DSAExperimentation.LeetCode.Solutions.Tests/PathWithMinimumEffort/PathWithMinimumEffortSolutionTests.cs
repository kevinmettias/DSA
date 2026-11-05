using DSAExperimentation.LeetCode.Solutions.PathWithMinimumEffort;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PathWithMinimumEffort;

// Harness only. Both strategies are PathWithMinimumEffortSolution's - this file
// just pins them to a single-cell grid
// (no step is ever taken, so the effort is zero) and a rectangular grid, since
// the problem's m x n bound is not square and the benchmark only ever measures
// square workloads.
public sealed partial class PathWithMinimumEffortSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            {
                [[5]],
                0
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
    [MemberData(nameof(Cases))]
    public void MinimumEffortPathByBinarySearchFloodFill_Cases_ReturnsSmallestBottleneckEffort(
        int[][] heights, int expected) =>
        Assert.Equal(expected, PathWithMinimumEffortSolution.MinimumEffortPathByBinarySearchFloodFill(heights));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumEffortPathByHeapDijkstra_Cases_ReturnsSmallestBottleneckEffort(
        int[][] heights, int expected) =>
        Assert.Equal(expected, PathWithMinimumEffortSolution.MinimumEffortPathByHeapDijkstra(heights));
}
