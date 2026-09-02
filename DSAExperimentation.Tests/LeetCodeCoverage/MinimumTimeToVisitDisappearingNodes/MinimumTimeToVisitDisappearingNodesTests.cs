using DSAExperimentation.LeetCode.MinimumTimeToVisitDisappearingNodes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToVisitDisappearingNodes;

// Harness only. Both strategies are MinimumTimeToVisitDisappearingNodesSolution's
// - this file just pins them to LeetCode's published examples, including the two
// otherwise-identical graphs that only differ in disappear[] (Example 2 relaxes
// Example 1's deadlines just enough to make node 1 reachable) and the exact-tie
// case (a node reached exactly when it disappears still counts as unreachable).
public sealed class MinimumTimeToVisitDisappearingNodesTests
{
    public static TheoryData<int, int[][], int[], int[]> Examples =>
        new()
        {
            { 3, [[0, 1, 2], [1, 2, 1], [0, 2, 4]], [1, 1, 5], [0, -1, 4] },
            { 3, [[0, 1, 2], [1, 2, 1], [0, 2, 4]], [1, 3, 5], [0, 2, 3] },
            { 2, [[0, 1, 1]], [1, 1], [0, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimesByDijkstraQueue_LeetCodeExamples_ReturnsEarliestArrivalTimes(
        int n, int[][] edges, int[] disappear, int[] expected) =>
        Assert.Equal(
            expected, MinimumTimeToVisitDisappearingNodesSolution.MinimumTimesByDijkstraQueue(n, edges, disappear));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimesByPriorityHeap_LeetCodeExamples_ReturnsEarliestArrivalTimes(
        int n, int[][] edges, int[] disappear, int[] expected) =>
        Assert.Equal(
            expected, MinimumTimeToVisitDisappearingNodesSolution.MinimumTimesByPriorityHeap(n, edges, disappear));
}
