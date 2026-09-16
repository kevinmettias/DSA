using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SetMatrixZeroesBenchmarks (ARCHITECTURE 17.9): both arms zero the same matrix
// in place, so a harness whose arms disagree is timing two different matrices. Each arm clones the
// pristine matrix inside the call and zeroes the clone - the solution mutates its argument, and
// [GlobalSetup] runs once rather than once per invocation - so neither arm ever writes the field and
// one harness instance is safe to call twice in either order. Setup draws the cells from one fixed
// seed, so the same Size must rebuild the same matrix; the answers are whole matrices, which
// AnswerText.Of renders row by row because LeetCode pins the grid's own order.
public sealed partial class SetMatrixZeroesBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CopyAndScan()),
            AnswerText.Of(BuildHarness().CopyAndScan()));

    [Fact]
    public void CopyAndScan_SparseZeroMatrix_AgreesWithRowColumnSets()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.RowColumnSets()), AnswerText.Of(harness.CopyAndScan()));
    }

    [Fact]
    public void RowColumnSets_SparseZeroMatrix_AgreesWithCopyAndScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.CopyAndScan()), AnswerText.Of(harness.RowColumnSets()));
    }

    private static SetMatrixZeroesBenchmarks BuildHarness()
    {
        var harness = new SetMatrixZeroesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
