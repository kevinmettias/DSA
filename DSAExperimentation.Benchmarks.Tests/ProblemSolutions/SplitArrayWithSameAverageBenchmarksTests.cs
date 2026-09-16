using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SplitArrayWithSameAverageBenchmarks (ARCHITECTURE 17.9): both arms are
// SplitArrayWithSameAverageSolution's decision procedures over the same values - an all-subsets
// brute force against a subset-sum DP that also tracks the chosen count - so a harness whose arms
// disagree is timing two different problems. Setup is a pure function of Length and its own fixed
// seed, so the same Length must rebuild the same values.
//
// WEAK BY CONSTRUCTION, and reported as such: each arm answers one bool, so agreement witnesses
// that the two procedures reached the same verdict and nothing more - two arms that both answer
// "no" for the wrong reason are indistinguishable from two that both answer correctly. It does
// catch the failure that matters most here, an arm that disagrees with the other on the same
// input, which is what the exponential and the polynomial procedure existing side by side is
// meant to rule out.
public sealed partial class SplitArrayWithSameAverageBenchmarksTests
{
    private const int SmallestLength = 16;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CanSplitBySubsetMasks(),
            BuildHarness().CanSplitBySubsetMasks());

    [Fact]
    public void CanSplitBySubsetMasks_SixteenValues_AgreesWithCanSplitByMemoizedSubsetSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanSplitByMemoizedSubsetSum(), harness.CanSplitBySubsetMasks());
    }

    [Fact]
    public void CanSplitByMemoizedSubsetSum_SixteenValues_AgreesWithCanSplitBySubsetMasks()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanSplitBySubsetMasks(), harness.CanSplitByMemoizedSubsetSum());
    }

    private static SplitArrayWithSameAverageBenchmarks BuildHarness()
    {
        var harness = new SplitArrayWithSameAverageBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
