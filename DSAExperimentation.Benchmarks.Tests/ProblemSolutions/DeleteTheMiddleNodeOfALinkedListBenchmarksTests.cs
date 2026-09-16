using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteTheMiddleNodeOfALinkedListBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - counting the list and copying every node but the middle
// against splicing the middle out in one slow/fast traversal - so a harness whose arms disagree is timing
// two different problems. Setup draws the values from one fixed seed, and deleting is destructive, so each
// arm is handed a list rebuilt from those same values. LeetCode 2095 always removes exactly one node, so
// the reading's documented shape is a chain one node shorter than the one it was given; the same Length
// must rebuild the same values and with them the same survivors.
public sealed partial class DeleteTheMiddleNodeOfALinkedListBenchmarksTests
{
    private const int SmallestLength = 500;

    private const int ExpectedRemainingNodeCount = SmallestLength - 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedRemainingNodeCount, Values(BuildHarness().CountThenRebuild()).Count);
        Assert.Equal(
            AnswerText.Of(Values(BuildHarness().CountThenRebuild())),
            AnswerText.Of(Values(BuildHarness().CountThenRebuild())));
    }

    [Fact]
    public void CountThenRebuild_FiveHundredSeededValues_AgreesWithSlowFastPointers()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedRemainingNodeCount, Values(harness.CountThenRebuild()).Count);
        Assert.Equal(
            AnswerText.Of(Values(harness.SlowFastPointers())),
            AnswerText.Of(Values(harness.CountThenRebuild())));
    }

    [Fact]
    public void SlowFastPointers_FiveHundredSeededValues_AgreesWithCountThenRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedRemainingNodeCount, Values(harness.SlowFastPointers()).Count);
        Assert.Equal(
            AnswerText.Of(Values(harness.CountThenRebuild())),
            AnswerText.Of(Values(harness.SlowFastPointers())));
    }

    // Both arms report the surviving chain as that internal node type through an object, so the test
    // reads the values off it rather than comparing nodes.
    private static List<int> Values(object? head)
    {
        var values = new List<int>();

        for (var node = head as SinglyLinkedListNode<int>; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static DeleteTheMiddleNodeOfALinkedListBenchmarks BuildHarness()
    {
        var harness = new DeleteTheMiddleNodeOfALinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
