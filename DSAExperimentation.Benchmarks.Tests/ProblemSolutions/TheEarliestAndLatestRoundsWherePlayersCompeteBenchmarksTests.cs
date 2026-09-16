using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TheEarliestAndLatestRoundsWherePlayersCompeteBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - the unmemoized recursion
// that re-explores an identical (roundSize, low, high) state against the same recurrence routed
// through this repo's Memoizer - so a harness whose arms disagree is timing two different
// problems. Both arms return LeetCode's own (earliest, latest) round pair, so they are compared
// directly. Setup only derives the second tracked player from PlayerCount, so the same
// PlayerCount must rebuild the same bracket query.
public sealed partial class TheEarliestAndLatestRoundsWherePlayersCompeteBenchmarksTests
{
    private const int SmallestPlayerCount = 10;

    [Fact]
    public void Setup_SamePlayerCount_RebuildsTheSameQuery() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_SmallestPlayerCount_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_SmallestPlayerCount_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static TheEarliestAndLatestRoundsWherePlayersCompeteBenchmarks BuildHarness()
    {
        var harness = new TheEarliestAndLatestRoundsWherePlayersCompeteBenchmarks
        {
            PlayerCount = SmallestPlayerCount,
        };
        harness.Setup();

        return harness;
    }
}
