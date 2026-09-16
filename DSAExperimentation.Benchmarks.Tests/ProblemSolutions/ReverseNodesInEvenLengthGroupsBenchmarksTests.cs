using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseNodesInEvenLengthGroupsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - materializing every value into an array, computing
// the increasing-then-truncated group boundaries there and reversing each even run with
// Array.Reverse, against pointer splicing with no array materialization at all - so a harness whose
// arms disagree is timing two different problems. [GlobalSetup] builds the values 1..Length in
// order, so the same Length must rebuild the same chain; otherwise two published numbers were never
// comparable in the first place.
//
// The boundary rule fixes the answer independently of either data structure: the groups are 1, 2, 3,
// ... nodes with the final one taking whatever is left, and it is the final group's REAL length -
// not the size it was reaching for - that decides whether it reverses. The chain is still in
// ascending order, so each reversed run is visible as a descending stretch of the same values.
//
// Both arms return the new head as object? (the node type is internal, CS0050) and rebuild the chain
// inside the measured call, so one harness is safe to call twice in either order.
public sealed partial class ReverseNodesInEvenLengthGroupsBenchmarksTests
{
    private const int SmallestLength = 200;

    private const int FirstGroupLength = 1;
    private const int SecondGroupSize = 2;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ArrayEvenLengthGroupReverse())),
            AnswerText.Of(ValuesOf(BuildHarness().ArrayEvenLengthGroupReverse())));

    [Fact]
    public void ArrayEvenLengthGroupReverse_GrowingGroups_AgreesWithTheLinkedListPointerReversal()
    {
        var harness = BuildHarness();
        var reversed = harness.ArrayEvenLengthGroupReverse();

        Assert.Equal(ExpectedEvenLengthRunsReversed(SmallestLength), ValuesOf(reversed));
        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.LinkedListEvenLengthGroupReverse())),
            AnswerText.Of(ValuesOf(reversed)));
    }

    [Fact]
    public void LinkedListEvenLengthGroupReverse_GrowingGroups_AgreesWithTheArrayRebuild()
    {
        var harness = BuildHarness();
        var reversed = harness.LinkedListEvenLengthGroupReverse();

        Assert.Equal(ExpectedEvenLengthRunsReversed(SmallestLength), ValuesOf(reversed));
        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.ArrayEvenLengthGroupReverse())),
            AnswerText.Of(ValuesOf(reversed)));
    }

    private static int[] ValuesOf(object? head)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // The first group is a single node (odd, so it never reverses) and the size grows by one from
    // the second group on, with the final group taking only the nodes that remain.
    private static int[] ExpectedEvenLengthRunsReversed(int length)
    {
        var values = Enumerable.Range(1, length).ToArray();
        var start = FirstGroupLength;
        var groupSize = SecondGroupSize;

        while (start < values.Length)
        {
            var actualLength = Math.Min(groupSize, values.Length - start);

            if (actualLength % SecondGroupSize == 0)
            {
                Array.Reverse(values, start, actualLength);
            }

            start += actualLength;
            groupSize++;
        }

        return values;
    }

    private static ReverseNodesInEvenLengthGroupsBenchmarks BuildHarness()
    {
        var harness = new ReverseNodesInEvenLengthGroupsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
