using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.SellingPiecesOfWood;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SellingPiecesOfWoodSolution's, the same methods
// SellingPiecesOfWoodSolutionTests proves agree. They share one recurrence and differ only
// in whether recursive calls go through Memoizer's cache, so the measurement
// isolates memoization itself - the same un-memoized-vs-Memoizer shape
// MinimumScoreTriangulationOfPolygonBenchmarks and NumberOfWaysOfCuttingAPizzaBenchmarks
// already use. Every (h, w) pair up to the board size is priced, so nothing
// short-circuits the baseline's full branching early.
//
// Sizes are per arm. The un-memoized baseline's blow-up is far steeper here than in
// those single-axis interval DPs (two independent cut axes instead of one), so it
// stops at a 6 x 6 board: size 7 already runs tens of millions of calls. The memoized
// arm's O(size^3) runs on to 80, inside LC 2312's bound of 2 * 10^4 prices that
// pricing every pair reaches near 141. The two are compared at the sizes both run.
//
// Each arm is handed the prepared price index its hoisted overload takes, so
// building the HashMap is charged to [GlobalSetup] rather than to the search.
public class SellingPiecesOfWoodBenchmarks
{
    private const int RandomSeed = 2312; // LC problem number
    private const int MaxPrice = 50;

    private Dictionary<int, HashMap<(int Height, int Width), int>> _pricesBySize = [];

    public static IEnumerable<int> BaselineSizes => [4, 6];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 20, 80];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _pricesBySize = MemoizedSizes.ToDictionary(size => size, BuildPrices);

    private static HashMap<(int Height, int Width), int> BuildPrices(int size)
    {
        var random = new Random(RandomSeed);
        var prices = new HashMap<(int Height, int Width), int>();

        for (var height = 1; height <= size; height++)
        {
            for (var width = 1; width <= size; width++)
            {
                var price = random.Next(1, MaxPrice);

                prices.Set((height, width), price);
            }
        }

        return prices;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public long UnmemoizedRecursion(int size) =>
        SellingPiecesOfWoodSolution.SellingWoodByUnmemoizedRecursion(size, size, _pricesBySize[size]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public long MemoizedRecursion(int size) =>
        SellingPiecesOfWoodSolution.SellingWoodByMemoizedRecursion(size, size, _pricesBySize[size]);
}
