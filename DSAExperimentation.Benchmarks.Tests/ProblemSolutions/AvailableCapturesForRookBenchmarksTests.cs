using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AvailableCapturesForRookBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to
// each other, and this bounds the count by the rules themselves - a rook captures at most one pawn per direction,
// so the count is at most four, a bound no comparison between arms can establish.
public sealed partial class AvailableCapturesForRookBenchmarksTests
{
    // Four directions, at most one pawn captured in each before the rook is blocked.
    private const int MaxRookCaptures = 4;

    [Fact]
    public void FullBoardScan_EightByEightBoard_CountsAtMostOneCapturePerDirection() =>
        Assert.InRange(BuildHarness().FullBoardScan(), 0, MaxRookCaptures);

    private static AvailableCapturesForRookBenchmarks BuildHarness()
    {
        var harness = new AvailableCapturesForRookBenchmarks();
        harness.Setup();

        return harness;
    }
}
