using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MyCalendarIII;

// LeetCode 732. My Calendar III: Book(start, end) never rejects a booking - it
// returns the maximum number of events that have ever overlapped at any single
// point in time, across every booking made so far (including this one).
//
// This is a "design" problem: the published interface is a stateful class
// replayed across a sequence of Book calls, so the strategies here are factory
// methods returning that stateful object, the same shape MyCalendarISolution
// (LC 729) and MyCalendarIISolution (LC 731) use.
internal static class MyCalendarIIISolution
{
    public interface ICalendar
    {
        int Book(int start, int end);
    }

    // Real answer: this repo's own HashMap<int,int> to accumulate a +1 at every
    // booking's start and a -1 at its end (the standard technique for turning
    // "how many intervals overlap right now" into a prefix-sum sweep over event
    // points), then this repo's own MergeSort.Sort over an
    // ArrayIndexedSequence<int> to walk those delta keys in ascending order on
    // every Book call - the same "no ordered-map primitive, so sort the keys
    // instead of maintaining one" substitution RangeModuleTests already makes
    // for a real TreeMap/NavigableMap. The running prefix sum's maximum after
    // folding this booking's own +1/-1 in is exactly the "k" LeetCode wants
    // back from Book.
    public static ICalendar CreateByHashMapMergeSortSweep() => new HashMapMergeSortSweepCalendar();

    // Baseline: the textbook brute force - on every booking, rescan every start
    // point ever seen against every booking recorded so far to find the worst
    // simultaneous overlap, O(n) event points times O(n) bookings per call.
    // Deliberately written without this repo's own primitives - the arm
    // CreateByHashMapMergeSortSweep has to justify itself against.
    public static ICalendar CreateByBruteForceEventRescan() => new BruteForceEventRescanCalendar();

    private sealed class HashMapMergeSortSweepCalendar : ICalendar
    {
        private readonly HashMap<int, int> _delta = new();

        public int Book(int start, int end)
        {
            AddDelta(start, 1);
            AddDelta(end, -1);

            return MaxOverlapAcrossDeltas();
        }

        private void AddDelta(int point, int amount)
        {
            var current = _delta.TryGetValue(point, out var existing) ? existing : 0;
            _delta.Set(point, current + amount);
        }

        // Folds every recorded +1/-1 into a prefix sum over the booking points in
        // ascending order; the running sum's high-water mark is the answer.
        private int MaxOverlapAcrossDeltas()
        {
            var keys = _delta.Keys.ToArray();
            MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(keys));

            var running = 0;
            var maxOverlap = 0;

            foreach (var key in keys)
            {
                _delta.TryGetValue(key, out var change);
                running += change;
                maxOverlap = Math.Max(maxOverlap, running);
            }

            return maxOverlap;
        }
    }

    private sealed class BruteForceEventRescanCalendar : ICalendar
    {
        private readonly List<(int Start, int End)> _bookings = [];
        private int _maxOverlap;

        public int Book(int start, int end)
        {
            _bookings.Add((start, end));
            _maxOverlap = Math.Max(_maxOverlap, MaxOverlapAcrossStarts());
            return _maxOverlap;
        }

        private int MaxOverlapAcrossStarts()
        {
            var maxOverlap = 0;

            foreach (var (candidateStart, _) in _bookings)
            {
                var overlapCount = OverlapCountAt(candidateStart);
                maxOverlap = Math.Max(maxOverlap, overlapCount);
            }

            return maxOverlap;
        }

        private int OverlapCountAt(int candidateStart)
        {
            var overlapCount = 0;

            foreach (var (start, end) in _bookings)
            {
                if (start <= candidateStart && candidateStart < end)
                {
                    overlapCount++;
                }
            }

            return overlapCount;
        }
    }
}
