using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GameOfLifeBenchmarks (ARCHITECTURE 17.9): both arms are GameOfLifeSolution's
// - the full-board copy against the live-cell set snapshot - so a harness whose arms disagree is
// timing two different boards. Both arms return the advanced board as int[][], whose row and column
// order the problem pins, so AnswerText.Of compares them cell by cell. Each arm clones the pristine
// board inside its own call because the solution advances in place, so one harness can be called
// twice in either order without the first call having moved the board the second one steps from -
// arm order does not matter here. Setup builds that pristine board off one seed, so the same Size
// must rebuild the same board.
public sealed partial class GameOfLifeBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameBoard() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().FullBoardCopy()),
            AnswerText.Of(BuildHarness().FullBoardCopy()));

    [Fact]
    public void FullBoardCopy_RandomBoard_AgreesWithSetSnapshot()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.SetSnapshot()), AnswerText.Of(harness.FullBoardCopy()));
    }

    [Fact]
    public void SetSnapshot_RandomBoard_AgreesWithFullBoardCopy()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.FullBoardCopy()), AnswerText.Of(harness.SetSnapshot()));
    }

    private static GameOfLifeBenchmarks BuildHarness()
    {
        var harness = new GameOfLifeBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
