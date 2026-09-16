using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SpiralMatrixIVBenchmarks (ARCHITECTURE 17.9): both arms are
// SpiralMatrixIVSolution's spiral fill of the same Size x Size grid from the same prepared chain
// - a direction-array walk carrying a visited matrix against the boundary shrink - so a harness
// whose arms disagree is timing two different problems. Setup is a pure function of Size, and
// neither arm mutates the chain it is handed, so the same Size must rebuild the same values and
// one harness is safe to call twice in either order.
//
// The cell values are random and can repeat, so the order-sensitive rendering compares the grid
// cell by cell rather than as a set: two arms that agree have placed the same value in the same
// cell, not merely used the same values.
public sealed partial class SpiralMatrixIVBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameChain() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BoundaryShrinkingSpiralWalk()),
            AnswerText.Of(BuildHarness().BoundaryShrinkingSpiralWalk()));

    [Fact]
    public void DirectionArrayWithVisitedTracking_FiftyByFiftyGrid_AgreesWithBoundaryShrinkingSpiralWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BoundaryShrinkingSpiralWalk()),
            AnswerText.Of(harness.DirectionArrayWithVisitedTracking()));
    }

    [Fact]
    public void BoundaryShrinkingSpiralWalk_FiftyByFiftyGrid_AgreesWithDirectionArrayWithVisitedTracking()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DirectionArrayWithVisitedTracking()),
            AnswerText.Of(harness.BoundaryShrinkingSpiralWalk()));
    }

    private static SpiralMatrixIVBenchmarks BuildHarness()
    {
        var harness = new SpiralMatrixIVBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
