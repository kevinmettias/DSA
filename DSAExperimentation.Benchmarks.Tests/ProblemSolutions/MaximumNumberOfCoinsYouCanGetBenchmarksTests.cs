using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfCoinsYouCanGetBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumNumberOfCoinsYouCanGetSolution's competing strategies for one question - the quadratic
// round-by-round simulation against one merge sort plus the picking arithmetic - so a harness whose
// arms disagree is timing two different problems. Both answer with a single coin total, compared
// directly.
public sealed partial class MaximumNumberOfCoinsYouCanGetBenchmarksTests
{
    private const int SmallestPileCount = 300;

    [Fact]
    public void Setup_SamePileCount_RebuildsTheSamePileArray()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The pile array is private, so the rebuild is pinned through the total it produces: the
        // same PileCount must draw the same seeded piles and score them identically.
        Assert.Equal(first.SimulateRoundsWithLinearScans(), second.SimulateRoundsWithLinearScans());
        Assert.Equal(
            first.SortAscendingThenSumEveryOtherFromMiddle(),
            second.SortAscendingThenSumEveryOtherFromMiddle());
    }

    [Fact]
    public void SimulateRoundsWithLinearScans_SeededPiles_AgreesWithSortAscendingThenSumEveryOtherFromMiddle()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.SortAscendingThenSumEveryOtherFromMiddle(),
            harness.SimulateRoundsWithLinearScans());
    }

    [Fact]
    public void SortAscendingThenSumEveryOtherFromMiddle_SeededPiles_AgreesWithSimulateRoundsWithLinearScans()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.SimulateRoundsWithLinearScans(),
            harness.SortAscendingThenSumEveryOtherFromMiddle());
    }

    private static MaximumNumberOfCoinsYouCanGetBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfCoinsYouCanGetBenchmarks { PileCount = SmallestPileCount };
        harness.Setup();

        return harness;
    }
}
