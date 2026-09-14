using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.StockPriceFluctuation;

// LeetCode 2034. Stock Price Fluctuation: a stream of (timestamp, price) records in
// which a later record for an already-seen timestamp is a CORRECTION that supersedes
// the earlier price, plus queries for the latest, maximum and minimum current price.
//
// This is a design problem - LeetCode's own shape is a stateful object with four
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// DetectSquaresSolution uses for its own design problem. The whole difficulty is the
// correction: an extremum that was true before a correction may be stale afterwards,
// and neither strategy can afford to search for the record it superseded.
internal static class StockPriceFluctuationSolution
{
    // Composed: this repo's own HashMap<int,int> holds the authoritative price per
    // timestamp (so a correction is a single overwrite), while two
    // Heap<(int Price, int Timestamp), TOrder> instances - the same two-heap shape
    // FindMedianFromDataStream uses, one MaxHeapOrder, one MinHeapOrder - track
    // candidate extremes with LAZY DELETION. A superseded entry is never removed when
    // the correction happens; instead a root is only trusted once its timestamp's
    // HashMap entry still matches the price it was pushed with, so each stale entry is
    // discarded once, at the query that first surfaces it.
    public static IStockPrice CreateByLazyDeletionTwoHeaps() => new LazyDeletionTwoHeapsStockPrice();

    // The textbook answer: keep timestamp -> price and rescan every stored price on
    // each Maximum/Minimum query. Deliberately written without this repo's primitives
    // - a BCL Dictionary and a LINQ pass - because it is the arm the two-heap strategy
    // has to justify itself against. Corrections cost nothing here and queries cost
    // O(n) each, exactly the trade the heaps invert.
    public static IStockPrice CreateByFullScan() => new FullScanStockPrice();

    // LeetCode's own four operations. An extremum query on an empty stream reports 0;
    // LeetCode guarantees at least one update before any query.
    internal interface IStockPrice
    {
        void Update(int timestamp, int price);

        int Current();

        int Maximum();

        int Minimum();
    }

    private sealed class LazyDeletionTwoHeapsStockPrice : IStockPrice
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

        // A heap entry is stale when the timestamp it was pushed for has since been
        // corrected to a different price. Entries for a timestamp that is still
        // current stay trusted no matter how many other corrections happened.
        private bool IsStale((int Price, int Timestamp) entry)
            => _priceAtTimestamp.TryGetValue(entry.Timestamp, out var currentPrice) && currentPrice != entry.Price;
    }

    private sealed class FullScanStockPrice : IStockPrice
    {
        private readonly Dictionary<int, int> _priceAtTimestamp = [];
        private int _latestTimestamp;

        public void Update(int timestamp, int price)
        {
            _priceAtTimestamp[timestamp] = price;
            _latestTimestamp = Math.Max(_latestTimestamp, timestamp);
        }

        public int Current()
        {
            _priceAtTimestamp.TryGetValue(_latestTimestamp, out var price);
            return price;
        }

        public int Maximum() => _priceAtTimestamp.Values.DefaultIfEmpty().Max();

        public int Minimum() => _priceAtTimestamp.Values.DefaultIfEmpty().Min();
    }
}
