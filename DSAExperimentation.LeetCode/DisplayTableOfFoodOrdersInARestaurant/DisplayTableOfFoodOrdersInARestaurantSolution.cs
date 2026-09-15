using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.DisplayTableOfFoodOrdersInARestaurant;

// LeetCode 1418. Display Table of Food Orders in a Restaurant: turn raw
// (customer, table, food) orders into a display table whose header row is "Table"
// followed by every ordered food in alphabetical order, then one row per table in
// increasing numeric order holding that table's count of each food.
//
// Both strategies agree on the two sort orders LeetCode's answer fixes: food names
// compare ordinally (a culture-aware Comparer<string>.Default can disagree with
// LeetCode's expected byte-order sort on some locales, the same reasoning
// AccountsMerge records) and table numbers compare as integers, not as text.
//
// The strategies differ only in how a cell's count is obtained: rescanning every
// raw order once per (table, food) cell, or a single grouped pass into a nested
// HashMap<int, HashMap<string, int>>.
internal static class DisplayTableOfFoodOrdersInARestaurantSolution
{
    private const int TableFieldIndex = 1;
    private const int FoodFieldIndex = 2;
    private const string TableHeader = "Table";

    // The textbook answer: gather the distinct tables and foods, then fill each
    // cell by counting matching orders over a fresh pass of the whole input -
    // O(orders * tables * foods). Deliberately BCL throughout; it is the arm the
    // grouped strategy below has to justify itself against.
    public static List<List<string>> DisplayTableByRescanPerCell(string[][] orders)
    {
        var tableNumbers = orders
            .Select(order => int.Parse(order[TableFieldIndex]))
            .Distinct()
            .OrderBy(tableNumber => tableNumber)
            .ToArray();

        var foods = orders
            .Select(order => order[FoodFieldIndex])
            .Distinct()
            .OrderBy(food => food, StringComparer.Ordinal)
            .ToArray();

        var table = new List<List<string>> { HeaderRow(foods) };

        foreach (var tableNumber in tableNumbers)
        {
            var row = RescannedRow(orders, tableNumber, foods);
            table.Add(row);
        }

        return table;
    }

    private static List<string> RescannedRow(string[][] orders, int tableNumber, string[] foods)
    {
        var tableText = tableNumber.ToString();
        var row = new List<string> { tableText };

        foreach (var food in foods)
        {
            row.Add(CountOrdersForCell(orders, new TableLabel(tableText), new FoodName(food)).ToString());
        }

        return row;
    }

    private static int CountOrdersForCell(string[][] orders, TableLabel tableLabel, FoodName food)
    {
        var count = 0;

        foreach (var order in orders)
        {
            if (order[TableFieldIndex] == tableLabel.Text && order[FoodFieldIndex] == food.Text)
            {
                count++;
            }
        }

        return count;
    }

    // One grouped pass into this repo's own nested HashMap (table -> food -> count),
    // with food names deduped through a second HashMap<string, bool> and both axes
    // sorted by MergeSort over ArrayIndexedSequence - the same "group with HashMap,
    // sort with MergeSort" composition AccountsMerge uses, aggregating counts
    // instead of deduping strings. O(orders + tables*foods) to fill, plus
    // O(tables log tables + foods log foods) to sort.
    public static List<List<string>> DisplayTableByGroupedHashMap(string[][] orders)
    {
        var tally = TallyOrders(orders);
        var sortedFoods = SortedFoodNames(tally.FoodNames);
        var sortedTables = SortedTableNumbers(tally.CountsByTable);

        var table = new List<List<string>> { HeaderRow(sortedFoods) };

        foreach (var tableNumber in sortedTables)
        {
            var row = GroupedRow(tally.CountsByTable, tableNumber, sortedFoods);
            table.Add(row);
        }

        return table;
    }

    // One pass's worth of grouped state: the per-table food counts that fill the
    // cells, and the set of food names that fixes the column axis.
    private readonly record struct OrderTally(
        HashMap<int, HashMap<string, int>> CountsByTable,
        HashMap<string, bool> FoodNames);

    private static OrderTally TallyOrders(string[][] orders)
    {
        var countsByTable = new HashMap<int, HashMap<string, int>>();
        var foodNames = new HashMap<string, bool>();

        foreach (var order in orders)
        {
            RecordOrder(order, countsByTable, foodNames);
        }

        return new OrderTally(countsByTable, foodNames);
    }

    private static void RecordOrder(
        string[] order, HashMap<int, HashMap<string, int>> countsByTable, HashMap<string, bool> foodNames)
    {
        var tableNumber = int.Parse(order[TableFieldIndex]);
        var foodItem = order[FoodFieldIndex];

        foodNames.Set(foodItem, true);

        if (!countsByTable.TryGetValue(tableNumber, out var foodCounts))
        {
            foodCounts = new HashMap<string, int>();
            countsByTable.Set(tableNumber, foodCounts);
        }

        foodCounts.TryGetValue(foodItem, out var count);
        foodCounts.Set(foodItem, count + 1);
    }

    private static string[] SortedFoodNames(HashMap<string, bool> foodNames)
    {
        var sortedFoods = foodNames.Keys.ToArray();
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(
            new ArrayIndexedSequence<string>(sortedFoods), StringComparer.Ordinal);

        return sortedFoods;
    }

    private static int[] SortedTableNumbers(HashMap<int, HashMap<string, int>> countsByTable)
    {
        var sortedTables = countsByTable.Keys.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedTables));

        return sortedTables;
    }

    private static List<string> GroupedRow(
        HashMap<int, HashMap<string, int>> countsByTable, int tableNumber, string[] sortedFoods)
    {
        countsByTable.TryGetValue(tableNumber, out var foodCounts);
        var row = new List<string> { tableNumber.ToString() };

        foreach (var food in sortedFoods)
        {
            foodCounts.TryGetValue(food, out var count);
            row.Add(count.ToString());
        }

        return row;
    }

    // Shared by both strategies: the header is pure formatting of an already-sorted
    // food axis, not part of either counting technique.
    private static List<string> HeaderRow(string[] sortedFoods)
    {
        var header = new List<string> { TableHeader };
        header.AddRange(sortedFoods);

        return header;
    }
}
