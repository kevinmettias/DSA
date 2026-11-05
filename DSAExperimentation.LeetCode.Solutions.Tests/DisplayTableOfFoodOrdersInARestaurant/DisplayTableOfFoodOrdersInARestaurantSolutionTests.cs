using DSAExperimentation.LeetCode.Solutions.DisplayTableOfFoodOrdersInARestaurant;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DisplayTableOfFoodOrdersInARestaurant;

// Harness only: both strategies live in DisplayTableOfFoodOrdersInARestaurantSolution
// and are asserted against the same cases - LeetCode's three published ones plus
// the multi-table case the original test carried, which exercises a food ordered by
// several tables and a table that ordered only some of the foods.
public sealed partial class DisplayTableOfFoodOrdersInARestaurantSolutionTests
{
    public static TheoryData<string[][], string[][]> Cases =>
        new()
        {
            {
                [
                    ["David", "3", "Ceviche"],
                    ["Corina", "10", "Beef Burrito"],
                    ["David", "3", "Fried Chicken"],
                    ["Carla", "5", "Water"],
                    ["Carla", "5", "Ham Burger"],
                    ["Phoebe", "5", "Water"],
                    ["Phoebe", "5", "Ham Burger"],
                    ["Phoebe", "5", "Fried Chicken"],
                    ["Ashley", "10", "Fried Chicken"],
                ],
                [
                    ["Table", "Beef Burrito", "Ceviche", "Fried Chicken", "Ham Burger", "Water"],
                    ["3", "0", "1", "1", "0", "0"],
                    ["5", "0", "0", "1", "2", "2"],
                    ["10", "1", "0", "1", "0", "0"],
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void DisplayTableByRescanPerCell_Cases_ReturnsSortedDisplayTable(
        string[][] orders, string[][] expected) =>
        AssertRows(expected, DisplayTableOfFoodOrdersInARestaurantSolution.DisplayTableByRescanPerCell(orders));

    [Theory]
    [MemberData(nameof(Cases))]
    public void DisplayTableByGroupedHashMap_Cases_ReturnsSortedDisplayTable(
        string[][] orders, string[][] expected) =>
        AssertRows(expected, DisplayTableOfFoodOrdersInARestaurantSolution.DisplayTableByGroupedHashMap(orders));

    private static void AssertRows(string[][] expected, List<List<string>> actual)
    {
        Assert.Equal(expected.Length, actual.Count);

        for (var row = 0; row < expected.Length; row++)
        {
            Assert.Equal(expected[row], actual[row]);
        }
    }
}
