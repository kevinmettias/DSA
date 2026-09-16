using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SplitArrayLargestSumBenchmarks (ARCHITECTURE 17.9): both arms are
// SplitArrayLargestSumSolution's searches for the same minimized largest subarray sum over the
// same values and the same subarray count - a hand-rolled binary search against the repo's own
// BinarySearch lower bound - so a harness whose arms disagree is timing two different problems.
// Setup is a pure function of Length, so the same Length must rebuild the same values and the
// same subarray count.
//
// Both arms return the minimized largest sum itself, which is the answer LeetCode 410 asks for,
// so agreement compares the whole result rather than a proxy.
public sealed partial class SplitArrayLargestSumBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ManualBinarySearch(), BuildHarness().ManualBinarySearch());

    [Fact]
    public void ManualBinarySearch_TwoHundredValues_AgreesWithSequenceLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SequenceLowerBound(), harness.ManualBinarySearch());
    }

    [Fact]
    public void SequenceLowerBound_TwoHundredValues_AgreesWithManualBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualBinarySearch(), harness.SequenceLowerBound());
    }

    private static SplitArrayLargestSumBenchmarks BuildHarness()
    {
        var harness = new SplitArrayLargestSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
