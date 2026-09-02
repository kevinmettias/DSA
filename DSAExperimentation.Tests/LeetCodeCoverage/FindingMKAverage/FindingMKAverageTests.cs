using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindingMKAverage;

// LeetCode 1825. Finding MK Average: a sliding window of the last m stream elements
// (this repo's own Queue<int> for FIFO eviction) plus two value-indexed
// FenwickTree<Element,SumOperation<Element>> (Binary Indexed Trees, one over counts,
// one over sums) tracking the window's multiset by value. The sum of the smallest
// `target` elements is found by BinarySearch.LowerBound walking a monotonic view of
// the count tree's own PrefixQuery (via a small IRandomAccessSequence<int> adapter) -
// the same coordinate-indexed-Fenwick idea CountOfSmallerNumbersAfterSelfTests uses,
// generalized from "count smaller" to "sum of k smallest". The MK-average itself
// reduces to SumOfSmallest(m-k) - SumOfSmallest(k), avoiding any separate handling of
// the largest-k side entirely.
public sealed partial class FindingMKAverageTests
{
    [Fact]
    public void MKAverage_LeetCodeExampleSequence_MatchesExpectedResults()
    {
        var mkAverage = new MKAverage(3, 1);

        mkAverage.AddElement(3);
        mkAverage.AddElement(1);
        Assert.Equal(-1, mkAverage.CalculateMKAverage());

        mkAverage.AddElement(10);
        Assert.Equal(3, mkAverage.CalculateMKAverage());

        mkAverage.AddElement(5);
        mkAverage.AddElement(5);
        Assert.Equal(5, mkAverage.CalculateMKAverage());
    }

    [Fact]
    public void CalculateMKAverage_BeforeWindowFills_ReturnsNegativeOne()
    {
        var mkAverage = new MKAverage(99, 33);

        mkAverage.AddElement(42);

        Assert.Equal(-1, mkAverage.CalculateMKAverage());
    }

    [Fact]
    public void CalculateMKAverage_AllElementsEqual_ReturnsThatValue()
    {
        var mkAverage = new MKAverage(6, 1);

        for (var i = 0; i < 6; i++)
        {
            mkAverage.AddElement(7);
        }

        Assert.Equal(7, mkAverage.CalculateMKAverage());
    }

    [Fact]
    public void AddElement_PastWindowSize_EvictsOldestElementFirst()
    {
        // Window size 3, k=1: [1,2,3] -> middle element 2. Adding 100 evicts the
        // oldest (1), leaving [2,3,100] -> middle element 3, not 2 - proving eviction
        // is FIFO (oldest-first), not smallest/largest-first.
        var mkAverage = new MKAverage(3, 1);

        mkAverage.AddElement(1);
        mkAverage.AddElement(2);
        mkAverage.AddElement(3);
        mkAverage.AddElement(100);

        Assert.Equal(3, mkAverage.CalculateMKAverage());
    }

    private readonly struct FenwickCountSequence(FenwickTree<int, SumOperation<int>> counts) : IRandomAccessSequence<int>
    {
        public int Length => counts.Count;

        public int Get(int index) => counts.PrefixQuery(index);
    }

    private sealed class MKAverage
    {
        private const int MaxValue = 100_000;

        private readonly int _m;
        private readonly int _k;
        private readonly RepoQueue _window = new();
        private readonly FenwickTree<int, SumOperation<int>> _counts = new(MaxValue);
        private readonly FenwickTree<long, SumOperation<long>> _sums = new(MaxValue);

        public MKAverage(int m, int k)
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
                return -1;
            }

            var smallSum = SumOfSmallest(_k);
            var midPlusSmallSum = SumOfSmallest(_m - _k);

            return (int)((midPlusSmallSum - smallSum) / (_m - (2 * _k)));
        }

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
    }
}
