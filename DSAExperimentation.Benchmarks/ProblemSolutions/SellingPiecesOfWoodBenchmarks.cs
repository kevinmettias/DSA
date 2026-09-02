using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Selling Pieces of Wood (LC 2312): plain un-memoized (Height, Width) recursion -
// exponential, since the same (Height, Width) sub-piece recurs across many different
// earlier horizontal/vertical cut sequences that land on it - vs. this repo's own
// Memoizer<TState,TResult> caching that exact pair (the same
// MinimumScoreTriangulationOfPolygonBenchmarks/NumberOfWaysOfCuttingAPizzaBenchmarks
// un-memoized-vs-Memoizer shape). Both look up each piece's listed price through this
// repo's own HashMap<TKey,TValue>; only the recursion's caching differs. The
// un-memoized recursion's blow-up is far steeper here than the single-axis interval
// DPs those benchmarks use (two independent cut axes instead of one), so Size is kept
// very modest (<=6) for the same "the baseline's blowup is real" reason those
// benchmarks' own comments give - a size-7 baseline already runs tens of millions of
// calls. Every (h, w) pair up to Size is priced, so nothing short-circuits the naive
// baseline's full branching early.
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

        for (var h = 1; h <= Size; h++)
        {
            for (var w = 1; w <= Size; w++)
            {
                _prices.Set((h, w), random.Next(1, MaxPrice));
            }
        }
    }

    [Benchmark(Baseline = true)]
    public long UnmemoizedRecursion() => BestValue((Size, Size));

    private long BestValue((int Height, int Width) piece)
    {
        var (height, width) = piece;
        _prices.TryGetValue(piece, out var listedPrice);
        long best = listedPrice;

        for (var cut = 1; cut < height; cut++)
        {
            best = Math.Max(best, BestValue((cut, width)) + BestValue((height - cut, width)));
        }

        for (var cut = 1; cut < width; cut++)
        {
            best = Math.Max(best, BestValue((height, cut)) + BestValue((height, width - cut)));
        }

        return best;
    }

    [Benchmark]
    public long MemoizedRecursion()
        => Memoizer.Memoize<(int Height, int Width), long>((Size, Size), BestValueMemoized);

    private long BestValueMemoized(
        (int Height, int Width) piece, Func<(int Height, int Width), long> bestValue)
    {
        var (height, width) = piece;
        _prices.TryGetValue(piece, out var listedPrice);
        long best = listedPrice;

        for (var cut = 1; cut < height; cut++)
        {
            best = Math.Max(best, bestValue((cut, width)) + bestValue((height - cut, width)));
        }

        for (var cut = 1; cut < width; cut++)
        {
            best = Math.Max(best, bestValue((height, cut)) + bestValue((height, width - cut)));
        }

        return best;
    }
}
