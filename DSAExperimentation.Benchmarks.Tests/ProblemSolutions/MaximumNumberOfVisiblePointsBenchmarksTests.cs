using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfVisiblePointsBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumNumberOfVisiblePointsSolution's competing strategies for one question - the quadratic
// pairwise brute force against one merge sort followed by a two-pointer sweep over the
// angle-doubled array - so a harness whose arms disagree is timing two different problems. Both
// answer with a single visible-point count, compared directly.
public sealed partial class MaximumNumberOfVisiblePointsBenchmarksTests
{
    private const int SmallestPointCount = 200;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSamePointCloud()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The point cloud is private, so the rebuild is pinned through the count it produces: the
        // same PointCount must draw the same seeded offsets from Location (never landing on it)
        // and window them identically.
        Assert.Equal(first.PairwiseBruteForce(), second.PairwiseBruteForce());
        Assert.Equal(first.SortAndSlideWindow(), second.SortAndSlideWindow());
    }

    [Fact]
    public void PairwiseBruteForce_SeededPointCloud_AgreesWithSortAndSlideWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortAndSlideWindow(), harness.PairwiseBruteForce());
    }

    [Fact]
    public void SortAndSlideWindow_SeededPointCloud_AgreesWithPairwiseBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PairwiseBruteForce(), harness.SortAndSlideWindow());
    }

    private static MaximumNumberOfVisiblePointsBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfVisiblePointsBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
