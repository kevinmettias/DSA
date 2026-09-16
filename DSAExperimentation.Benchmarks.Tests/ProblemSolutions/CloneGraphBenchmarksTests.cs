using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CloneGraphBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - a DFS whose original-to-clone memo is a BCL Dictionary against the same DFS
// over the repo's own HashMap - so a harness whose arms disagree is timing two different problems.
// Both arms reduce the clone to the root node's value, because the graph's node type is internal to
// the solution tier and a public [Benchmark] cannot expose it, so an agreement between them is worth
// the least of any class in this batch: it says the two walks agreed on the root, not what they
// copied.
public sealed partial class CloneGraphBenchmarksTests
{
    private const int SmallestNodeCount = 10;

    // Setup numbers its ring's nodes 1..NodeCount in index order and returns nodes[0], so the graph's
    // root - the only node either arm's answer is read from - carries value 1. The graph itself is
    // private, so that value is also all a rebuild can be pinned to: a harness built from the same
    // NodeCount must clone a root numbered 1, and the ring it belongs to must be the same ring.
    private const int RingRootValue = 1;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheRingRootedAtValueOne()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(RingRootValue, first.DictionaryDfs());
        Assert.Equal(RingRootValue, second.HashMapDfs());
    }

    [Fact]
    public void DictionaryDfs_TenNodeRing_AgreesWithHashMapDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapDfs(), harness.DictionaryDfs());
    }

    [Fact]
    public void HashMapDfs_TenNodeRing_AgreesWithDictionaryDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryDfs(), harness.HashMapDfs());
    }

    private static CloneGraphBenchmarks BuildHarness()
    {
        var harness = new CloneGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
