using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountValidPathsInATreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - one BFS per unordered node pair against this repo's
// own DisjointSet split into blobs - so a harness whose arms disagree is timing two different
// problems, not two ways of answering one. Setup draws its tree from one fixed seed, so the same
// NodeCount must rebuild the same tree; otherwise two published numbers were never comparable in the
// first place.
//
// The edge array is private and the path count is the only thing either arm reports, so the
// documented shape is asserted through that: a valid path is one unordered pair among the tree's
// NodeCount nodes, so no count can exceed C(NodeCount, 2), and the NodeCount - 1 edges of a tree on
// that many nodes are what make the whole pair set reachable at all.
public sealed partial class CountValidPathsInATreeBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    private const long NodePairCount = (long)SmallestNodeCount * (SmallestNodeCount - 1) / 2;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTree()
    {
        Assert.InRange(BuildHarness().PerPairPathWalk(), 0, NodePairCount);
        Assert.Equal(BuildHarness().PerPairPathWalk(), BuildHarness().PerPairPathWalk());
    }

    [Fact]
    public void PerPairPathWalk_SeededRecursiveTree_AgreesWithDisjointSetBlobs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetBlobs(), harness.PerPairPathWalk());
    }

    [Fact]
    public void DisjointSetBlobs_SeededRecursiveTree_AgreesWithPerPairPathWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PerPairPathWalk(), harness.DisjointSetBlobs());
    }

    private static CountValidPathsInATreeBenchmarks BuildHarness()
    {
        var harness = new CountValidPathsInATreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
