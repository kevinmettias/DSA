using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MatchsticksToSquareBenchmarks (ARCHITECTURE 17.9): the baseline returns a
// bare bool, and the fixture pins which bool: the matchsticks are four interleaved copies of
// 1..SticksPerSide, so the copy each side collected sums to the same total and a square can always
// be made - a decisive literal the class comment names, and an expected value the generic agreement
// check cannot supply. It is weak by construction: it witnesses that the baseline does not report
// the square unmakeable, which a strategy that always said true would also satisfy.
public sealed partial class MatchsticksToSquareBenchmarksTests
{
    private const int SmallestSticksPerSide = 2;

    // Four copies of the same run, one per side, so the side sums are equal and the square exists.
    private const bool ExpectedSquareVerdict = true;

    [Fact]
    public void CanMakeSquareByNaiveBacktracking_FourInterleavedCopies_FindsTheSquare() =>
        Assert.Equal(ExpectedSquareVerdict, BuildHarness().CanMakeSquareByNaiveBacktracking());

    private static MatchsticksToSquareBenchmarks BuildHarness()
    {
        var harness = new MatchsticksToSquareBenchmarks { SticksPerSide = SmallestSticksPerSide };
        harness.Setup();

        return harness;
    }
}
