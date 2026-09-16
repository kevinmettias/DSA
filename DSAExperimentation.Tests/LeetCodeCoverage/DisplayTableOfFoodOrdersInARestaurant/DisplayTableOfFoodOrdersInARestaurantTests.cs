using DSAExperimentation.LeetCode.DisplayTableOfFoodOrdersInARestaurant;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DisplayTableOfFoodOrdersInARestaurant;

// Harness only: both strategies live in DisplayTableOfFoodOrdersInARestaurantSolution
// and are asserted against the same examples - LeetCode's three published ones plus
// the multi-table case the original test carried, which exercises a food ordered by
// several tables and a table that ordered only some of the foods.
public sealed partial class DisplayTableOfFoodOrdersInARestaurantTests
{
    public static TheoryData<string[][], string[][]> Examples =>
        new()
        {
            {
                [
                    ["David", "3", "Ceviche"],
                    ["Corina", "10", "Beef Burrito"],
                    ["David", "3", "Fried Chicken"],
                    ["Carla", "5", "Water"],
                    ["Carla", "5", "Ceviche"],
                    ["Rous", "3", "Ceviche"],
                ],
                [
                    ["Table", "Beef Burrito", "Ceviche", "Fried Chicken", "Water"],
                    ["3", "0", "2", "1", "0"],
                    ["5", "0", "1", "0", "1"],
                    ["10", "1", "0", "0", "0"],
                ]
            },
            {
                [
                    ["James", "12", "Fried Chicken"],
                    ["Ratesh", "12", "Fried Chicken"],
                    ["Amadeus", "12", "Fried Chicken"],
                    ["Adam", "1", "Canadian Waffles"],
                    ["Brianna", "1", "Canadian Waffles"],
                ],
                [
                    ["Table", "Canadian Waffles", "Fried Chicken"],
                    ["1", "2", "0"],
                    ["12", "0", "3"],
                ]
            },
            {
                [
                    ["Laura", "2", "Bean Burrito"],
                    ["Jhon", "2", "Beef Burrito"],
                    ["Melissa", "2", "Soda"],
                ],
                [
                    ["Table", "Bean Burrito", "Beef Burrito", "Soda"],
                    ["2", "1", "1", "1"],
                ]
            },
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
    [MemberData(nameof(Examples))]
    public void DisplayTableByRescanPerCell_LeetCodeExamples_ReturnsSortedDisplayTable(
        string[][] orders, string[][] expected) =>
        AssertRows(expected, DisplayTableOfFoodOrdersInARestaurantSolution.DisplayTableByRescanPerCell(orders));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DisplayTableByGroupedHashMap_LeetCodeExamples_ReturnsSortedDisplayTable(
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
