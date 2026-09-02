using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Finding MK Average (LC 1825): a brute-force baseline (a List<int> sliding window,
// re-sorted from scratch on every calculateMKAverage call) vs. this repo's own
// Queue<int> for FIFO eviction plus two value-indexed FenwickTree<Element,
// SumOperation<Element>> Binary Indexed Trees (one of counts, one of sums), with the
// sum of the k smallest elements found via BinarySearch.LowerBound over a monotonic
// view of the count tree's PrefixQuery - the same idea FindingMKAverageTests uses.
// Both approaches replay the exact same addElement stream.
[MemoryDiagnoser]
public class FindingMKAverageBenchmarks
{
    private const int WindowSize = 99;
    private const int K = 33;
    private const int MiddleTrimMultiplier = 2;

    // LC problem number, reused as the deterministic stream seed.
    private const int RandomSeed = 1825;
    private const int MaxElementValue = 100_000;

    [Params(500, 5_000)]
    public int Length;

    private int[] _stream = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stream = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long SortingSlidingWindow()
    {
        var window = new List<int>(WindowSize + 1);
        var checksum = 0L;

        foreach (var value in _stream)
        {
            window.Add(value);

            if (window.Count > WindowSize)
            {
                window.RemoveAt(0);
            }

            if (window.Count == WindowSize)
            {
                checksum += SumMiddleElements(window);
            }
        }

        return checksum;
    }

    private static long SumMiddleElements(List<int> window)
    {
        var sorted = window.OrderBy(v => v).ToArray();
        var sum = 0L;

        for (var i = K; i < WindowSize - K; i++)
        {
            sum += sorted[i];
        }

        return sum / (WindowSize - (MiddleTrimMultiplier * K));
    }

    [Benchmark]
    public long FenwickOrderStatistics()
    {
        var mkAverage = new MKAverage(WindowSize, K);
        var checksum = 0L;

        foreach (var value in _stream)
        {
            mkAverage.AddElement(value);
            var result = mkAverage.CalculateMKAverage();
            checksum += result < 0 ? 0 : result;
        }

        return checksum;
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

            return (int)((midPlusSmallSum - smallSum) / (_m - (MiddleTrimMultiplier * _k)));
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
