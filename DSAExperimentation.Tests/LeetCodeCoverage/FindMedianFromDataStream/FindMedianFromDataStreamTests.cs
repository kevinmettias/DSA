using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMedianFromDataStream;

// LeetCode 295. Find Median from Data Stream: two of this repo's own
// Heap<T,TOrder> instances - a MaxHeapOrder<int> heap holding the smaller half, a
// MinHeapOrder<int> heap holding the larger half, rebalanced after every insert so
// their roots always straddle the median - the classic two-heap approach, composing
// Heap<T,TOrder> twice the way MinStackTests.cs composes two Stack<int> instances.
public sealed class FindMedianFromDataStreamTests
{
    [Fact]
    public void AddNumFindMedian_LeetCodeExample_TracksRunningMedian()
    {
        var medianFinder = new MedianFinderOperations();

        medianFinder.AddNum(1);
        medianFinder.AddNum(2);
        Assert.Equal(1.5, medianFinder.FindMedian());

        medianFinder.AddNum(3);
        Assert.Equal(2.0, medianFinder.FindMedian());
    }

    [Fact]
    public void AddNumFindMedian_DescendingInsertOrder_StillTracksRunningMedian()
    {
        var medianFinder = new MedianFinderOperations();

        medianFinder.AddNum(5);
        medianFinder.AddNum(4);
        medianFinder.AddNum(3);
        medianFinder.AddNum(2);
        medianFinder.AddNum(1);

        Assert.Equal(3.0, medianFinder.FindMedian());
    }

    private sealed class MedianFinderOperations
    {
        private readonly Heap<int, MaxHeapOrder<int>> _lowerHalf = new();
        private readonly Heap<int, MinHeapOrder<int>> _upperHalf = new();

        public void AddNum(int num)
        {
            if (_lowerHalf.Count == 0 || num <= PeekLower())
            {
                _lowerHalf.Push(num);
            }
            else
            {
                _upperHalf.Push(num);
            }

            Rebalance();
        }

        public double FindMedian()
            => _lowerHalf.Count > _upperHalf.Count
                ? PeekLower()
                : (PeekLower() + PeekUpper()) / 2.0;

        private void Rebalance()
        {
            if (_lowerHalf.Count > _upperHalf.Count + 1)
            {
                _lowerHalf.TryPop(out var moved);
                _upperHalf.Push(moved);
            }
            else if (_upperHalf.Count > _lowerHalf.Count)
            {
                _upperHalf.TryPop(out var moved);
                _lowerHalf.Push(moved);
            }
        }

        private int PeekLower()
        {
            _lowerHalf.TryPeek(out var value);
            return value;
        }

        private int PeekUpper()
        {
            _upperHalf.TryPeek(out var value);
            return value;
        }
    }
}
