using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountingBitsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - re-counting each number's set bits against reusing the value
// halved - so a harness whose arms disagree is timing two different problems, not two ways of
// answering one. There is no [GlobalSetup] here: the maximum value is a scalar [Params] property with
// no input container to prepare ahead of the measured call, so the same parameters rebuild nothing
// and the arms are compared directly.
public sealed partial class CountingBitsBenchmarksTests
{
    private const int SmallestMaximumValue = 2_000;

    [Fact]
    public void PerNumberLoop_CountsZeroThroughTwoThousand_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.MemoizedRecurrence()), AnswerText.Of(harness.PerNumberLoop()));
    }

    [Fact]
    public void MemoizedRecurrence_CountsZeroThroughTwoThousand_AgreesWithPerNumberLoop()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.PerNumberLoop()), AnswerText.Of(harness.MemoizedRecurrence()));
    }

    // AnswerText.Of rather than OfUnorderedSet: both arms report the popcount of value i at index i,
    // so a value's position is the number it counts, not an arbitrary outer order.
    private static CountingBitsBenchmarks BuildHarness() =>
        new() { MaximumValue = SmallestMaximumValue };
}
