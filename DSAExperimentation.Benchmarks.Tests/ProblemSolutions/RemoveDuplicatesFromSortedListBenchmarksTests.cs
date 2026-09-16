using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveDuplicatesFromSortedListBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveDuplicatesFromSortedListSolution's, competing strategies for the same question - a distinct
// filter against an in-place scan that splices nodes out - so a harness whose arms disagree returns a
// different chain. Each arm builds the list fresh inside the call, so the hoisted _values is never
// written through and one harness serves both arms in either order. Both arms return the head as
// object (the node type is internal, so a public [Benchmark] method cannot name it as a return type,
// CS0050), which AnswerText cannot render - a linked node is not an enumerable sequence - so each
// result is walked into its values first.
public sealed partial class RemoveDuplicatesFromSortedListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().DistinctFilter())),
            AnswerText.Of(ValuesOf(BuildHarness().DistinctFilter())));

    [Fact]
    public void DistinctFilter_AgreesWithInPlaceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.InPlaceScan())),
            AnswerText.Of(ValuesOf(harness.DistinctFilter())));
    }

    [Fact]
    public void InPlaceScan_AgreesWithDistinctFilter()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.DistinctFilter())),
            AnswerText.Of(ValuesOf(harness.InPlaceScan())));
    }

    private static RemoveDuplicatesFromSortedListBenchmarks BuildHarness()
    {
        var harness = new RemoveDuplicatesFromSortedListBenchmarks { Length = SmallestLength };
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
