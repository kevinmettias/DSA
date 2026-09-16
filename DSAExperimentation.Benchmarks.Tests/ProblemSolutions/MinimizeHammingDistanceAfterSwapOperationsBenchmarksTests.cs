using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimizeHammingDistanceAfterSwapOperationsBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - an adjacency-list BFS that finds each index's
// swap-component against this repo's own DisjointSet, with both finishing the per-component shortfall the
// same way - so a harness whose arms disagree is timing two different problems. Both arms return the
// minimum Hamming distance as an int, so they are compared directly, and both only read the two value
// arrays and the swap pairs, so one harness is safe to read twice in either order. Setup draws all three
// from one fixed seed, so the same Length must rebuild the same source, target and swap pairs.
public sealed partial class MinimizeHammingDistanceAfterSwapOperationsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().AdjacencyListBfs(), BuildHarness().AdjacencyListBfs());

    [Fact]
    public void AdjacencyListBfs_SeededSourceTargetAndSwaps_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetUnionFind(), harness.AdjacencyListBfs());
    }

    [Fact]
    public void DisjointSetUnionFind_SeededSourceTargetAndSwaps_AgreesWithAdjacencyListBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AdjacencyListBfs(), harness.DisjointSetUnionFind());
    }

    private static MinimizeHammingDistanceAfterSwapOperationsBenchmarks BuildHarness()
    {
        var harness = new MinimizeHammingDistanceAfterSwapOperationsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
