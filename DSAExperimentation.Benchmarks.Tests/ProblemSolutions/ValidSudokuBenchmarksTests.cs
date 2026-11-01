using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidSudokuBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to agreeing with each other, which a shared wrong verdict would pass, so this pins the baseline's
// verdict to LeetCode's published one.
//
// The class carries no [Params]: [GlobalSetup] fixes one board, LC 36's own first example, whose
// published answer is true. Both arms return bool, so this harness witnesses the verdict only, not
// the per-cell marks behind it.
public sealed partial class ValidSudokuBenchmarksTests
{
    // LC 36's own first example: no digit repeats in a row, a column or a 3 x 3 box.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void IsValidByBooleanGrid_FixedBoard_AcceptsLeetCodesFirstExample() =>
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByBooleanGrid());

    private static ValidSudokuBenchmarks BuildHarness()
    {
        var harness = new ValidSudokuBenchmarks();
        harness.Setup();

        return harness;
    }
}
