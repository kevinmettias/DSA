using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveDuplicatesFromSortedListIIBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveDuplicatesFromSortedListIISolution's, competing strategies for the same question - an array
// group filter against a two-pointer scan that splices nodes out - so a harness whose arms disagree
// returns a different chain. Each arm builds the list fresh inside the call, so the hoisted _values is
// never written through and one harness serves both arms in either order. Both arms return the head
// as object (the node type is internal, so a public [Benchmark] method cannot name it as a return
// type, CS0050), which AnswerText cannot render - a linked node is not an enumerable sequence - so
// each result is walked into its values first.
public sealed partial class RemoveDuplicatesFromSortedListIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ArrayGroupFilter())),
            AnswerText.Of(ValuesOf(BuildHarness().ArrayGroupFilter())));

    [Fact]
    public void ArrayGroupFilter_AgreesWithTwoPointerScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.TwoPointerScan())),
            AnswerText.Of(ValuesOf(harness.ArrayGroupFilter())));
    }

    [Fact]
    public void TwoPointerScan_AgreesWithArrayGroupFilter()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.ArrayGroupFilter())),
            AnswerText.Of(ValuesOf(harness.TwoPointerScan())));
    }

    private static RemoveDuplicatesFromSortedListIIBenchmarks BuildHarness()
    {
        var harness = new RemoveDuplicatesFromSortedListIIBenchmarks { Length = SmallestLength };
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
