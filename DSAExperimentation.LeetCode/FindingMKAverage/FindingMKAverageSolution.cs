using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.FindingMKAverage;

// LeetCode 1825. Finding MK Average: a stream of integers where calculateMKAverage
// reports the mean of the last m elements after the k smallest and the k largest of
// them are discarded, or -1 before m elements have arrived.
//
// This is a design problem - LeetCode's own shape is a stateful object with two
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// AllOneDataStructureSolution uses for its own design problem.
//
// CreateBySortingSlidingWindow is the textbook baseline the composition below has to
// justify itself against: a List<int> window and a full re-sort on every query.
// CreateByFenwickOrderStatistics keeps the window in this repo's own Queue<int> for
// FIFO eviction and mirrors it into two value-indexed
// FenwickTree<Element, SumOperation<Element>> Binary Indexed Trees (one of counts,
// one of sums), so the trimmed mean is two order-statistic queries instead of a sort.
internal static class FindingMKAverageSolution
{
    // Both ends are trimmed by k, so a window of m keeps m - 2k elements.
    private const int TrimmedEnds = 2;

    // LC 1825 constrains every streamed value to 1 <= num <= 10^5, which is what
    // makes a value-indexed tree possible at all.
    private const int MaxElementValue = 100_000;

    // LeetCode reports -1 until the window has filled.
    private const int WindowNotFull = -1;

    // The textbook answer: a BCL List<int> sliding window, re-sorted from scratch on
    // every query, with the trimmed range summed by index. Deliberately written
    // without this repo's primitives - it is the arm the composed strategy has to
    // beat.
    public static IMKAverage CreateBySortingSlidingWindow(int m, int k) => new SortingSlidingWindowMKAverage(m, k);

    // This repo's own answer: Queue<int> evicts the oldest element, and the two
    // Fenwick trees answer "sum of the smallest t elements currently in the window"
    // in O(log maxValue) - so the MK average is SumOfSmallest(m - k) minus
    // SumOfSmallest(k), with the largest-k side never handled separately at all.
    public static IMKAverage CreateByFenwickOrderStatistics(int m, int k) => new FenwickOrderStatisticsMKAverage(m, k);

    // LeetCode's MKAverage class: the two operations its call script invokes.
    internal interface IMKAverage
    {
        void AddElement(int num);

        int CalculateMKAverage();
    }

    private sealed class SortingSlidingWindowMKAverage : IMKAverage
    {
        private readonly int _m;
        private readonly int _k;
        private readonly List<int> _window;

        public SortingSlidingWindowMKAverage(int m, int k)
        {
            _m = m;
            _k = k;
            _window = new List<int>(m);
        }

        public void AddElement(int num)
        {
            _window.Add(num);

            if (_window.Count > _m)
            {
                _window.RemoveAt(0);
            }
        }

        public int CalculateMKAverage()
        {
            if (_window.Count < _m)
            {
                return WindowNotFull;
            }

            var sorted = _window.OrderBy(value => value).ToArray();
            var sum = 0L;

            for (var i = _k; i < _m - _k; i++)
            {
                sum += sorted[i];
            }

            return (int)(sum / (_m - (TrimmedEnds * _k)));
        }
    }

    private sealed class FenwickOrderStatisticsMKAverage : IMKAverage
    {
        private readonly int _m;
        private readonly int _k;
        private readonly RepoQueue _window = new();
        private readonly FenwickTree<int, SumOperation<int>> _counts = new(MaxElementValue);
        private readonly FenwickTree<long, SumOperation<long>> _sums = new(MaxElementValue);

        public FenwickOrderStatisticsMKAverage(int m, int k)
        {
            _m = m;
            _k = k;
        }

        public void AddElement(int num)
        {
            _window.Enqueue(num);
            _counts.Add(num - 1, 1);
            _sums.Add(num - 1, num);

            if (_window.Count > _m && _window.TryDequeue(out var evicted))
            {
                _counts.Add(evicted - 1, -1);
                _sums.Add(evicted - 1, -evicted);
            }
        }

        public int CalculateMKAverage()
        {
            if (_window.Count < _m)
            {
                return WindowNotFull;
            }

            var smallSum = SumOfSmallest(_k);
            var midPlusSmallSum = SumOfSmallest(_m - _k);

            return (int)((midPlusSmallSum - smallSum) / (_m - (TrimmedEnds * _k)));
        }

        // The count tree's own PrefixQuery is monotonic in the value index, which is
        // exactly BinarySearch.LowerBound's sortedness precondition - so bisecting it
        // finds the value the target-th smallest element sits at, and the sum tree
        // supplies everything strictly below that value in one more prefix query.
        private long SumOfSmallest(int target)
        {
            if (target <= 0)
            {
                return 0;
            }

            var sequence = new FenwickCountSequence(_counts);
            var index = BinarySearch.LowerBound(sequence, target);
            var before = index == 0 ? 0 : _counts.PrefixQuery(index - 1);
            var remainder = target - before;
            var sumBefore = index == 0 ? 0L : _sums.PrefixQuery(index - 1);
            var value = index + 1;

            return sumBefore + ((long)remainder * value);
        }

        // An IRandomAccessSequence<int> view over the count tree's prefix sums - the
        // same "wrap an existing structure in a witness over its own Get" idiom
        // RangeModuleSolution's StartsView and IntervalSet's IntervalEndsView use.
        private readonly struct FenwickCountSequence(FenwickTree<int, SumOperation<int>> counts)
            : IRandomAccessSequence<int>
        {
            public int Length => counts.Count;

            public int Get(int index) => counts.PrefixQuery(index);
        }
    }
}
