using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for one question - checking every
// subarray's two ends against its whole span against a monotonic-stack count of the spans each
// element can dominate - so a harness whose arms disagree is timing two different problems. Setup
// draws the array from one fixed seed, so the same Length must rebuild the same values.
public sealed partial class FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStack(), harness.BruteForce());
    }

    [Fact]
    public void MonotonicStack_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MonotonicStack());
    }

    private static FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumBenchmarks BuildHarness()
    {
        var harness = new FindTheNumberOfSubarraysWhereBoundaryElementsAreMaximumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
