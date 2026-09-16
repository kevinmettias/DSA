using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumPartitionFactorBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumPartitionFactorSolution's competing strategies for one question - the linear scan over
// candidate thresholds against the binary search over "is the too-close graph bipartite" - so a
// harness whose arms disagree is timing two different problems. Both answer with a single partition
// factor, compared directly.
//
// The point cloud is a closed form over PointCount with no draw from any stream, so the same
// PointCount is the whole of what pins it.
public sealed partial class MaximumPartitionFactorBenchmarksTests
{
    private const int SmallestPointCount = 50;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSameGridPoints()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(first.LinearScan(), second.LinearScan());
        Assert.Equal(first.BinarySearchBipartiteCheck(), second.BinarySearchBipartiteCheck());
    }

    [Fact]
    public void LinearScan_FlattenedGridPoints_AgreesWithBinarySearchBipartiteCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchBipartiteCheck(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearchBipartiteCheck_FlattenedGridPoints_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchBipartiteCheck());
    }

    private static MaximumPartitionFactorBenchmarks BuildHarness()
    {
        var harness = new MaximumPartitionFactorBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
