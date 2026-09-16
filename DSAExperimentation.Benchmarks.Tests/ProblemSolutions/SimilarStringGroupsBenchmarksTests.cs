using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SimilarStringGroupsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the naive list of index sets scanned linearly
// for every merge against this repo's DisjointSet - so a harness whose arms disagree is
// grouping two different word pools. Setup grows the clustered pool from one fixed seed, so
// the same WordCount must rebuild the same words; otherwise two published numbers were never
// comparable.
public sealed partial class SimilarStringGroupsBenchmarksTests
{
    private const int SmallestWordCount = 60;

    [Fact]
    public void Setup_SameWordCount_RebuildsTheSameWordPool() =>
        Assert.Equal(BuildHarness().GroupListScan(), BuildHarness().GroupListScan());

    [Fact]
    public void GroupListScan_SixtyWordClusterPool_AgreesWithDisjointSetByRank()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetByRank(), harness.GroupListScan());
    }

    [Fact]
    public void DisjointSetByRank_SixtyWordClusterPool_AgreesWithGroupListScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GroupListScan(), harness.DisjointSetByRank());
    }

    private static SimilarStringGroupsBenchmarks BuildHarness()
    {
        var harness = new SimilarStringGroupsBenchmarks { WordCount = SmallestWordCount };
        harness.Setup();

        return harness;
    }
}
