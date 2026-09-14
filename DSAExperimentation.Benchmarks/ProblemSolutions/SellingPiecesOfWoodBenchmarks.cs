using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.LeetCode.SellingPiecesOfWood;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SellingPiecesOfWoodSolution's, the same methods
// SellingPiecesOfWoodTests proves agree. They share one recurrence and differ only
// in whether recursive calls go through Memoizer's cache, so the measurement
// isolates memoization itself - the same un-memoized-vs-Memoizer shape
// MinimumScoreTriangulationOfPolygonBenchmarks and NumberOfWaysOfCuttingAPizzaBenchmarks
// already use.
//
// The un-memoized baseline's blow-up is far steeper here than in those single-axis
// interval DPs (two independent cut axes instead of one), so Size stays very modest
// (<= 6): a size-7 baseline already runs tens of millions of calls. Every (h, w)
// pair up to Size is priced, so nothing short-circuits its full branching early.
//
// Each arm is handed the prepared price index its hoisted overload takes, so
// building the HashMap is charged to [GlobalSetup] rather than to the search.
[MemoryDiagnoser]
public class SellingPiecesOfWoodBenchmarks
{
    private const int RandomSeed = 2312; // LC problem number
    private const int MaxPrice = 50;

    [Params(4, 6)]
    public int Size;

    private HashMap<(int Height, int Width), int> _prices = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _prices = new HashMap<(int Height, int Width), int>();

        for (var height = 1; height <= Size; height++)
        {
            for (var width = 1; width <= Size; width++)
            {
                var price = random.Next(1, MaxPrice);

                _prices.Set((height, width), price);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public long UnmemoizedRecursion() =>
        SellingPiecesOfWoodSolution.SellingWoodByUnmemoizedRecursion(Size, Size, _prices);

    [Benchmark]
    public long MemoizedRecursion() =>
        SellingPiecesOfWoodSolution.SellingWoodByMemoizedRecursion(Size, Size, _prices);
}
