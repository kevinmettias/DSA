using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SerializeAndDeserializeBSTBenchmarks (ARCHITECTURE 17.9): both arms round-trip
// the same tree through their own grammar, so a harness whose arms disagree is timing two different
// trees - or the same tree two different ways. Setup inserts the shuffled values 0..NodeCount-1, so
// the tree holds exactly NodeCount nodes and the rebuilt tree must hold that many again; the Setup
// test asserts that count against the size Setup was asked for, which catches a grammar that loses
// or duplicates a node without needing the arms to agree on it.
//
// Weak with respect to shape, and reported as such with this batch: an arm's only public answer is
// the rebuilt tree's node count, so agreement witnesses that both grammars preserved the node count,
// not that either rebuilt the same tree. Strengthening it would mean returning the tree itself,
// which is not this harness's call.
public sealed partial class SerializeAndDeserializeBSTBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().NullMarkerQueueRoundTrip(), BuildHarness().NullMarkerQueueRoundTrip());
        Assert.Equal(SmallestNodeCount, BuildHarness().NullMarkerQueueRoundTrip());
    }

    [Fact]
    public void NullMarkerQueueRoundTrip_ShuffledBst_AgreesWithPreOrderValueOnlyRoundTrip()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PreOrderValueOnlyRoundTrip(), harness.NullMarkerQueueRoundTrip());
    }

    [Fact]
    public void PreOrderValueOnlyRoundTrip_ShuffledBst_AgreesWithNullMarkerQueueRoundTrip()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NullMarkerQueueRoundTrip(), harness.PreOrderValueOnlyRoundTrip());
    }

    private static SerializeAndDeserializeBSTBenchmarks BuildHarness()
    {
        var harness = new SerializeAndDeserializeBSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
