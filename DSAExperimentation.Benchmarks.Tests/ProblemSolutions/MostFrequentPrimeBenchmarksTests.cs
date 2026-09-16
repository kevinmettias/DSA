using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MostFrequentPrimeBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - trial division per candidate against a sieve built once from the grid's own digit
// range - so a harness whose arms disagree is timing two different problems. Both arms only read the digit
// grid built in [GlobalSetup], so one harness instance is safe to call twice in either order. Setup draws that
// grid from one fixed seed, so the same GridSize must rebuild the same grid; otherwise two published numbers
// were never comparable in the first place.
public sealed partial class MostFrequentPrimeBenchmarksTests
{
    private const int SmallestGridSize = 2;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameGrid() =>
        Assert.Equal(BuildHarness().TrialDivision(), BuildHarness().TrialDivision());

    [Fact]
    public void TrialDivision_SeededDigitGrid_AgreesWithSieve()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Sieve(), harness.TrialDivision());
    }

    [Fact]
    public void Sieve_SeededDigitGrid_AgreesWithTrialDivision()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrialDivision(), harness.Sieve());
    }

    private static MostFrequentPrimeBenchmarks BuildHarness()
    {
        var harness = new MostFrequentPrimeBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
