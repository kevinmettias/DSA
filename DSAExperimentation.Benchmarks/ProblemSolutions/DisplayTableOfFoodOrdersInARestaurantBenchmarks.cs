using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Display Table of Food Orders in a Restaurant (LC 1418): the "translate the
// problem statement literally" approach - gather distinct tables/foods via LINQ,
// then recount each (table, food) cell by rescanning every raw order -
// O(orders * tables * foods) - vs. a single grouped pass into this repo's own
// nested HashMap<int, HashMap<string,int>> (table -> food -> count), with both
// axes sorted via MergeSort over ArrayIndexedSequence - O(orders + tables*foods)
// to fill plus O(tables log tables + foods log foods) to sort, the same "group
// with HashMap, sort with MergeSort" composition AccountsMergeBenchmarks already
// proves for a different problem. Tables/foods are drawn from small fixed pools
// so every table sees most foods, keeping the baseline's rescan genuinely
// O(orders) per cell instead of trivially short-circuiting on an empty cell.
[MemoryDiagnoser]
public class DisplayTableOfFoodOrdersInARestaurantBenchmarks
{
    [Params(200, 2_000)]
    public int OrderCount;

    private string[][] _orders = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1418);
        var tables = Enumerable.Range(1, 30).Select(t => t.ToString()).ToArray();
        var foods = Enumerable.Range(0, 15).Select(f => $"Food{f}").ToArray();

        _orders = Enumerable.Range(0, OrderCount)
            .Select(i => new[] { $"Customer{i}", tables[random.Next(tables.Length)], foods[random.Next(foods.Length)] })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanEveryOrderPerCell()
    {
        var distinctTables = _orders.Select(o => int.Parse(o[1])).Distinct().OrderBy(t => t).ToArray();
        var distinctFoods = _orders.Select(o => o[2]).Distinct().OrderBy(f => f, StringComparer.Ordinal).ToArray();

        var cellCount = 0;

        foreach (var table in distinctTables)
        {
            var tableText = table.ToString();

            foreach (var food in distinctFoods)
            {
                var count = 0;
                foreach (var order in _orders)
                {
                    if (order[1] == tableText && order[2] == food)
                    {
                        count++;
                    }
                }

                cellCount += count;
            }
        }

        return cellCount;
    }

    [Benchmark]
    public int GroupedHashMapThenMergeSort()
    {
        var countsByTable = new HashMap<int, HashMap<string, int>>();
        var foodNames = new HashMap<string, bool>();

        foreach (var order in _orders)
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

        var cellCount = 0;

        foreach (var table in sortedTables)
        {
            countsByTable.TryGetValue(table, out var foodCounts);

            foreach (var food in sortedFoods)
            {
                foodCounts.TryGetValue(food, out var count);
                cellCount += count;
            }
        }

        return cellCount;
    }
}
