using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SoupServingsBenchmarks (ARCHITECTURE 17.9): both arms are
// SoupServingsSolution's searches of the same game tree for the same Milliliters, so a harness
// whose arms disagree is timing two different problems. There is no [GlobalSetup] to rebuild -
// the only preparation the harness ever did was the one integer division that is part of the
// LeetCode-shaped call itself - so the workload is the Milliliters the arms are handed.
//
// Both arms return a probability, so the comparison is a floating-point one against a named
// relative tolerance rather than an exact equality.
public sealed partial class SoupServingsBenchmarksTests
{
    private const double RelativeTolerance = 1e-9;

    private const int SmallestMilliliters = 600;

    [Fact]
    public void UnmemoizedRecursion_SixHundredMilliliters_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion(), RelativeTolerance);
    }

    [Fact]
    public void MemoizedRecursion_SixHundredMilliliters_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion(), RelativeTolerance);
    }

    private static SoupServingsBenchmarks BuildHarness() =>
        new() { Milliliters = SmallestMilliliters };
}
