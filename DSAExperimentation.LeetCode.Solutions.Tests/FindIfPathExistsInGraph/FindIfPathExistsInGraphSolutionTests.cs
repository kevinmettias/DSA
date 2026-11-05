using DSAExperimentation.LeetCode.Solutions.FindIfPathExistsInGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindIfPathExistsInGraph;

// Harness only. Both reachability strategies are FindIfPathExistsInGraphSolution's
// - this file just pins them to the degenerate
// single-node graph where source is already the destination and the disconnected
// chain where the whole source component has to be exhausted before the answer is
// no.
public sealed partial class FindIfPathExistsInGraphSolutionTests
{
    public static TheoryData<ReachabilityCase> Cases =>
        new()
        {
            { new ReachabilityCase(NodeCount: 4, Edges: [[0, 1], [1, 2], [2, 3]], Source: 0, Destination: 3, Expected: true) },
            { new ReachabilityCase(NodeCount: 5, Edges: [[0, 1], [2, 3], [3, 4]], Source: 1, Destination: 4, Expected: false) },
            { new ReachabilityCase(NodeCount: 5, Edges: [[0, 1], [2, 3], [3, 4]], Source: 4, Destination: 2, Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasPathByDepthFirstSearch_Cases_ReportsWhetherDestinationIsReachable(
        ReachabilityCase testCase)
    {
        var actual = FindIfPathExistsInGraphSolution.HasPathByDepthFirstSearch(
            testCase.NodeCount, testCase.Edges, testCase.Source, testCase.Destination);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasPathByDisjointSet_Cases_ReportsWhetherDestinationIsReachable(
        ReachabilityCase testCase)
    {
        var actual = FindIfPathExistsInGraphSolution.HasPathByDisjointSet(
            testCase.NodeCount, testCase.Edges, testCase.Source, testCase.Destination);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the graph, the two endpoints the question is asked about,
    // and whether a path joins them. Source and Destination are both ints and the
    // relation between them is not symmetric, so the row names which is which rather
    // than leaving two interchangeable positions. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct ReachabilityCase(
        int NodeCount, int[][] Edges, int Source, int Destination, bool Expected);
}
