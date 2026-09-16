using DSAExperimentation.LeetCode.FindIfPathExistsInGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindIfPathExistsInGraph;

// Harness only. Both reachability strategies are FindIfPathExistsInGraphSolution's
// - this file just pins them to LeetCode's published examples, plus the degenerate
// single-node graph where source is already the destination and the disconnected
// chain where the whole source component has to be exhausted before the answer is
// no.
public sealed partial class FindIfPathExistsInGraphTests
{
    public static TheoryData<ReachabilityCase> Examples =>
        new()
        {
            { new ReachabilityCase(NodeCount: 3, Edges: [[0, 1], [1, 2], [2, 0]], Source: 0, Destination: 2, Expected: true) },
            { new ReachabilityCase(NodeCount: 6, Edges: [[0, 1], [0, 2], [3, 5], [5, 4], [4, 3]], Source: 0, Destination: 5, Expected: false) },
            { new ReachabilityCase(NodeCount: 1, Edges: [], Source: 0, Destination: 0, Expected: true) },
            { new ReachabilityCase(NodeCount: 4, Edges: [[0, 1], [1, 2], [2, 3]], Source: 0, Destination: 3, Expected: true) },
            { new ReachabilityCase(NodeCount: 5, Edges: [[0, 1], [2, 3], [3, 4]], Source: 1, Destination: 4, Expected: false) },
            { new ReachabilityCase(NodeCount: 5, Edges: [[0, 1], [2, 3], [3, 4]], Source: 4, Destination: 2, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPathByDepthFirstSearch_LeetCodeExamples_ReportsWhetherDestinationIsReachable(
        ReachabilityCase example)
    {
        var actual = FindIfPathExistsInGraphSolution.HasPathByDepthFirstSearch(
            example.NodeCount, example.Edges, example.Source, example.Destination);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasPathByDisjointSet_LeetCodeExamples_ReportsWhetherDestinationIsReachable(
        ReachabilityCase example)
    {
        var actual = FindIfPathExistsInGraphSolution.HasPathByDisjointSet(
            example.NodeCount, example.Edges, example.Source, example.Destination);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the graph, the two endpoints the question is asked about,
    // and whether a path joins them. Source and Destination are both ints and the
    // relation between them is not symmetric, so the row names which is which rather
    // than leaving two interchangeable positions. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct ReachabilityCase(
        int NodeCount, int[][] Edges, int Source, int Destination, bool Expected);
}
