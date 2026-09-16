using DSAExperimentation.LeetCode.MaximumIceCreamBars;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumIceCreamBars;

// Harness only: both strategies live in MaximumIceCreamBarsSolution. One test
// method per strategy over one shared set of examples, so a failure names the
// strategy that broke - the O(n^2) selection scan included, which was previously a
// benchmark-only arm nothing asserted.
public sealed partial class MaximumIceCreamBarsTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 3, 2, 4, 1], 7, 4 }, // LC example 1: 1 + 1 + 2 + 3
            { [10, 6, 8, 7, 7, 8], 5, 0 }, // LC example 2: even the cheapest bar is unaffordable
            { [1, 6, 3, 1, 2, 5], 20, 6 }, // LC example 3: the whole tray costs 18
            { [1, 3, 2, 4, 1], 11, 5 }, // coins exactly cover every bar
            { [5], 5, 1 }, // one bar, exactly affordable
            { [5], 4, 0 }, // one bar, one coin short
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxIceCreamBySelectionScan_LeetCodeExamples_ReturnsAffordableBarCount(int[] costs, int coins, int expected)
    {
        var actual = MaximumIceCreamBarsSolution.MaxIceCreamBySelectionScan(costs, coins);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxIceCreamByMergeSortGreedy_LeetCodeExamples_ReturnsAffordableBarCount(int[] costs, int coins, int expected)
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
