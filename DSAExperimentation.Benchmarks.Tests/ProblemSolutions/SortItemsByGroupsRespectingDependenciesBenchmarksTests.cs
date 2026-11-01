using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortItemsByGroupsRespectingDependenciesBenchmarks (ARCHITECTURE 17.9):
// each arm returns the order it produced, and on this workload only one order is valid: Setup wires
// only lower-id to higher-id edges, so both graphs are acyclic and every item must come back, and
// every item i below the last has an edge to i + 1, so the item ids must come back as 0..n-1 in
// order. The baseline is asserted against that order, which catches a dropped item and a misplaced
// one alike, rather than two arms sharing a wrong answer; the generic arm check holds the other arm
// to the baseline's answer.
public sealed partial class SortItemsByGroupsRespectingDependenciesBenchmarksTests
{
    private const int SmallestItemCount = 50;

    // Every item is reachable in an acyclic graph, so a complete order holds all of them.
    private const int ExpectedSortedItemCount = SmallestItemCount;

    [Fact]
    public void NaiveRescanTwoLevelSort_FiftyItemsInFiveGroups_PlacesEveryItemInIdOrder() =>
        AssertTheOnlyValidOrder(BuildHarness().NaiveRescanTwoLevelSort());

    private static void AssertTheOnlyValidOrder(int[] order)
    {
        Assert.Equal(ExpectedSortedItemCount, order.Length);
        Assert.Equal(Enumerable.Range(0, SmallestItemCount), order);
    }

    private static SortItemsByGroupsRespectingDependenciesBenchmarks BuildHarness()
    {
        var harness = new SortItemsByGroupsRespectingDependenciesBenchmarks { ItemCount = SmallestItemCount };
        harness.Setup();

        return harness;
    }
}
