using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CycleLengthQueriesInATreeBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a per-query ancestor dictionary against the
// parent-index two-pointer walk - so a harness whose arms disagree is timing two different problems.
// Setup fixes the id range to a complete binary tree of a fixed depth and draws one pair of distinct
// ids per query, so the reading's documented shape is one length per query, each at least the closing
// edge plus a single path edge and at most the two full root paths of a deepest-leaf pair. The same
// QueriesCount must rebuild the same pairs and with them the same lengths.
public sealed partial class CycleLengthQueriesInATreeBenchmarksTests
{
    private const int SmallestQueriesCount = 1_000;

    // [GlobalSetup] bounds every id by a complete binary tree of this many levels.
    private const int TreeLevels = 20;

    private const int MinimumCycleLength = 2;

    // Two deepest nodes sit in different root subtrees at worst, so each side of the path contributes
    // TreeLevels - 1 edges and the added edge closes the cycle.
    private const int MaximumCycleLength = (2 * TreeLevels) - 1;

    [Fact]
    public void Setup_SameQueriesCount_RebuildsTheSameWorkload()
    {
        var lengths = BuildHarness().AncestorDictionaryWalk();

        Assert.Equal(SmallestQueriesCount, lengths.Length);
        Assert.All(lengths, length => Assert.InRange(length, MinimumCycleLength, MaximumCycleLength));
        Assert.Equal(
            AnswerText.Of(BuildHarness().AncestorDictionaryWalk()),
            AnswerText.Of(BuildHarness().AncestorDictionaryWalk()));
    }

    [Fact]
    public void AncestorDictionaryWalk_OneThousandSeededQueries_AgreesWithParentIndexTwoPointerWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ParentIndexTwoPointerWalk()),
            AnswerText.Of(harness.AncestorDictionaryWalk()));
    }

    [Fact]
    public void ParentIndexTwoPointerWalk_OneThousandSeededQueries_AgreesWithAncestorDictionaryWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.AncestorDictionaryWalk()),
            AnswerText.Of(harness.ParentIndexTwoPointerWalk()));
    }

    private static CycleLengthQueriesInATreeBenchmarks BuildHarness()
    {
        var harness = new CycleLengthQueriesInATreeBenchmarks { QueriesCount = SmallestQueriesCount };
        harness.Setup();

        return harness;
    }
}
