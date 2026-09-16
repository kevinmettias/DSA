using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DivisorGameBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the memoized game-theory recursion scanning every divisor of
// the position against the closed form it provably reduces to - so a harness whose arms disagree is
// timing two different problems. The verdict is a bare bool, so the closed form's own statement is
// what keeps agreement from being vacuous: Alice wins exactly when the starting position is even,
// and the benchmark's smallest parameter (100) is even, so both arms must answer true.
public sealed partial class DivisorGameBenchmarksTests
{
    private const int SmallestStartingPosition = 100;
    private const bool ExpectedWinForAnEvenStartingPosition = true;

    [Fact]
    public void CanAliceWinByMemoizedRecursion_EvenStartingPosition_WinsAndAgreesWithParityFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedWinForAnEvenStartingPosition, harness.CanAliceWinByMemoizedRecursion());
        Assert.Equal(harness.CanAliceWinByParityFormula(), harness.CanAliceWinByMemoizedRecursion());
    }

    [Fact]
    public void CanAliceWinByParityFormula_EvenStartingPosition_WinsAndAgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedWinForAnEvenStartingPosition, harness.CanAliceWinByParityFormula());
        Assert.Equal(harness.CanAliceWinByMemoizedRecursion(), harness.CanAliceWinByParityFormula());
    }

    private static DivisorGameBenchmarks BuildHarness() => new() { StartingPosition = SmallestStartingPosition };
}
