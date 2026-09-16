using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReshapeTheMatrixBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - recomputing both the source and the destination row/col from a
// flat index per cell against a cursor that only wraps when a destination row fills - so a harness
// whose arms disagree is timing two different problems. Setup draws the matrix from one fixed seed
// and derives the target shape from Rows alone, so the same Rows must rebuild the same workload;
// otherwise two published numbers were never comparable in the first place.
//
// Setup fixes the target shape as well: half as many columns as the source and twice as many rows,
// so the total cell count is preserved and the reshaping branch is taken rather than the
// "return the original unchanged" branch. That shape is visible on each arm's own answer, which is
// why the reshape's row and column counts are asserted alongside the agreement.
public sealed partial class ReshapeTheMatrixBenchmarksTests
{
    private const int SmallestRows = 20;

    // The benchmark reshapes into twice as many rows and half as many columns of the same width.
    private const int TargetReshapeFactor = 2;
    private const int SourceColumnCount = 8;
    private const int ExpectedTargetRowCount = SmallestRows * TargetReshapeFactor;
    private const int ExpectedTargetColumnCount = SourceColumnCount / TargetReshapeFactor;

    [Fact]
    public void Setup_SameRows_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearIndexDivMod()),
            AnswerText.Of(BuildHarness().LinearIndexDivMod()));

    [Fact]
    public void LinearIndexDivMod_DoubledRows_AgreesWithCursorWalk()
    {
        var harness = BuildHarness();
        var reshaped = harness.LinearIndexDivMod();

        Assert.Equal(ExpectedTargetRowCount, reshaped.Length);
        Assert.Equal(ExpectedTargetColumnCount, reshaped[0].Length);
        Assert.Equal(AnswerText.Of(harness.CursorWalk()), AnswerText.Of(reshaped));
    }

    [Fact]
    public void CursorWalk_DoubledRows_AgreesWithLinearIndexDivMod()
    {
        var harness = BuildHarness();
        var reshaped = harness.CursorWalk();

        Assert.Equal(ExpectedTargetRowCount, reshaped.Length);
        Assert.Equal(ExpectedTargetColumnCount, reshaped[0].Length);
        Assert.Equal(AnswerText.Of(harness.LinearIndexDivMod()), AnswerText.Of(reshaped));
    }

    private static ReshapeTheMatrixBenchmarks BuildHarness()
    {
        var harness = new ReshapeTheMatrixBenchmarks { Rows = SmallestRows };
        harness.Setup();

        return harness;
    }
}
