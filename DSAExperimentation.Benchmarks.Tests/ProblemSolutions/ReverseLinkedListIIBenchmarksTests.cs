using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseLinkedListIIBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the baseline's answer to one derived from the workload's shape.
// [GlobalSetup] fills the chain with 1..Length in ascending order, which makes the reversal's
// boundary visible on the answer itself: only the values between the two positions swap, so the
// expected sequence is the head prefix, then the sub-range backwards, then the untouched tail.
public sealed partial class ReverseLinkedListIIBenchmarksTests
{
    private const int SmallestLength = 200;

    // The benchmark takes the quarter and three-quarter positions of the chain, 1-indexed.
    private const int LengthQuarters = 4;
    private const int ThreeQuarterNumerator = 3;

    [Fact]
    public void ArrayRebuild_QuarterToThreeQuarterRange_ReversesOnlyTheSubRange() =>
        Assert.Equal(ExpectedSubRangeReversed(SmallestLength), ValuesOf(BuildHarness().ArrayRebuild()));

    private static int[] ValuesOf(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
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
