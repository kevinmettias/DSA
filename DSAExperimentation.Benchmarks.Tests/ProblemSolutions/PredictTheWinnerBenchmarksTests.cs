using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PredictTheWinnerBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - can the first player force a win or a tie on this array? - so a
// harness whose arms disagree is timing two different problems. ArrayLength is the only [Params] axis
// and Setup derives the scores from it, so the same ArrayLength must rebuild the same array.
public sealed partial class PredictTheWinnerBenchmarksTests
{
    private const int SmallestArrayLength = 22;

    [Fact]
    public void Setup_SameArrayLength_RebuildsTheSameScores() =>
        Assert.Equal(
            BuildHarness().CanWinByMemoizedRecursion(),
            BuildHarness().CanWinByMemoizedRecursion());

    [Fact]
    public void CanWinByUnmemoizedRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanWinByMemoizedRecursion(), harness.CanWinByUnmemoizedRecursion());
    }

    [Fact]
    public void CanWinByMemoizedRecursion_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanWinByUnmemoizedRecursion(), harness.CanWinByMemoizedRecursion());
    }

    private static PredictTheWinnerBenchmarks BuildHarness()
    {
        var harness = new PredictTheWinnerBenchmarks { ArrayLength = SmallestArrayLength };
        harness.Setup();

        return harness;
    }
}
