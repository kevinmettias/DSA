using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MajorityElementBenchmarks (ARCHITECTURE 17.9): Setup seeds just over half the
// array with one value and fills the rest with noise disjoint from it, so the majority value is the
// fixture's own - a decisive literal rather than a derived one, and an expected value the generic
// agreement check, which only compares arms with each other, cannot supply.
public sealed partial class MajorityElementBenchmarksTests
{
    private const int SmallestLength = 200;

    // The seeded value Setup repeats (Length / 2) + 1 times, which is a strict majority of the
    // smallest [Params] length; the noise fills [] 1, 1000000) and so can never coincide with it.
    private const int ExpectedMajorityValue = -1;

    [Fact]
    public void BoyerMooreVoting_SeededMajorityOverDisjointNoise_ReturnsTheSeededMajorityValue() =>
        Assert.Equal(ExpectedMajorityValue, BuildHarness().BoyerMooreVoting());

    private static MajorityElementBenchmarks BuildHarness()
    {
        var harness = new MajorityElementBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
