using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SerializeAndDeserializeBinaryTreeBenchmarks (ARCHITECTURE 17.9): both arms
// round-trip the same tree through their own grammar and return the rebuilt tree, so a harness whose
// arms disagree is timing two different round trips. Setup builds the complete tree of a gapless
// level-order array drawn from seeded Random(297) across [-1000, 1000], and a round trip is only
// correct when it rebuilds that tree exactly - so each arm's tree is asserted against the complete
// tree of the same draw, rebuilt here, which catches a grammar that loses, duplicates or moves a
// node without needing the arms to agree on it.
public sealed partial class SerializeAndDeserializeBinaryTreeBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    // Mirrors SerializeAndDeserializeBinaryTreeBenchmarks' own private RandomSeed, MinValue and MaxValue.
    private const int RandomSeed = 297;
    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(AnswerGraphText.Of(BuildHarness().QueueRoundTrip()), AnswerGraphText.Of(BuildHarness().QueueRoundTrip()));
        Assert.Equal(OriginalTree(), AnswerGraphText.Of(BuildHarness().QueueRoundTrip()));
    }

    [Fact]
    public void StringConcatRoundTrip_BalancedHeapLayoutTree_RebuildsTheOriginalTree() =>
        Assert.Equal(OriginalTree(), AnswerGraphText.Of(BuildHarness().StringConcatRoundTrip()));

    // The benchmark's own draw, rebuilt from its documented shape.
    private static string OriginalTree()
    {
        var levelOrder = SeededDraws.Values(SmallestNodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));

        return AnswerGraphText.Of(BinaryTrees.Complete(levelOrder));
    }

    private static SerializeAndDeserializeBinaryTreeBenchmarks BuildHarness()
    {
        var harness = new SerializeAndDeserializeBinaryTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
