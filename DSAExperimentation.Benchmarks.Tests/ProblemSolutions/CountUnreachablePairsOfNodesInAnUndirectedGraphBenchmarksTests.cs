using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountUnreachablePairsOfNodesInAnUndirectedGraphBenchmarks (ARCHITECTURE
// 17.9): BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts
// only an expected count derived from the workload's construction rather than from either arm.
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
    public void DepthFirstFloodFill_DisjointChains_CountsEveryPairSplitAcrossTwoChains() =>
        Assert.Equal(
            UnreachablePairsAcrossEqualChains(SmallestNodeCount, ChainNodeCount),
            BuildHarness().DepthFirstFloodFill());

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
