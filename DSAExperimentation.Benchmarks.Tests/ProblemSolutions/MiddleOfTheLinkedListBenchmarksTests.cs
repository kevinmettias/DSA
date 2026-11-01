using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MiddleOfTheLinkedListBenchmarks (ARCHITECTURE 17.9): the baseline reports the
// middle node, and the reading's documented shape pins the tail that starts there: LeetCode 876
// returns the second middle node when the length is even, so a chain of SmallestLength nodes (even)
// leaves a tail of exactly SmallestLength / 2 nodes from the answer - an expected value derived from
// the workload's construction rather than from either arm, which the generic agreement check cannot
// supply.
public sealed partial class MiddleOfTheLinkedListBenchmarksTests
{
    private const int SmallestLength = 10;

    // Even length, so the middle is the second of the two: exactly half the chain remains from it.
    private const int ExpectedTailNodeCount = SmallestLength / 2;

    [Fact]
    public void CountThenWalk_SeededChain_LandsOnTheSecondMiddleNode() =>
        Assert.Equal(ExpectedTailNodeCount, Values(BuildHarness().CountThenWalk()).Count);

    // The arm reports the middle node, so the test reads the values off the tail that starts there
    // rather than comparing nodes.
    private static List<int> Values(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
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
