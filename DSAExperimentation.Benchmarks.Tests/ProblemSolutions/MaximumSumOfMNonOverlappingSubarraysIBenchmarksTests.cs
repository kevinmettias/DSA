using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumSumOfMNonOverlappingSubarraysIBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the length-window DP against the
// sliding-window-maximum sweep - so a harness whose arms disagree is timing two different problems.
// Setup draws the values from one fixed seed and derives the subarray budget and the window bounds
// from the length, so the same Length must rebuild the same workload; otherwise two published numbers
// were never comparable in the first place.
public sealed partial class MaximumSumOfMNonOverlappingSubarraysIBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DynamicProgramming(), BuildHarness().DynamicProgramming());

    [Fact]
    public void DynamicProgramming_LengthDerivedSubarrayBudget_AgreesWithSlidingWindowMaximum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SlidingWindowMaximum(), harness.DynamicProgramming());
    }

    [Fact]
    public void SlidingWindowMaximum_LengthDerivedSubarrayBudget_AgreesWithDynamicProgramming()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DynamicProgramming(), harness.SlidingWindowMaximum());
    }

    private static MaximumSumOfMNonOverlappingSubarraysIBenchmarks BuildHarness()
    {
        var harness = new MaximumSumOfMNonOverlappingSubarraysIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
