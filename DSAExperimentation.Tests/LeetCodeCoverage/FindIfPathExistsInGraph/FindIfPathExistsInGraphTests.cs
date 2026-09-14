using DSAExperimentation.LeetCode.FindIfPathExistsInGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindIfPathExistsInGraph;

// Harness only. Both reachability strategies are FindIfPathExistsInGraphSolution's
// - this file just pins them to LeetCode's published examples, plus the degenerate
// single-node graph where source is already the destination and the disconnected
// chain where the whole source component has to be exhausted before the answer is
// no.
public sealed class FindIfPathExistsInGraphTests
{
    public static TheoryData<int, int[][], int, int, bool> Examples =>
        new()
        {
            { 3, [[0, 1], [1, 2], [2, 0]], 0, 2, true },
            { 6, [[0, 1], [0, 2], [3, 5], [5, 4], [4, 3]], 0, 5, false },
            { 1, [], 0, 0, true },
            { 4, [[0, 1], [1, 2], [2, 3]], 0, 3, true },
            { 5, [[0, 1], [2, 3], [3, 4]], 1, 4, false },
            { 5, [[0, 1], [2, 3], [3, 4]], 4, 2, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPathByDepthFirstSearch_LeetCodeExamples_ReportsWhetherDestinationIsReachable(
        int nodeCount, int[][] edges, int source, int destination, bool expected) =>
        Assert.Equal(
            expected,
            FindIfPathExistsInGraphSolution.HasPathByDepthFirstSearch(nodeCount, edges, source, destination));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPathByDisjointSet_LeetCodeExamples_ReportsWhetherDestinationIsReachable(
        int nodeCount, int[][] edges, int source, int destination, bool expected) =>
        Assert.Equal(
            expected, FindIfPathExistsInGraphSolution.HasPathByDisjointSet(nodeCount, edges, source, destination));
}
