using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for Shift2DGridBenchmarks (ARCHITECTURE 17.9): both arms report the same shifted
// grid for the same (grid, shift count) input, so a harness whose arms disagree is timing two
// different shifts. Setup draws the cells from one fixed seed and derives the shift count from Size
// through the class's own halving constant, so the same Size must rebuild the same grid and the same
// partial rotation; both arms write into a fresh array and only read the field, so one harness
// instance is safe to call twice in either order. AnswerText.Of renders the grid row by row, which is
// the order LeetCode pins.
public sealed partial class Shift2DGridBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().IndexArithmeticShift()),
            AnswerText.Of(BuildHarness().IndexArithmeticShift()));

    [Fact]
    public void IndexArithmeticShift_SeededGridPartialRotation_AgreesWithDequeRotationShift()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.DequeRotationShift()), AnswerText.Of(harness.IndexArithmeticShift()));
    }

    [Fact]
    public void DequeRotationShift_SeededGridPartialRotation_AgreesWithIndexArithmeticShift()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.IndexArithmeticShift()), AnswerText.Of(harness.DequeRotationShift()));
    }

    private static Shift2DGridBenchmarks BuildHarness()
    {
        var harness = new Shift2DGridBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
