using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveZeroSumConsecutiveNodesFromLinkedListBenchmarks (ARCHITECTURE 17.9): both
// arms are RemoveZeroSumConsecutiveNodesFromLinkedListSolution's, competing strategies for the same
// question - a per-start rescan against one prefix-sum map pass - so a harness whose arms disagree
// returns a different chain. Each arm builds the list fresh inside the call, so the hoisted _values is
// never written through and one harness serves both arms in either order. Both arms return the head as
// object (the node type is internal, so a public [Benchmark] method cannot name it as a return type,
// CS0050), which AnswerText cannot render - a linked node is not an enumerable sequence - so each
// result is walked into its values first.
public sealed partial class RemoveZeroSumConsecutiveNodesFromLinkedListBenchmarksTests
{
    private const int SmallestLength = 300;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().NestedRescan())),
            AnswerText.Of(ValuesOf(BuildHarness().NestedRescan())));

    [Fact]
    public void NestedRescan_AgreesWithPrefixSumMap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.PrefixSumMap())),
            AnswerText.Of(ValuesOf(harness.NestedRescan())));
    }

    [Fact]
    public void PrefixSumMap_AgreesWithNestedRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.NestedRescan())),
            AnswerText.Of(ValuesOf(harness.PrefixSumMap())));
    }

    private static RemoveZeroSumConsecutiveNodesFromLinkedListBenchmarks BuildHarness()
    {
        var harness = new RemoveZeroSumConsecutiveNodesFromLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static int[] ValuesOf(object? answer)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)answer; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }
}
