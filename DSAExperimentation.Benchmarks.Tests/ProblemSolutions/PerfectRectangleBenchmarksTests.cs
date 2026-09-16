using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PerfectRectangleBenchmarks (ARCHITECTURE 17.9): its two arms are
// PerfectRectangleSolution's, competing verdicts for the same rectangle set - pairwise overlap
// checks plus an area sum against counting corner parities - so a harness whose arms disagree is
// timing two different problems. Setup tiles the grid with unit squares from GridSize alone, which
// is a genuine perfect cover by construction, so the tests pin the answer as well as the agreement
// rather than letting two shared rejections pass as agreement.
public sealed partial class PerfectRectangleBenchmarksTests
{
    private const int SmallestGridSize = 20;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsRectangleCoverByPairwiseOverlap(),
            BuildHarness().IsRectangleCoverByPairwiseOverlap());

    [Fact]
    public void IsRectangleCoverByPairwiseOverlap_UnitSquareTiling_AgreesWithIsRectangleCoverByCornerToggle()
    {
        var harness = BuildHarness();

        Assert.True(harness.IsRectangleCoverByCornerToggle());
        Assert.Equal(
            harness.IsRectangleCoverByCornerToggle(),
            harness.IsRectangleCoverByPairwiseOverlap());
    }

    [Fact]
    public void IsRectangleCoverByCornerToggle_UnitSquareTiling_AgreesWithIsRectangleCoverByPairwiseOverlap()
    {
        var harness = BuildHarness();

        Assert.True(harness.IsRectangleCoverByPairwiseOverlap());
        Assert.Equal(
            harness.IsRectangleCoverByPairwiseOverlap(),
            harness.IsRectangleCoverByCornerToggle());
    }

    private static PerfectRectangleBenchmarks BuildHarness()
    {
        var harness = new PerfectRectangleBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
