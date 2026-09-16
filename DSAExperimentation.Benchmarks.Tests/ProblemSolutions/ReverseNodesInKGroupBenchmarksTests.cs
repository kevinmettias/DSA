using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseNodesInKGroupBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - reversing each complete run with Array.Reverse over a
// materialized array against splicing the same runs on the list's own pointers - so a harness whose
// arms disagree is timing two different problems. [GlobalSetup] builds the values 1..Length in
// order, so the same Length must rebuild the same chain; otherwise two published numbers were never
// comparable in the first place.
//
// The group size is fixed at four and Length is a multiple of it, so every group is complete and no
// truncated tail is left over - the answer is the same ascending range with each run of four
// rendered backwards, which is stated here from the fixture's own shape rather than read back out of
// an arm. Both arms return the new head as object? (the node type is internal, CS0050) and rebuild
// the chain inside the measured call, so one harness is safe to call twice in either order.
public sealed partial class ReverseNodesInKGroupBenchmarksTests
{
    private const int SmallestLength = 200;

    // Both arms take the group size from the class's own constant.
    private const int GroupSize = 4;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ArrayGroupReverse())),
            AnswerText.Of(ValuesOf(BuildHarness().ArrayGroupReverse())));

    [Fact]
    public void ArrayGroupReverse_FullGroupsOfFour_AgreesWithTheLinkedListPointerReversal()
    {
        var harness = BuildHarness();
        var reversed = harness.ArrayGroupReverse();

        Assert.Equal(ExpectedGroupsReversed(SmallestLength), ValuesOf(reversed));
        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.LinkedListGroupReverse())),
            AnswerText.Of(ValuesOf(reversed)));
    }

    [Fact]
    public void LinkedListGroupReverse_FullGroupsOfFour_AgreesWithTheArrayReverse()
    {
        var harness = BuildHarness();
        var reversed = harness.LinkedListGroupReverse();

        Assert.Equal(ExpectedGroupsReversed(SmallestLength), ValuesOf(reversed));
        Assert.Equal(
            AnswerText.Of(ValuesOf(harness.ArrayGroupReverse())),
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

    // Length is an exact multiple of the group size, so every run is complete.
    private static int[] ExpectedGroupsReversed(int length)
    {
        var values = Enumerable.Range(1, length).ToArray();

        for (var start = 0; start < values.Length; start += GroupSize)
        {
            Array.Reverse(values, start, GroupSize);
        }

        return values;
    }

    private static ReverseNodesInKGroupBenchmarks BuildHarness()
    {
        var harness = new ReverseNodesInKGroupBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
