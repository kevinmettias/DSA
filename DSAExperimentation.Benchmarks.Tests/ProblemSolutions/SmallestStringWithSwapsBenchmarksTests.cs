using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestStringWithSwapsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - an adjacency-list BFS over the swap pairs
// against this repo's own DisjointSet - so a harness whose arms disagree is swapping two
// different positions. Setup draws the source string and the pair array from one fixed seed,
// so the same Length must rebuild the same workload; otherwise two published numbers were
// never comparable.
//
// The answer is a rearrangement of the same characters into the same positions, so it is
// always exactly as long as the source: that length is checked alongside the agreement so an
// arm that dropped or duplicated a position cannot agree with a sibling that did the same.
public sealed partial class SmallestStringWithSwapsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameSourceAndPairs() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().AdjacencyListBfs()),
            AnswerText.Of(BuildHarness().AdjacencyListBfs()));

    [Fact]
    public void AdjacencyListBfs_TwoHundredIndexSource_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();
        var answer = harness.AdjacencyListBfs();

        Assert.Equal(AnswerText.Of(harness.DisjointSetUnionFind()), AnswerText.Of(answer));
        Assert.Equal(SmallestLength, answer.Length);
    }

    [Fact]
    public void DisjointSetUnionFind_TwoHundredIndexSource_AgreesWithAdjacencyListBfs()
    {
        var harness = BuildHarness();
        var answer = harness.DisjointSetUnionFind();

        Assert.Equal(AnswerText.Of(harness.AdjacencyListBfs()), AnswerText.Of(answer));
        Assert.Equal(SmallestLength, answer.Length);
    }

    private static SmallestStringWithSwapsBenchmarks BuildHarness()
    {
        var harness = new SmallestStringWithSwapsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
