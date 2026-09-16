using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SpiralMatrixBenchmarks (ARCHITECTURE 17.9): both arms are
// SpiralMatrixSolution's traversals of the same square matrix - a visited-grid simulation against
// the four-boundary-pointer shrink - so a harness whose arms disagree is timing two different
// problems. Setup is a pure function of Size, so the same Size must rebuild the same matrix.
//
// The matrix Setup builds is numbered 0 .. Size^2 - 1 in row-major order, which is what makes the
// two traversals comparable: every value is distinct, so the order-sensitive rendering of the
// result is the traversal order itself and two arms that agree on it have produced the same
// spiral rather than merely the same values. Neither arm mutates the matrix, so one harness is
// safe to call twice in either order.
public sealed partial class SpiralMatrixBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameMatrix() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().VisitedGridWalk()),
            AnswerText.Of(BuildHarness().VisitedGridWalk()));

    [Fact]
    public void VisitedGridWalk_TwentyByTwentyMatrix_AgreesWithBoundaryPointerShrink()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BoundaryPointerShrink()),
            AnswerText.Of(harness.VisitedGridWalk()));
    }

    [Fact]
    public void BoundaryPointerShrink_TwentyByTwentyMatrix_AgreesWithVisitedGridWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.VisitedGridWalk()),
            AnswerText.Of(harness.BoundaryPointerShrink()));
    }

    private static SpiralMatrixBenchmarks BuildHarness()
    {
        var harness = new SpiralMatrixBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
