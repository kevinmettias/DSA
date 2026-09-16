using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindIfPathExistsInGraphBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - an iterative BCL depth-first reachability search
// against this repo's own DisjointSet - so a harness whose arms disagree is answering two different
// connectivity questions. Both answers are one bool, so they are compared directly.
//
// Setup builds two separate spanning trees, [0, half) and [half, NodeCount), and puts the source at
// node 0 and the destination at node NodeCount - 1 - one in each component. No path can therefore
// exist on any run, which makes the answer decisive instead of seed-dependent, and it is also the
// search arm's worst case: it must exhaust the whole source component before it can say no.
public sealed partial class FindIfPathExistsInGraphBenchmarksTests
{
    // The smaller of Setup's [Params(300, 3_000)] node counts.
    private const int SmallestNodeCount = 300;

    // Setup's own guarantee: source and destination sit in two disjoint spanning trees.
    private const bool ExpectedPathExists = false;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameVerdict() =>
        Assert.Equal(
            BuildHarness().HasPathByDepthFirstSearch(),
            BuildHarness().HasPathByDepthFirstSearch());

    [Fact]
    public void HasPathByDepthFirstSearch_DisjointComponents_AgreesWithHasPathByDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPathExists, harness.HasPathByDepthFirstSearch());
        Assert.Equal(harness.HasPathByDisjointSet(), harness.HasPathByDepthFirstSearch());
    }

    [Fact]
    public void HasPathByDisjointSet_DisjointComponents_AgreesWithHasPathByDepthFirstSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPathExists, harness.HasPathByDisjointSet());
        Assert.Equal(harness.HasPathByDepthFirstSearch(), harness.HasPathByDisjointSet());
    }

    private static FindIfPathExistsInGraphBenchmarks BuildHarness()
    {
        var harness = new FindIfPathExistsInGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
