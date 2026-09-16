using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindNthSmallestIntegerWithKOneBitsBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - walking the positive integers counting those with
// K one bits against decoding the position as a combination's rank - so a harness whose arms
// disagree has picked the nth integer out of two different sequences. Both answers are one long, so
// they are compared directly.
//
// The benchmark has no [Params] beyond Position and no [GlobalSetup]: Position is the whole tuned
// input and is set on the bare initializer, so there is nothing to set up before calling an arm.
public sealed partial class FindNthSmallestIntegerWithKOneBitsBenchmarksTests
{
    // The smaller of the benchmark's [Params(200, 2000)] positions.
    private const long SmallestPosition = 200;

    [Fact]
    public void PopCountScan_TwoHundredthIntegerWithFourOneBits_AgreesWithMemoizedBinomialSelection()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedBinomialSelection(), harness.PopCountScan());
    }

    [Fact]
    public void MemoizedBinomialSelection_TwoHundredthIntegerWithFourOneBits_AgreesWithPopCountScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PopCountScan(), harness.MemoizedBinomialSelection());
    }

    private static FindNthSmallestIntegerWithKOneBitsBenchmarks BuildHarness() =>
        new() { Position = SmallestPosition };
}
