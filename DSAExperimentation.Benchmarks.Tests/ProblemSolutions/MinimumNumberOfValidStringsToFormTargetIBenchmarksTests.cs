using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfValidStringsToFormTargetIBenchmarks (ARCHITECTURE 17.9):
// both arms are MinimumNumberOfValidStringsToFormTargetISolution's, the same methods
// MinimumNumberOfValidStringsToFormTargetITests proves correct, and both return the fewest
// words that build the target, or the shared "impossible" marker when no cover exists. The
// nested character comparison and the Z-function sweep are competing strategies for that one
// number, so arms that disagree are timing two different problems.
public sealed partial class MinimumNumberOfValidStringsToFormTargetIBenchmarksTests
{
    // The smallest declared [Params] value: the brute-force arm costs one nested comparison per
    // target position, and the word list is fixed at this tier's own size.
    private const int SmallestTargetLength = 500;

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

    private static MinimumNumberOfValidStringsToFormTargetIBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfValidStringsToFormTargetIBenchmarks { TargetLength = SmallestTargetLength };
        harness.Setup();

        return harness;
    }
}
