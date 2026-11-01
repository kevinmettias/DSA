using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DisplayTableOfFoodOrdersInARestaurantBenchmarks (ARCHITECTURE 17.9): the
// display table's frame, known from the workload's shape without either arm. LC 1418 puts "Table"
// in the header's first cell, and Setup draws tables and foods from the two fixed pools below,
// never from the order count, which is what bounds the table's dimensions.
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

    // The header row plus at least one table row, and likewise the header cell plus at least one
    // food column.
    private const int MinimumTableDimensionCount = 2;

    [Fact]
    public void RescanEveryOrderPerCell_BoundedTableAndFoodPools_FramesTheDisplayTable()
    {
        var table = BuildHarness().RescanEveryOrderPerCell();

        Assert.Equal(ExpectedHeaderFirstCell, table[0][0]);
        Assert.InRange(table.Count, MinimumTableDimensionCount, MaximumTableRowCount);
        Assert.InRange(table[0].Count, MinimumTableDimensionCount, MaximumColumnCount);
    }

    private static DisplayTableOfFoodOrdersInARestaurantBenchmarks BuildHarness()
    {
        var harness = new DisplayTableOfFoodOrdersInARestaurantBenchmarks { OrderCount = SmallestOrderCount };
        harness.Setup();

        return harness;
    }
}
