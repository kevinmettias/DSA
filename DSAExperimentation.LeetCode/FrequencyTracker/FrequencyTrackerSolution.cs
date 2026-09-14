using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FrequencyTracker;

// LeetCode 2671. Frequency Tracker: a multiset of numbers supporting Add, DeleteOne
// (remove one occurrence, a no-op when the number is absent) and HasFrequency - "is
// there some number that currently occurs exactly this many times?".
//
// An instance API rather than a pure function, so "every strategy for the problem"
// (section 17.3) takes the form of two implementations behind one shared surface,
// reached through the CreateBy<Strategy> factories below - the same shape
// AllOneDataStructureSolution uses for its own Design-category problem. There is no
// separate "prepare input" step to hoist into a benchmark's [GlobalSetup]; each arm
// constructs its own instance and replays the same call script instead.
internal static class FrequencyTrackerSolution
{
    // The textbook answer: no hashing at all. A raw List<int> multiset, DeleteOne via
    // IndexOf, and HasFrequency sorting a snapshot and walking it once counting run
    // lengths - O(n log n) per query. Deliberately written without this repo's
    // primitives; it is the arm the composed strategy below has to justify itself
    // against.
    public static IFrequencyTracker CreateBySortedScanList() => new SortedScanListFrequencyTracker();

    // This repo's own answer: the classic "frequency of frequencies" trick over two
    // HashMap<TKey,TValue> instances - one holding each number's current count, one
    // holding how many distinct numbers currently sit at each count. Add/DeleteOne
    // move a number's count by one and mirror that move into the second map, so
    // HasFrequency is a single lookup rather than a scan.
    public static IFrequencyTracker CreateByPairedHashMaps() => new PairedHashMapFrequencyTracker();

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without restating
    // it.
    internal interface IFrequencyTracker
    {
        void Add(int number);

        void DeleteOne(int number);

        bool HasFrequency(int frequency);
    }

    private sealed class SortedScanListFrequencyTracker : IFrequencyTracker
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

            return HasRunOfLength(sorted, frequency);
        }

        private static bool HasRunOfLength(int[] sorted, int frequency)
        {
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

    private sealed class PairedHashMapFrequencyTracker : IFrequencyTracker
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
            // A number that was not present sits at count 0, which is not a real
            // bucket - LeetCode only ever asks about frequencies of at least one.
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
