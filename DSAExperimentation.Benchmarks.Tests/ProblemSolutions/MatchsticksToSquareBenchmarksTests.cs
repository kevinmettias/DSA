using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MatchsticksToSquareBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the hand-written four-bucket backtracker against the generic
// backtracking search over the same buckets - so a harness whose arms disagree is timing two
// different problems. Both arms return a bare bool, and the fixture pins which bool: the
// matchsticks are four interleaved copies of 1..SticksPerSide, so the copy each side collected sums
// to the same total and a square can always be made - a decisive literal the class comment names.
// Agreement here is therefore weak by construction: it witnesses that neither strategy reports the
// square unmakeable, which a strategy that always said true would also satisfy. Setup draws nothing
// from a stream beyond the shuffle seed, so the same SticksPerSide rebuilds the same matchsticks.
public sealed partial class MatchsticksToSquareBenchmarksTests
{
    private const int SmallestSticksPerSide = 6;

    // Four copies of the same run, one per side, so the side sums are equal and the square exists.
    private const bool ExpectedSquareVerdict = true;

    [Fact]
    public void Setup_SameSticksPerSide_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().CanMakeSquareByNaiveBacktracking(), BuildHarness().CanMakeSquareByNaiveBacktracking());
        Assert.Equal(BuildHarness().CanMakeSquareByGenericBacktrack(), BuildHarness().CanMakeSquareByGenericBacktrack());
    }

    [Fact]
    public void CanMakeSquareByNaiveBacktracking_FourInterleavedCopies_AgreesWithGenericBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSquareVerdict, harness.CanMakeSquareByNaiveBacktracking());
        Assert.Equal(harness.CanMakeSquareByGenericBacktrack(), harness.CanMakeSquareByNaiveBacktracking());
    }

    [Fact]
    public void CanMakeSquareByGenericBacktrack_FourInterleavedCopies_AgreesWithNaiveBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSquareVerdict, harness.CanMakeSquareByGenericBacktrack());
        Assert.Equal(harness.CanMakeSquareByNaiveBacktracking(), harness.CanMakeSquareByGenericBacktrack());
    }

    private static MatchsticksToSquareBenchmarks BuildHarness()
    {
        var harness = new MatchsticksToSquareBenchmarks { SticksPerSide = SmallestSticksPerSide };
        harness.Setup();

        return harness;
    }
}
