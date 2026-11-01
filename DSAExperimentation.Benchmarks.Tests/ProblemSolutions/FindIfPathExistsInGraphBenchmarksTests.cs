using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindIfPathExistsInGraphBenchmarks (ARCHITECTURE 17.9): the generic arms check
// only compares the arms with each other, so this pins the baseline's verdict to the one Setup's
// construction fixes independently of any arm.
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
    public void HasPathByDepthFirstSearch_DisjointComponents_FindsNoPath() =>
        Assert.Equal(ExpectedPathExists, BuildHarness().HasPathByDepthFirstSearch());

    private static FindIfPathExistsInGraphBenchmarks BuildHarness()
    {
        var harness = new FindIfPathExistsInGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
