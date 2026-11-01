using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteTheMiddleNodeOfALinkedListBenchmarks (ARCHITECTURE 17.9): the generic arm
// check compares the arms only with each other, so what stays here is a length derived without either
// arm. LeetCode 2095 always removes exactly one node, so the reading's documented shape is a chain one
// node shorter than the one it was given.
public sealed partial class DeleteTheMiddleNodeOfALinkedListBenchmarksTests
{
    private const int SmallestLength = 500;

    private const int ExpectedRemainingNodeCount = SmallestLength - 1;

    [Fact]
    public void CountThenRebuild_FiveHundredSeededValues_LeavesOneNodeFewer() =>
        Assert.Equal(ExpectedRemainingNodeCount, Values(BuildHarness().CountThenRebuild()).Count);

    // The arm reports the surviving chain, so the test reads the values off it rather than counting nodes.
    private static List<int> Values(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
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
