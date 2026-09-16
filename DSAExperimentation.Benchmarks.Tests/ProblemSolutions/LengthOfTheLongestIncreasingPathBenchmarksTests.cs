using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LengthOfTheLongestIncreasingPathBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the quadratic all-pairs comparison against a
// segment-tree sweep - so a harness whose arms disagree is timing two different problems. Setup draws
// distinct points from one fixed seed and derives the required index from the point count, so the
// same PointCount must rebuild the same query; otherwise two published numbers were never comparable.
public sealed partial class LengthOfTheLongestIncreasingPathBenchmarksTests
{
    private const int SmallestPointCount = 200;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSameQuery() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededDistinctPoints_AgreesWithSegmentTreeSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeSweep(), harness.BruteForce());
    }

    [Fact]
    public void SegmentTreeSweep_SeededDistinctPoints_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SegmentTreeSweep());
    }

    private static LengthOfTheLongestIncreasingPathBenchmarks BuildHarness()
    {
        var harness = new LengthOfTheLongestIncreasingPathBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
