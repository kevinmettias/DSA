using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumScoreTriangulationOfPolygonBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the cheapest triangulation of the vertex-weighted
// polygon - so a harness whose arms disagree is timing two different problems. Both arms run the same
// (left, right) interval recurrence and differ only in whether the repeated pair is cached, so
// agreement is what proves this repo's Memoizer returns what the un-memoized walk computes. Both read
// the one vertex-weight array [GlobalSetup] built from a fixed seed, so the same VertexCount must
// rebuild the same weights.
public sealed partial class MinimumScoreTriangulationOfPolygonBenchmarksTests
{
    private const int SmallestVertexCount = 10;

    [Fact]
    public void Setup_SameVertexCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MemoizedRecursion(), BuildHarness().MemoizedRecursion());

    [Fact]
    public void MemoizedRecursion_SameVertexWeights_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    [Fact]
    public void UnmemoizedRecursion_SameVertexWeights_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    private static MinimumScoreTriangulationOfPolygonBenchmarks BuildHarness()
    {
        var harness = new MinimumScoreTriangulationOfPolygonBenchmarks { VertexCount = SmallestVertexCount };
        harness.Setup();

        return harness;
    }
}
