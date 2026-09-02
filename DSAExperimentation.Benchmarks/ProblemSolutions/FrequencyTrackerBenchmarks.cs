using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Frequency Tracker (LC 2671): SortedScanList is the "no hashing at all" baseline -
// a raw List<int> multiset, DeleteOne finds its target via IndexOf, and HasFrequency
// sorts a snapshot and walks it once counting run-lengths - vs. RepoHashMap, which
// pairs two of this repo's own HashMap<TKey,TValue> instances (a number's current
// count, and how many distinct numbers currently sit at each frequency) so
// Add/DeleteOne are O(1) amortized and HasFrequency is a single map lookup, the same
// "no hashing at all" vs. HashMap<TKey,TValue> contrast DesignHashMapBenchmarks
// already establishes for LC 706.
[MemoryDiagnoser]
public class FrequencyTrackerBenchmarks
{
    private const int Seed = 2671; // LC problem number
    private const int MinQueryCount = 20;
    private const int QueryCountDivisor = 10;
    private const int DeleteCountDivisor = 4;

    [Params(200, 5_000)]
    public int Length;

    private int[] _numbersToAdd = null!;
    private int[] _numbersToDelete = null!;
    private int[] _frequenciesToQuery = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var valueRange = Math.Max(1, Length / DeleteCountDivisor);

        _numbersToAdd = Enumerable.Range(0, Length).Select(_ => random.Next(valueRange)).ToArray();
        _numbersToDelete = _numbersToAdd.Take(Length / DeleteCountDivisor).ToArray();

        var queryCount = Math.Max(MinQueryCount, Length / QueryCountDivisor);
        _frequenciesToQuery = Enumerable.Range(0, queryCount).Select(_ => random.Next(1, valueRange + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SortedScanList()
    {
        var tracker = new ListFrequencyTracker();
        return RunWorkload(tracker.Add, tracker.DeleteOne, tracker.HasFrequency);
    }

    [Benchmark]
    public int RepoHashMap()
    {
        var tracker = new HashMapFrequencyTracker();
        return RunWorkload(tracker.Add, tracker.DeleteOne, tracker.HasFrequency);
    }

    private int RunWorkload(Action<int> add, Action<int> deleteOne, Func<int, bool> hasFrequency)
    {
        foreach (var number in _numbersToAdd)
        {
            add(number);
        }

        foreach (var number in _numbersToDelete)
        {
            deleteOne(number);
        }

        var trueCount = 0;
        foreach (var frequency in _frequenciesToQuery)
        {
            if (hasFrequency(frequency))
            {
                trueCount++;
            }
        }

        return trueCount;
    }

    private sealed class ListFrequencyTracker
    {
        private readonly List<int> _values = [];

        public void Add(int number) => _values.Add(number);

        public void DeleteOne(int number)
        {
            var index = _values.IndexOf(number);
            if (index >= 0)
            {
                _values.RemoveAt(index);
            }
        }

        public bool HasFrequency(int frequency)
        {
            if (_values.Count == 0)
            {
                return false;
            }

            var sorted = _values.ToArray();
            Array.Sort(sorted);

            var runLength = 1;
            for (var i = 1; i < sorted.Length; i++)
            {
                if (sorted[i] == sorted[i - 1])
                {
                    runLength++;
                    continue;
                }

                if (runLength == frequency)
                {
                    return true;
                }

                runLength = 1;
            }

            return runLength == frequency;
        }
    }

    private sealed class HashMapFrequencyTracker
    {
        private readonly HashMap<int, int> _countByNumber = new();
        private readonly HashMap<int, int> _countByFrequency = new();

        public void Add(int number)
        {
            _countByNumber.TryGetValue(number, out var oldCount);
            DecrementFrequencyBucket(oldCount);

            var newCount = oldCount + 1;
            _countByNumber.Set(number, newCount);
            IncrementFrequencyBucket(newCount);
        }

        public void DeleteOne(int number)
        {
            if (!_countByNumber.TryGetValue(number, out var oldCount) || oldCount == 0)
            {
                return;
            }

            DecrementFrequencyBucket(oldCount);

            var newCount = oldCount - 1;
            if (newCount == 0)
            {
                _countByNumber.TryRemove(number);
            }
            else
            {
                _countByNumber.Set(number, newCount);
                IncrementFrequencyBucket(newCount);
            }
        }

        public bool HasFrequency(int frequency)
            => _countByFrequency.TryGetValue(frequency, out var count) && count > 0;

        private void IncrementFrequencyBucket(int frequency)
        {
            _countByFrequency.TryGetValue(frequency, out var count);
            _countByFrequency.Set(frequency, count + 1);
        }

        private void DecrementFrequencyBucket(int frequency)
        {
            if (frequency == 0)
            {
                return;
            }

            if (_countByFrequency.TryGetValue(frequency, out var count) && count > 0)
            {
                _countByFrequency.Set(frequency, count - 1);
            }
        }
    }
}
