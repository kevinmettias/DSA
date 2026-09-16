using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountUnreachablePairsOfNodesInAnUndirectedGraphBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - a DFS flood fill against
// this repo's own DisjointSet plus a HashMap size tally - so a harness whose arms disagree is timing
// two different problems, not two ways of answering one. Setup derives the edge array from NodeCount
// alone, so the same NodeCount must rebuild the same graph; otherwise two published numbers were
// never comparable in the first place.
//
// The edge array is private, and the workload's defining property - NodeCount / 25 disjoint chains,
// every one of them a real component to discover - decides the answer outright: every pair inside
// one chain is reachable and every pair split across two chains is not.
public sealed partial class CountUnreachablePairsOfNodesInAnUndirectedGraphBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    private const int ChainNodeCount = 25;

    // C(size, 2) - one unordered pair per two distinct nodes.
    private const int UnorderedPairDivisor = 2;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameChains()
    {
        Assert.Equal(
            UnreachablePairsAcrossEqualChains(SmallestNodeCount, ChainNodeCount),
            BuildHarness().DepthFirstFloodFill());
        Assert.Equal(BuildHarness().DepthFirstFloodFill(), BuildHarness().DepthFirstFloodFill());
    }

    [Fact]
    public void DepthFirstFloodFill_DisjointChains_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetUnionFind(), harness.DepthFirstFloodFill());
    }

    [Fact]
    public void DisjointSetUnionFind_DisjointChains_AgreesWithDepthFirstFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstFloodFill(), harness.DisjointSetUnionFind());
    }

    private static CountUnreachablePairsOfNodesInAnUndirectedGraphBenchmarks BuildHarness()
    {
        var harness = new CountUnreachablePairsOfNodesInAnUndirectedGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // C(nodeCount, 2) minus C(chainNodeCount, 2) per chain, which is the reachable pairs.
    private static long UnreachablePairsAcrossEqualChains(int nodeCount, int chainNodeCount) =>
        PairsWithin(nodeCount) - nodeCount / chainNodeCount * PairsWithin(chainNodeCount);

    private static long PairsWithin(long size) => size * (size - 1) / UnorderedPairDivisor;
}
