using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MiddleOfTheLinkedListBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - counting the list and then walking Length / 2 steps from the head
// against the one-pass slow/fast two-pointer walk - so a harness whose arms disagree is timing two
// different problems. Both arms report the middle node as an object over the data structure's internal
// node type, so the test reads the values off the tail that starts there rather than comparing nodes.
//
// The reading's documented shape pins the tail's length: LeetCode 876 returns the second middle node
// when the length is even, so a chain of SmallestLength nodes (even) leaves a tail of exactly
// SmallestLength / 2 nodes from the answer. Both arms only read the chain, so the shared prepared list
// makes one harness safe to read twice in either order, and Setup draws from one fixed seed, so the same
// Length must rebuild the same chain.
public sealed partial class MiddleOfTheLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    // Even length, so the middle is the second of the two: exactly half the chain remains from it.
    private const int ExpectedTailNodeCount = SmallestLength / 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(Values(BuildHarness().CountThenWalk())),
            AnswerText.Of(Values(BuildHarness().CountThenWalk())));

    [Fact]
    public void CountThenWalk_SeededChain_AgreesWithSlowFastTwoPointer()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTailNodeCount, Values(harness.CountThenWalk()).Count);
        Assert.Equal(
            AnswerText.Of(Values(harness.SlowFastTwoPointer())),
            AnswerText.Of(Values(harness.CountThenWalk())));
    }

    [Fact]
    public void SlowFastTwoPointer_SeededChain_AgreesWithCountThenWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTailNodeCount, Values(harness.SlowFastTwoPointer()).Count);
        Assert.Equal(
            AnswerText.Of(Values(harness.CountThenWalk())),
            AnswerText.Of(Values(harness.SlowFastTwoPointer())));
    }

    // Both arms report the middle node as that internal node type through an object, so the test reads
    // the values off it rather than comparing nodes - and reading the whole tail, not just the value,
    // also pins that both walks landed on the same position.
    private static List<int> Values(object? head)
    {
        var values = new List<int>();

        for (var node = head as SinglyLinkedListNode<int>; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static MiddleOfTheLinkedListBenchmarks BuildHarness()
    {
        var harness = new MiddleOfTheLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
