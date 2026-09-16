using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumWidthRampBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the O(n^2) scan of every ordered pair against the O(n)
// monotonic candidate stack - so a harness whose arms disagree is timing two different problems.
// Both arms return the ramp width as an int, so they are compared directly, and both only read the
// value array, so one harness is safe to read twice in either order. Setup draws the values from one
// fixed seed, so the same Length must rebuild the same array and with it the same widest ramp.
public sealed partial class MaximumWidthRampBenchmarksTests
{
    private const int SmallestLength = 500;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().PairwiseScan(), BuildHarness().PairwiseScan());

    [Fact]
    public void PairwiseScan_SeededValues_AgreesWithCandidateStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CandidateStack(), harness.PairwiseScan());
    }

    [Fact]
    public void CandidateStack_SeededValues_AgreesWithPairwiseScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseScan(), harness.CandidateStack());
    }

    private static MaximumWidthRampBenchmarks BuildHarness()
    {
        var harness = new MaximumWidthRampBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
