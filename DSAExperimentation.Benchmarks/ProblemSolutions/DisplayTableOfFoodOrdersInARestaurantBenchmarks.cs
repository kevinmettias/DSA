using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DisplayTableOfFoodOrdersInARestaurant;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DisplayTableOfFoodOrdersInARestaurantSolution's, the
// same methods DisplayTableOfFoodOrdersInARestaurantSolutionTests proves correct. Tables and
// foods are drawn from small fixed pools so every table sees most foods, keeping the
// rescan arm genuinely O(orders) per cell instead of trivially short-circuiting on an
// empty cell. LC 1418 spells customer and food names in letters and spaces only, so
// each name is a word followed by its index spelled through LowercaseNames.
public class DisplayTableOfFoodOrdersInARestaurantBenchmarks
{
    private const int RandomSeed = 1418; // LC problem number
    private const int TableCount = 30;
    private const int FoodCount = 15;
    private const string FoodPrefix = "Food";
    private const string CustomerPrefix = "Customer";

    private string[][] _orders = [];

    [Params(200, 2_000)]
    public int OrderCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var tables = Enumerable.Range(1, TableCount).Select(table => table.ToString()).ToArray();
        var foods = Enumerable.Range(0, FoodCount).Select(FoodName).ToArray();

        _orders = Enumerable.Range(0, OrderCount)
            .Select(i => new[] { CustomerName(i), tables[random.Next(tables.Length)], foods[random.Next(foods.Length)] })
            .ToArray();
    }

    private static string FoodName(int food) => FoodPrefix + LowercaseNames.Of(food);

    private static string CustomerName(int customer) => CustomerPrefix + LowercaseNames.Of(customer);

    [Benchmark(Baseline = true)]
    public List<List<string>> RescanEveryOrderPerCell() =>
        DisplayTableOfFoodOrdersInARestaurantSolution.DisplayTableByRescanPerCell(_orders);

    [Benchmark]
    public List<List<string>> GroupedHashMapThenMergeSort() =>
        DisplayTableOfFoodOrdersInARestaurantSolution.DisplayTableByGroupedHashMap(_orders);
}
