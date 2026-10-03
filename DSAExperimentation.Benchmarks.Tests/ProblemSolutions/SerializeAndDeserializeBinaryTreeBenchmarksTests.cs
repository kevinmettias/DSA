using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SerializeAndDeserializeBinaryTreeBenchmarks (ARCHITECTURE 17.9): both arms
// round-trip the same tree through their own grammar and return the rebuilt tree, so a harness whose
// arms disagree is timing two different round trips. Setup builds the BinaryTrees.Balanced tree,
// which is documented to hold exactly NodeCount nodes in heap layout, and a round trip is only
// correct when it rebuilds that tree exactly - so each arm's tree is asserted against a fresh
// BinaryTrees.Balanced of the same size, which catches a grammar that loses, duplicates or moves a
// node without needing the arms to agree on it.
public sealed partial class SerializeAndDeserializeBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(AnswerGraphText.Of(BuildHarness().QueueRoundTrip()), AnswerGraphText.Of(BuildHarness().QueueRoundTrip()));
        Assert.Equal(OriginalTree(), AnswerGraphText.Of(BuildHarness().QueueRoundTrip()));
    }

    [Fact]
    public void StringConcatRoundTrip_BalancedHeapLayoutTree_RebuildsTheOriginalTree() =>
        Assert.Equal(OriginalTree(), AnswerGraphText.Of(BuildHarness().StringConcatRoundTrip()));

    private static string OriginalTree() => AnswerGraphText.Of(BinaryTrees.Balanced(SmallestNodeCount));

    private static SerializeAndDeserializeBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new SerializeAndDeserializeBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
