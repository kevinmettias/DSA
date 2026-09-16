using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SpiralMatrixIIIBenchmarks (ARCHITECTURE 17.9): both arms are
// SpiralMatrixIIISolution's walks out of the same start cell over the same rows x cols grid - a
// walk that guards every step with a visited set against a direct growing-stride walk - so a
// harness whose arms disagree is timing two different problems. Setup derives all four inputs
// from Size alone, so the same Size must rebuild the same walk parameters.
//
// The walk returns one coordinate pair per in-bounds cell, and the problem pins the order it
// visits them in, so the order-sensitive rendering compares the entire walk.
public sealed partial class SpiralMatrixIIIBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWalkParameters() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().GrowingStepDirectionWalk()),
            AnswerText.Of(BuildHarness().GrowingStepDirectionWalk()));

    [Fact]
    public void DirectionVectorWithVisitedSet_TwentyByTwentyGrid_AgreesWithGrowingStepDirectionWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.GrowingStepDirectionWalk()),
            AnswerText.Of(harness.DirectionVectorWithVisitedSet()));
    }

    [Fact]
    public void GrowingStepDirectionWalk_TwentyByTwentyGrid_AgreesWithDirectionVectorWithVisitedSet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DirectionVectorWithVisitedSet()),
            AnswerText.Of(harness.GrowingStepDirectionWalk()));
    }

    private static SpiralMatrixIIIBenchmarks BuildHarness()
    {
        var harness = new SpiralMatrixIIIBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
