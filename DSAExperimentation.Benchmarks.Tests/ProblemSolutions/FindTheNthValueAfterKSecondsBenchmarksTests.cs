using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheNthValueAfterKSecondsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - rolling the prefix sums k times against the closed-form
// modular binomial coefficient - so a harness whose arms disagree is timing two different problems.
// The class has no Setup, so Size (driving both n and k) is the whole workload.
public sealed partial class FindTheNthValueAfterKSecondsBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void BruteForce_SmallestSize_AgreesWithModularBinomial()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ModularBinomial(), harness.BruteForce());
    }

    [Fact]
    public void ModularBinomial_SmallestSize_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ModularBinomial());
    }

    private static FindTheNthValueAfterKSecondsBenchmarks BuildHarness() =>
        new() { Size = SmallestSize };
}
