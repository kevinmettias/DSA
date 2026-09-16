using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SerializeAndDeserializeBinaryTreeBenchmarks (ARCHITECTURE 17.9): both arms
// round-trip the same tree through their own grammar, so a harness whose arms disagree is timing two
// different round trips. Setup builds the seeded BinaryTrees.Balanced tree, which is documented to
// hold exactly NodeCount nodes in heap layout, so the rebuilt tree must hold that many again; the
// Setup test asserts that count against the size Setup was asked for rather than only against the
// other arm, so it catches a grammar that loses or duplicates a node.
//
// Weak with respect to shape, and reported as such with this batch: an arm's only public answer is
// the rebuilt tree's node count, so agreement witnesses that both grammars preserved the node count,
// not that either rebuilt the same tree. Strengthening it would mean returning the tree itself,
// which is not this harness's call.
public sealed partial class SerializeAndDeserializeBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().QueueRoundTrip(), BuildHarness().QueueRoundTrip());
        Assert.Equal(SmallestNodeCount, BuildHarness().QueueRoundTrip());
    }

    [Fact]
    public void QueueRoundTrip_BalancedHeapLayoutTree_AgreesWithStringConcatRoundTrip()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StringConcatRoundTrip(), harness.QueueRoundTrip());
    }

    [Fact]
    public void StringConcatRoundTrip_BalancedHeapLayoutTree_AgreesWithQueueRoundTrip()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QueueRoundTrip(), harness.StringConcatRoundTrip());
    }

    private static SerializeAndDeserializeBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new SerializeAndDeserializeBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
