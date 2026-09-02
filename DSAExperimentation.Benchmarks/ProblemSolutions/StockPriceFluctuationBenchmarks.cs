using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stock Price Fluctuation (LC 2034): a naive baseline replaying the raw
// HashMap<int,int> of timestamp->price and doing an O(n) full-dictionary scan on
// every Maximum/Minimum query, vs this repo's own two-heap-with-lazy-deletion
// approach (HashMap for the authoritative price per timestamp, a MaxHeapOrder heap
// and a MinHeapOrder heap of (Price, Timestamp) candidates, stale roots discarded
// against the HashMap before trusting a peek). Both replay the identical
// interleaved Update/Maximum/Minimum script, including corrections that overwrite
// an already-pushed timestamp.
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
    public int FullScanEveryQuery()
    {
        var priceAtTimestamp = new HashMap<int, int>();
        var latestTimestamp = 0;
        var lastMax = 0;
        var lastMin = 0;

        for (var i = 0; i < _timestamps.Length; i++)
        {
            priceAtTimestamp.Set(_timestamps[i], _prices[i]);
            latestTimestamp = Math.Max(latestTimestamp, _timestamps[i]);

            lastMax = ScanForMaximum(priceAtTimestamp, _timestamps, i);
            lastMin = ScanForMinimum(priceAtTimestamp, _timestamps, i);
        }

        priceAtTimestamp.TryGetValue(latestTimestamp, out var current);
        return current + lastMax + lastMin;
    }

    private static int ScanForMaximum(HashMap<int, int> priceAtTimestamp, int[] timestamps, int upToIndex)
    {
        var max = int.MinValue;

        for (var i = 0; i <= upToIndex; i++)
        {
            priceAtTimestamp.TryGetValue(timestamps[i], out var price);
            max = Math.Max(max, price);
        }

        return max;
    }

    private static int ScanForMinimum(HashMap<int, int> priceAtTimestamp, int[] timestamps, int upToIndex)
    {
        var min = int.MaxValue;

        for (var i = 0; i <= upToIndex; i++)
        {
            priceAtTimestamp.TryGetValue(timestamps[i], out var price);
            min = Math.Min(min, price);
        }

        return min;
    }

    [Benchmark]
    public int LazyDeletionTwoHeaps()
    {
        var stockPrice = new StockPriceOperations();
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

    private sealed class StockPriceOperations
    {
        private readonly HashMap<int, int> _priceAtTimestamp = new();
        private readonly Heap<(int Price, int Timestamp), MaxHeapOrder<(int, int)>> _maxHeap = new();
        private readonly Heap<(int Price, int Timestamp), MinHeapOrder<(int, int)>> _minHeap = new();
        private int _latestTimestamp;

        public void Update(int timestamp, int price)
        {
            _priceAtTimestamp.Set(timestamp, price);
            _maxHeap.Push((price, timestamp));
            _minHeap.Push((price, timestamp));
            _latestTimestamp = Math.Max(_latestTimestamp, timestamp);
        }

        public int Current()
        {
            _priceAtTimestamp.TryGetValue(_latestTimestamp, out var price);
            return price;
        }

        public int Maximum()
        {
            while (_maxHeap.TryPeek(out var top) && IsStale(top))
            {
                _maxHeap.TryPop(out _);
            }

            _maxHeap.TryPeek(out var current);
            return current.Price;
        }

        public int Minimum()
        {
            while (_minHeap.TryPeek(out var top) && IsStale(top))
            {
                _minHeap.TryPop(out _);
            }

            _minHeap.TryPeek(out var current);
            return current.Price;
        }

        private bool IsStale((int Price, int Timestamp) entry)
            => _priceAtTimestamp.TryGetValue(entry.Timestamp, out var currentPrice) && currentPrice != entry.Price;
    }
}
