using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimizeManhattanDistancesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning every surviving pair once per removed point against the
// 45-degree u/v transform, which answers each removal in O(1) off one sorted pass - so a harness whose arms
// disagree is timing two different problems. Both arms return the minimized maximum distance as an int, so
// they are compared directly, and neither mutates the point cloud or the sorted transform arrays, so one
// harness is safe to read twice in either order. Setup draws the points from one fixed seed and sorts both
// transforms from them, so the same PointCount must rebuild the same cloud and the same prepared arrays.
public sealed partial class MinimizeManhattanDistancesBenchmarksTests
{
    private const int SmallestPointCount = 50;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededPointCloud_AgreesWithManhattanTransform()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManhattanTransform(), harness.BruteForce());
    }

    [Fact]
    public void ManhattanTransform_SeededPointCloud_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ManhattanTransform());
    }

    private static MinimizeManhattanDistancesBenchmarks BuildHarness()
    {
        var harness = new MinimizeManhattanDistancesBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
