using DSAExperimentation.LeetCode.Solutions.MaximumIceCreamBars;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumIceCreamBars;

// Harness only: both strategies live in MaximumIceCreamBarsSolution. One test
// method per strategy over one shared set of cases, so a failure names the
// strategy that broke - the O(n^2) selection scan included, which was previously a
// benchmark-only arm nothing asserted. The cases sit on the affordability
// boundary: coins that exactly cover every bar, and a single bar either exactly
// affordable or one coin short. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MaximumIceCreamBarsSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1, 3, 2, 4, 1], 11, 5 }, // coins exactly cover every bar
            { [5], 5, 1 }, // one bar, exactly affordable
            { [5], 4, 0 }, // one bar, one coin short
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxIceCreamBySelectionScan_Cases_ReturnsAffordableBarCount(int[] costs, int coins, int expected)
    {
        var actual = MaximumIceCreamBarsSolution.MaxIceCreamBySelectionScan(costs, coins);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxIceCreamByMergeSortGreedy_Cases_ReturnsAffordableBarCount(int[] costs, int coins, int expected)
    {
        var actual = MaximumIceCreamBarsSolution.MaxIceCreamByMergeSortGreedy(costs, coins);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MaxIceCreamByMergeSortGreedy_SortingStrategy_DoesNotReorderCallersArray()
    {
        int[] costs = [1, 3, 2, 4, 1];

        MaximumIceCreamBarsSolution.MaxIceCreamByMergeSortGreedy(costs, coins: 7);

        Assert.Equal([1, 3, 2, 4, 1], costs);
    }
}
