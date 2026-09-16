using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LexicographicallySmallestEquivalentStringBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - BFS over the 26-letter adjacency lists
// against this repo's own DisjointSet(26) - so a harness whose arms disagree is timing two different
// problems. Setup draws s1, s2 and baseStr from one fixed seed, so the same Length must rebuild the
// same three strings; otherwise two published numbers were never comparable in the first place.
public sealed partial class LexicographicallySmallestEquivalentStringBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameStrings() =>
        Assert.Equal(BuildHarness().AdjacencyListBfs(), BuildHarness().AdjacencyListBfs());

    [Fact]
    public void AdjacencyListBfs_SeededEquivalencePairs_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetUnionFind(), harness.AdjacencyListBfs());
    }

    [Fact]
    public void DisjointSetUnionFind_SeededEquivalencePairs_AgreesWithAdjacencyListBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AdjacencyListBfs(), harness.DisjointSetUnionFind());
    }

    private static LexicographicallySmallestEquivalentStringBenchmarks BuildHarness()
    {
        var harness = new LexicographicallySmallestEquivalentStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
