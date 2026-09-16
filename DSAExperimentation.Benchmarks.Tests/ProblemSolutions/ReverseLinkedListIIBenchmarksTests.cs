using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseLinkedListIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - materializing every value into an array and reversing the
// sub-range there against head-inserting the sub-range's nodes in place - so a harness whose arms
// disagree is timing two different problems. The two boundaries are derived from Length alone, so
// the same Length must rebuild the same workload; otherwise two published numbers were never
// comparable in the first place.
//
// [GlobalSetup] fills the chain with 1..Length in ascending order, which makes the reversal's
// boundary visible on the answer itself: only the values between the two positions swap, so the
// expected sequence is the head prefix, then the sub-range backwards, then the untouched tail.
//
// Each arm rebuilds or rewires the chain it is handed, and both build it from the hoisted values
// inside the measured call, so one harness is safe to call twice in either order.
public sealed partial class ReverseLinkedListIIBenchmarksTests
{
    private const int SmallestLength = 200;

    // The benchmark takes the quarter and three-quarter positions of the chain, 1-indexed.
    private const int LengthQuarters = 4;
    private const int ThreeQuarterNumerator = 3;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ArrayRebuild())),
            AnswerText.Of(ValuesOf(BuildHarness().ArrayRebuild())));

    [Fact]
    public void ArrayRebuild_QuarterToThreeQuarterRange_AgreesWithHeadInsertion()
    {
        var harness = BuildHarness();
        var reversed = harness.ArrayRebuild();

        Assert.Equal(ExpectedSubRangeReversed(SmallestLength), ValuesOf(reversed));
        Assert.Equal(AnswerText.Of(ValuesOf(harness.HeadInsertion())), AnswerText.Of(ValuesOf(reversed)));
    }

    [Fact]
    public void HeadInsertion_QuarterToThreeQuarterRange_AgreesWithArrayRebuild()
    {
        var harness = BuildHarness();
        var reversed = harness.HeadInsertion();

        Assert.Equal(ExpectedSubRangeReversed(SmallestLength), ValuesOf(reversed));
        Assert.Equal(AnswerText.Of(ValuesOf(harness.ArrayRebuild())), AnswerText.Of(ValuesOf(reversed)));
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

    // The head prefix 1..left-1, the [left, right] sub-range backwards, then the tail right+1..length.
    private static int[] ExpectedSubRangeReversed(int length)
    {
        var left = length / LengthQuarters;
        var right = length * ThreeQuarterNumerator / LengthQuarters;
        var values = new List<int>();

        for (var value = 1; value < left; value++)
        {
            values.Add(value);
        }

        for (var value = right; value >= left; value--)
        {
            values.Add(value);
        }

        for (var value = right + 1; value <= length; value++)
        {
            values.Add(value);
        }

        return [.. values];
    }

    private static ReverseLinkedListIIBenchmarks BuildHarness()
    {
        var harness = new ReverseLinkedListIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
