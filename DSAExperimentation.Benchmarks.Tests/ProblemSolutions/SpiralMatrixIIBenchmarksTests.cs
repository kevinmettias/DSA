using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SpiralMatrixIIBenchmarks (ARCHITECTURE 17.9): both arms are
// SpiralMatrixIISolution's generators of the same Size x Size spiral - a direction vector with a
// visited set against the boundary shrink - so a harness whose arms disagree is timing two
// different problems. There is no [GlobalSetup] to rebuild: the whole input is Size, which the
// caller hands each arm directly.
//
// The generated matrix is numbered 1 .. Size^2 in spiral order, so every value is distinct and
// the order-sensitive rendering of the result is the matrix itself: two arms that agree on it
// filled every cell with the same number, not merely the same set of numbers.
public sealed partial class SpiralMatrixIIBenchmarksTests
{
    private const int SmallestSize = 10;

    [Fact]
    public void DirectionVectorWithVisitedSet_TenByTenMatrix_AgreesWithBoundaryShrinking()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BoundaryShrinking()),
            AnswerText.Of(harness.DirectionVectorWithVisitedSet()));
    }

    [Fact]
    public void BoundaryShrinking_TenByTenMatrix_AgreesWithDirectionVectorWithVisitedSet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DirectionVectorWithVisitedSet()),
            AnswerText.Of(harness.BoundaryShrinking()));
    }

    private static SpiralMatrixIIBenchmarks BuildHarness() => new() { Size = SmallestSize };
}
