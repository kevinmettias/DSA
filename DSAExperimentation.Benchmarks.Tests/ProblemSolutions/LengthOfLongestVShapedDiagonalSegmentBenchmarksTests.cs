using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LengthOfLongestVShapedDiagonalSegmentBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - walking every start cell until the segment
// bends against the directional DP that threads the four diagonal directions - so a harness whose
// arms disagree is timing two different problems. Setup builds the grid from one fixed seed, so the
// same GridSize must rebuild the same grid; otherwise two published numbers were never comparable.
public sealed partial class LengthOfLongestVShapedDiagonalSegmentBenchmarksTests
{
    private const int SmallestGridSize = 10;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameGrid() =>
        Assert.Equal(BuildHarness().BruteForceWalk(), BuildHarness().BruteForceWalk());

    [Fact]
    public void BruteForceWalk_SeededGrid_AgreesWithDirectionalDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DirectionalDp(), harness.BruteForceWalk());
    }

    [Fact]
    public void DirectionalDp_SeededGrid_AgreesWithBruteForceWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceWalk(), harness.DirectionalDp());
    }

    private static LengthOfLongestVShapedDiagonalSegmentBenchmarks BuildHarness()
    {
        var harness = new LengthOfLongestVShapedDiagonalSegmentBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
