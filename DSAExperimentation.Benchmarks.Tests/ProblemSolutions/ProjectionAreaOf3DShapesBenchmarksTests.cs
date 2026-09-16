using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ProjectionAreaOf3DShapesBenchmarks (ARCHITECTURE 17.9): its three arms are
// competing strategies for one question - the summed area of the three projections of the same grid -
// so a harness whose arms disagree is timing different problems. Size is the only [Params] axis and
// Setup derives the square grid from it, so the same Size must rebuild the same grid. Each arm's own
// fact holds it against both of the others, so no arm is left compared only to one of its siblings.
public sealed partial class ProjectionAreaOf3DShapesBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameGrid() =>
        Assert.Equal(BuildHarness().ThreeSeparatePasses(), BuildHarness().ThreeSeparatePasses());

    [Fact]
    public void ThreeSeparatePasses_AgreesWithEveryOtherStrategy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RowAndColumnPasses(), harness.ThreeSeparatePasses());
        Assert.Equal(harness.SingleCombinedPass(), harness.ThreeSeparatePasses());
    }

    [Fact]
    public void RowAndColumnPasses_AgreesWithEveryOtherStrategy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ThreeSeparatePasses(), harness.RowAndColumnPasses());
        Assert.Equal(harness.SingleCombinedPass(), harness.RowAndColumnPasses());
    }

    [Fact]
    public void SingleCombinedPass_AgreesWithEveryOtherStrategy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ThreeSeparatePasses(), harness.SingleCombinedPass());
        Assert.Equal(harness.RowAndColumnPasses(), harness.SingleCombinedPass());
    }

    private static ProjectionAreaOf3DShapesBenchmarks BuildHarness()
    {
        var harness = new ProjectionAreaOf3DShapesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
