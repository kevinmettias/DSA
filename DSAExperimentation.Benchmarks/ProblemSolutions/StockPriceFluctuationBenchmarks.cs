using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StockPriceFluctuation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StockPriceFluctuationSolution's, the same factories
// StockPriceFluctuationTests proves correct. [GlobalSetup] builds the interleaved
// update script - including the corrections that overwrite an already-recorded
// timestamp - so the comparison is between rescanning every stored price on each
// Maximum/Minimum query and the two-heap-with-lazy-deletion strategy that discards a
// superseded entry once, at the query that first surfaces it.
[MemoryDiagnoser]
public class StockPriceFluctuationBenchmarks
{
    private const int RandomSeed = 2034;
    private const int MaxPrice = 1_000_000;

    // Every third update revisits an already-used timestamp as a price
    // correction, so both strategies are forced to handle staleness instead of
    // an all-fresh-timestamps workload making the naive scan look artificially
    // competitive.
    private const int CorrectionStride = 3;

    [Params(200, 2_000)]
    public int UpdateCount;

    private int[] _timestamps = null!;
    private int[] _prices = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _timestamps = new int[UpdateCount];
        _prices = new int[UpdateCount];
        var nextFreshTimestamp = 1;

        for (var i = 0; i < UpdateCount; i++)
        {
            _timestamps[i] = i > 0 && i % CorrectionStride == 0
                ? _timestamps[random.Next(i)]
                : nextFreshTimestamp++;
            _prices[i] = random.Next(1, MaxPrice);
        }
    }

    [Benchmark(Baseline = true)]
    public int FullScanEveryQuery() => Replay(StockPriceFluctuationSolution.CreateByFullScan());

    [Benchmark]
    public int LazyDeletionTwoHeaps() => Replay(StockPriceFluctuationSolution.CreateByLazyDeletionTwoHeaps());

    private int Replay(StockPriceFluctuationSolution.IStockPrice stockPrice)
    {
        var lastMax = 0;
        var lastMin = 0;

        for (var i = 0; i < _timestamps.Length; i++)
        {
            stockPrice.Update(_timestamps[i], _prices[i]);
            lastMax = stockPrice.Maximum();
            lastMin = stockPrice.Minimum();
        }

        return stockPrice.Current() + lastMax + lastMin;
    }
}
