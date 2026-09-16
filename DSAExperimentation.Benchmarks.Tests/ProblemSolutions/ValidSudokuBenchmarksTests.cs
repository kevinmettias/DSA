using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidSudokuBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidSudokuSolution's competing strategies for the same question - a 9 x 9 boolean grid of
// seen-marks against a set of composite row/column/box keys - so a harness whose arms disagree is
// validating two different boards.
//
// The class carries no [Params]: [GlobalSetup] fixes one board, LC 36's own first example, whose
// published answer is true. That decisive literal is asserted alongside the arms' agreement, so a
// shared wrong verdict cannot pass. Both arms return bool, so this harness witnesses the verdict
// only, not the per-cell marks behind it.
public sealed partial class ValidSudokuBenchmarksTests
{
    // LC 36's own first example: no digit repeats in a row, a column or a 3 x 3 box.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void Setup_FixedBoard_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().IsValidByBooleanGrid(), BuildHarness().IsValidByBooleanGrid());
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByBooleanGrid());
    }

    [Fact]
    public void IsValidByBooleanGrid_FixedBoard_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidBySetKeys(), harness.IsValidByBooleanGrid());
        Assert.Equal(ExpectedIsValid, harness.IsValidByBooleanGrid());
    }

    [Fact]
    public void IsValidBySetKeys_FixedBoard_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidByBooleanGrid(), harness.IsValidBySetKeys());
        Assert.Equal(ExpectedIsValid, harness.IsValidBySetKeys());
    }

    private static ValidSudokuBenchmarks BuildHarness()
    {
        var harness = new ValidSudokuBenchmarks();
        harness.Setup();

        return harness;
    }
}
