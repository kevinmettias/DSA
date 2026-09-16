using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SurfaceAreaOf3DShapesBenchmarks (ARCHITECTURE 17.9): both arms are
// SurfaceAreaOf3DShapesSolution's - the per-neighbour bounds-checked pass against the same pass over a
// padded border - so a harness whose arms disagree is timing two different questions. Both answer with
// a bare int over a seeded square grid, so a rebuild at the same Size has to produce the same area
// even though the grid itself is private.
public sealed partial class SurfaceAreaOf3DShapesBenchmarksTests
{
    private const int SmallestSize = 50;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BoundsCheckedPerNeighbor(),
            BuildHarness().BoundsCheckedPerNeighbor());

    [Fact]
    public void BoundsCheckedPerNeighbor_SeededSquareGrid_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PaddedGridNoBoundsChecks(), harness.BoundsCheckedPerNeighbor());
    }

    [Fact]
    public void PaddedGridNoBoundsChecks_SeededSquareGrid_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BoundsCheckedPerNeighbor(), harness.PaddedGridNoBoundsChecks());
    }

    private static SurfaceAreaOf3DShapesBenchmarks BuildHarness()
    {
        var harness = new SurfaceAreaOf3DShapesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
