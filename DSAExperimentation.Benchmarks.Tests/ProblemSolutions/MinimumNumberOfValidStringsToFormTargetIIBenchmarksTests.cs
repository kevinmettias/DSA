using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfValidStringsToFormTargetIIBenchmarks (ARCHITECTURE 17.9):
// both arms are MinimumNumberOfValidStringsToFormTargetIISolution's, the same methods
// MinimumNumberOfValidStringsToFormTargetIITests proves correct, and both return the fewest
// words that build the target, or the shared "impossible" marker when no cover exists. Only the
// matching strategy differs - nested comparison against a Z-function sweep - so arms that
// disagree are timing two different problems.
public sealed partial class MinimumNumberOfValidStringsToFormTargetIIBenchmarksTests
{
    // The smallest declared [Params] value: it keeps the brute-force arm inside this problem's
    // own bound while still giving the Z-function sweep a long target to match across.
    private const int SmallestTargetLength = 2_000;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().BruteForce(),
            BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AgreesWithZFunctionAcrossWords()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ZFunctionAcrossWords(), harness.BruteForce());
    }

    [Fact]
    public void ZFunctionAcrossWords_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ZFunctionAcrossWords());
    }

    private static MinimumNumberOfValidStringsToFormTargetIIBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfValidStringsToFormTargetIIBenchmarks { TargetLength = SmallestTargetLength };
        harness.Setup();

        return harness;
    }
}
