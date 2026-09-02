using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StockPriceFluctuation;

// LeetCode 2034. Stock Price Fluctuation: this repo's own HashMap<int,int> holds
// the authoritative price per timestamp (so a correction just overwrites it), while
// two Heap<(int Price, int Timestamp), TOrder> instances - the same two-heap shape
// FindMedianFromDataStreamTests already uses, one MaxHeapOrder, one MinHeapOrder -
// track candidate maximum/minimum prices with lazy deletion: a heap root is only
// trusted once its timestamp's HashMap entry still matches the price it was pushed
// with, discarding every stale (superseded-by-correction) entry first.
public sealed partial class StockPriceFluctuationTests
{
    [Fact]
    public void Update_LeetCodeExampleSequence_TracksCurrentMaximumAndMinimum()
    {
        var stockPrice = new StockPriceOperations();

        stockPrice.Update(1, 10);
        stockPrice.Update(2, 5);
        Assert.Equal(5, stockPrice.Current());
        Assert.Equal(10, stockPrice.Maximum());

        stockPrice.Update(1, 3);
        Assert.Equal(5, stockPrice.Maximum());

        stockPrice.Update(4, 2);
        Assert.Equal(2, stockPrice.Minimum());

        stockPrice.Update(4, 2);
        Assert.Equal(2, stockPrice.Minimum());
    }

    [Fact]
    public void Maximum_AfterCorrectionMakesOldRootStale_SkipsStaleHeapEntries()
    {
        var stockPrice = new StockPriceOperations();

        stockPrice.Update(1, 100);
        stockPrice.Update(2, 20);
        Assert.Equal(100, stockPrice.Maximum());

        stockPrice.Update(1, 1);
        Assert.Equal(20, stockPrice.Maximum());
        Assert.Equal(1, stockPrice.Minimum());
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
