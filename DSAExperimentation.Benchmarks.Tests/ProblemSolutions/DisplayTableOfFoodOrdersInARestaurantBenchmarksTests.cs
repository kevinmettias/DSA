using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DisplayTableOfFoodOrdersInARestaurantBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - rescanning every raw order once per
// (table, food) cell against one grouped pass into a nested HashMap - so a harness whose arms
// disagree is timing two different problems. AnswerText.Of, not OfUnorderedSet: LC 1418 fixes the
// display table's order outright - "Table" then the foods alphabetically, then one row per table in
// increasing numeric order - so that order is the answer. Setup draws tables and foods from the two
// fixed pools below, never from the order count, which is what bounds the table's dimensions.
public sealed partial class DisplayTableOfFoodOrdersInARestaurantBenchmarksTests
{
    // The two fixed pools Setup draws from, mirrored so the shape of the table it builds can be
    // bounded without reading the private order array.
    private const int TablePoolSize = 30;
    private const int FoodPoolSize = 15;
    private const int SmallestOrderCount = 200;
    private const string ExpectedHeaderFirstCell = "Table";
    private const int MaximumTableRowCount = TablePoolSize + 1;
    private const int MaximumColumnCount = FoodPoolSize + 1;

    [Fact]
    public void Setup_BoundedTableAndFoodPools_FrameTheDisplayTableAndRebuildTheSameWorkload()
    {
        var harness = BuildHarness();
        var table = harness.RescanEveryOrderPerCell();

        Assert.Equal(ExpectedHeaderFirstCell, table[0][0]);
        Assert.InRange(table.Count, 2, MaximumTableRowCount);
        Assert.InRange(table[0].Count, 2, MaximumColumnCount);
        Assert.Equal(AnswerText.Of(table), AnswerText.Of(BuildHarness().RescanEveryOrderPerCell()));
    }

    [Fact]
    public void RescanEveryOrderPerCell_BoundedPools_AgreesWithGroupedHashMapThenMergeSort()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.GroupedHashMapThenMergeSort()),
            AnswerText.Of(harness.RescanEveryOrderPerCell()));
    }

    [Fact]
    public void GroupedHashMapThenMergeSort_BoundedPools_AgreesWithRescanEveryOrderPerCell()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RescanEveryOrderPerCell()),
            AnswerText.Of(harness.GroupedHashMapThenMergeSort()));
    }

    private static DisplayTableOfFoodOrdersInARestaurantBenchmarks BuildHarness()
    {
        var harness = new DisplayTableOfFoodOrdersInARestaurantBenchmarks { OrderCount = SmallestOrderCount };
        harness.Setup();

        return harness;
    }
}
