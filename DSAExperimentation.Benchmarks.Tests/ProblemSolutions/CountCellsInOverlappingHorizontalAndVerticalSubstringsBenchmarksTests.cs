using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountCellsInOverlappingHorizontalAndVerticalSubstringsBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - the
// sliding-window comparison in each flattened direction against this repo's ZFunction.FindAll over
// the same two flattenings - so a harness whose arms disagree is timing two different problems. Both
// arms return an int, so they are compared directly. Setup builds the grid and the pattern from one
// fixed seed, so the same GridSize must rebuild the same pair, asserted through the one observable
// the arms expose.
public sealed partial class CountCellsInOverlappingHorizontalAndVerticalSubstringsBenchmarksTests
{
    private const int SmallestGridSize = 60;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameGridAndPattern() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_TwoLetterGrid_AgreesWithZFunction()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ZFunction(), harness.BruteForce());
    }

    [Fact]
    public void ZFunction_TwoLetterGrid_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ZFunction());
    }

    private static CountCellsInOverlappingHorizontalAndVerticalSubstringsBenchmarks BuildHarness()
    {
        var harness = new CountCellsInOverlappingHorizontalAndVerticalSubstringsBenchmarks
        {
            GridSize = SmallestGridSize,
        };
        harness.Setup();

        return harness;
    }
}
