using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DisplayTableOfFoodOrdersInARestaurant;

// LeetCode 1418. Display Table of Food Orders in a Restaurant: a nested
// HashMap<tableNumber, HashMap<foodName, count>> built in one pass over the raw
// orders, food names deduped via a second HashMap<string,bool>, both axes sorted
// with this repo's own MergeSort over ArrayIndexedSequence - the exact "group
// with HashMap, sort with MergeSort" composition AccountsMergeTests.cs already
// uses, just aggregating counts instead of deduping strings. Ordinal string
// comparison, same reasoning as AccountsMergeTests (culture-aware
// Comparer<string>.Default can disagree with LeetCode's expected byte-order sort
// on some locales); table numbers sort as integers via MergeSort's default
// IComparable<int> overload.
public sealed partial class DisplayTableOfFoodOrdersInARestaurantTests
{
    [Fact]
    public void DisplayTable_ClassicExample_GroupsAndSortsByTableThenFood()
    {
        string[][] orders =
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
        ];

        var table = DisplayTable(orders);

        Assert.Equal(4, table.Count);
        Assert.Equal(["Table", "Beef Burrito", "Ceviche", "Fried Chicken", "Ham Burger", "Water"], table[0]);
        Assert.Equal(["3", "0", "1", "1", "0", "0"], table[1]);
        Assert.Equal(["5", "0", "0", "1", "2", "2"], table[2]);
        Assert.Equal(["10", "1", "0", "1", "0", "0"], table[3]);
    }

    [Fact]
    public void DisplayTable_TwoTablesTwoFoods_ReturnsMatchingCounts()
    {
        string[][] orders =
        [
            ["James", "12", "Fried Chicken"],
            ["Ratesh", "12", "Fried Chicken"],
            ["Amadeus", "12", "Fried Chicken"],
            ["Adam", "1", "Canadian Waffles"],
            ["Brianna", "1", "Canadian Waffles"],
        ];

        var table = DisplayTable(orders);

        Assert.Equal(3, table.Count);
        Assert.Equal(["Table", "Canadian Waffles", "Fried Chicken"], table[0]);
        Assert.Equal(["1", "2", "0"], table[1]);
        Assert.Equal(["12", "0", "3"], table[2]);
    }

    private static List<List<string>> DisplayTable(string[][] orders)
    {
        var countsByTable = new HashMap<int, HashMap<string, int>>();
        var foodNames = new HashMap<string, bool>();

        foreach (var order in orders)
        {
            var tableNumber = int.Parse(order[1]);
            var foodItem = order[2];

            foodNames.Set(foodItem, true);

            if (!countsByTable.TryGetValue(tableNumber, out var foodCounts))
            {
                foodCounts = new HashMap<string, int>();
                countsByTable.Set(tableNumber, foodCounts);
            }

            foodCounts.TryGetValue(foodItem, out var count);
            foodCounts.Set(foodItem, count + 1);
        }

        var sortedFoods = foodNames.Keys.ToArray();
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(sortedFoods), StringComparer.Ordinal);

        var sortedTables = countsByTable.Keys.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedTables));

        var table = new List<List<string>> { new(["Table"]) };
        table[0].AddRange(sortedFoods);

        foreach (var tableNumber in sortedTables)
        {
            countsByTable.TryGetValue(tableNumber, out var foodCounts);
            var row = new List<string> { tableNumber.ToString() };

            foreach (var food in sortedFoods)
            {
                foodCounts.TryGetValue(food, out var count);
                row.Add(count.ToString());
            }

            table.Add(row);
        }

        return table;
    }
}
