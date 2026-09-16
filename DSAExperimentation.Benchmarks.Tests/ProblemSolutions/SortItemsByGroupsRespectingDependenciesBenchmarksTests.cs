using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortItemsByGroupsRespectingDependenciesBenchmarks (ARCHITECTURE 17.9):
// both arms are SortItemsByGroupsRespectingDependenciesSolution's two-level topological sorts of
// the same prepared item and group graphs, so a harness whose arms disagree is timing two
// different problems. Setup is a pure function of ItemCount and hands each arm its own hoisted
// graphs, so the same ItemCount must rebuild the same acyclic two-level graph.
//
// WEAK BY CONSTRUCTION, and reported as such: each arm reports only the length of the order it
// produced, not the order itself, so agreement witnesses that both sorts placed every item - an
// arm that dropped an item is caught, an arm that placed the same items in a different valid
// order is not. The length is also asserted against the item count, which is what makes the
// comparison decisive rather than two arms sharing a wrong total: Setup wires only lower-id to
// higher-id edges, so both graphs are acyclic and every item must come back.
public sealed partial class SortItemsByGroupsRespectingDependenciesBenchmarksTests
{
    private const int SmallestItemCount = 50;

    // Every item is reachable in an acyclic graph, so a complete order holds all of them.
    private const int ExpectedSortedItemCount = SmallestItemCount;

    [Fact]
    public void Setup_SameItemCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveRescanTwoLevelSort(), BuildHarness().NaiveRescanTwoLevelSort());

    [Fact]
    public void NaiveRescanTwoLevelSort_FiftyItemsInFiveGroups_AgreesWithKahnsTwoLevelSort()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSortedItemCount, harness.NaiveRescanTwoLevelSort());
        Assert.Equal(harness.KahnsTwoLevelSort(), harness.NaiveRescanTwoLevelSort());
    }

    [Fact]
    public void KahnsTwoLevelSort_FiftyItemsInFiveGroups_AgreesWithNaiveRescanTwoLevelSort()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSortedItemCount, harness.KahnsTwoLevelSort());
        Assert.Equal(harness.NaiveRescanTwoLevelSort(), harness.KahnsTwoLevelSort());
    }

    private static SortItemsByGroupsRespectingDependenciesBenchmarks BuildHarness()
    {
        var harness = new SortItemsByGroupsRespectingDependenciesBenchmarks { ItemCount = SmallestItemCount };
        harness.Setup();

        return harness;
    }
}
