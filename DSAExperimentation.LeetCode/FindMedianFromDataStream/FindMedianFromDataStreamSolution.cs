using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.FindMedianFromDataStream;

// LeetCode 295. Find Median from Data Stream: a design problem - LeetCode's own
// shape is a stateful object with AddNum/FindMedian operations, not a single
// return value - so the strategy choice is which implementation backs it, the
// same shape LRUCacheSolution uses for its own design problem (LC 146).
//
// IMedianFinder is bespoke to this problem alone - no other LeetCode entry shares
// an AddNum/FindMedian contract - so it stays here rather than in DataStructures/.
internal static class FindMedianFromDataStreamSolution
{
    // The textbook baseline this composition has to justify itself against: a
    // BCL List<int>, re-sorted from scratch on every FindMedian call.
    public static IMedianFinder CreateBySortOnEveryQuery() => new SortedListMedianFinder();

    // The composed answer: two of this repo's own Heap<T,TOrder> instances - a
    // MaxHeapOrder<int> heap holding the smaller half, a MinHeapOrder<int> heap
    // holding the larger half, rebalanced after every insert so their roots always
    // straddle the median - the same "compose a repo primitive twice" move
    // MinStackSolution makes with two Stack<int> instances.
    public static IMedianFinder CreateByTwoHeaps() => new TwoHeapMedianFinder();

    private sealed class SortedListMedianFinder : IMedianFinder
    {
        private const int MedianSplit = 2;
        private const double MedianAverageDivisor = 2.0;

        private readonly List<int> _values = [];

        public void AddNum(int num) => _values.Add(num);

        public double FindMedian()
        {
            var sorted = _values.ToArray();
            Array.Sort(sorted);

            var mid = sorted.Length / MedianSplit;
            var hasEvenLength = sorted.Length % MedianSplit == 0;

            return hasEvenLength
                ? AverageOfTwoMiddleValues(sorted, mid)
                : MiddleValue(sorted, mid);
        }

        private static double AverageOfTwoMiddleValues(int[] sorted, int mid) =>
            (sorted[mid - 1] + sorted[mid]) / MedianAverageDivisor;

        private static double MiddleValue(int[] sorted, int mid) => sorted[mid];
    }

    private sealed class TwoHeapMedianFinder : IMedianFinder
    {
        private const double MedianAverageDivisor = 2.0;

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

        public double FindMedian() =>
            _lowerHalf.Count > _upperHalf.Count
                ? PeekLower()
                : AverageOfHalves();

        private double AverageOfHalves() => (PeekLower() + PeekUpper()) / MedianAverageDivisor;

        private int PeekUpper()
        {
            _upperHalf.TryPeek(out var value);
            return value;
        }

        private int PeekLower()
        {
            _lowerHalf.TryPeek(out var value);
            return value;
        }
    }
}
