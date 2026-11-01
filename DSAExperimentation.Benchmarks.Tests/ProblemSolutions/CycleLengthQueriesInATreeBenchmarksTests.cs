using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CycleLengthQueriesInATreeBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is the workload's documented shape.
// Setup fixes the id range to a complete binary tree of a fixed depth and draws one pair of distinct
// ids per query, so the reading's documented shape is one length per query, each at least the closing
// edge plus a single path edge and at most the two full root paths of a deepest-leaf pair.
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
    public void AncestorDictionaryWalk_OneThousandSeededQueries_AnswersOneBoundedLengthPerQuery()
    {
        var lengths = BuildHarness().AncestorDictionaryWalk();

        Assert.Equal(SmallestQueriesCount, lengths.Length);
        Assert.All(lengths, length => Assert.InRange(length, MinimumCycleLength, MaximumCycleLength));
    }

    private static CycleLengthQueriesInATreeBenchmarks BuildHarness()
    {
        var harness = new CycleLengthQueriesInATreeBenchmarks { QueriesCount = SmallestQueriesCount };
        harness.Setup();

        return harness;
    }
}
