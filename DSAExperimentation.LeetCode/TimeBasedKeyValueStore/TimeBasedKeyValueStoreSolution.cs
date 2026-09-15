using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.TimeBasedKeyValueStore;

// LeetCode 981. Time Based Key-Value Store: TimeMap.Set(key, value, timestamp)
// records a value, and TimeMap.Get(key, timestamp) reports the value stored at the
// largest recorded timestamp <= the query, or "" when there is none. LeetCode
// guarantees the Set timestamps of a key are strictly increasing, so each key's
// history is already sorted by the time it is queried.
//
// This is a "design" problem - the published interface is a stateful object
// replayed across a call script, not a single pure function - so the strategies
// here are factory methods returning that stateful object, the same
// CreateBy<Strategy> shape MyCalendarISolution and AllOneDataStructureSolution use
// for theirs.
//
// CreateByLinearFloorScan walks a key's whole history on every Get: O(n) per
// query. CreateByBinarySearchFloor keeps the same history in parallel
// DynamicArray<int>/DynamicArray<string> columns - the "leader at index i" shape
// OnlineElectionSolution already uses - and floors to the query with this repo's
// own BinarySearch.UpperBound over a DynamicArraySequence<int> witness, minus one:
// the index just past the last value set at or before t. O(log n) per query.
internal static class TimeBasedKeyValueStoreSolution
{
    // Real answer: parallel history columns plus BinarySearch.UpperBound.
    public static ITimeMap CreateByBinarySearchFloor() => new BinarySearchFloorTimeMap();

    // The textbook answer: append to a flat list per key and scan it forward until
    // the timestamps overshoot the query. Deliberately written without this repo's
    // primitives - it is the arm the composed strategy above has to justify itself
    // against.
    public static ITimeMap CreateByLinearFloorScan() => new LinearFloorScanTimeMap();

    public interface ITimeMap
    {
        void Set(string key, string value, int timestamp);

        string Get(string key, int timestamp);
    }

    private sealed class BinarySearchFloorTimeMap : ITimeMap
    {
        private readonly HashMap<string, History> _histories = new();

        public void Set(string key, string value, int timestamp)
        {
            if (!_histories.TryGetValue(key, out var history))
            {
                history = new History();
                _histories.Set(key, history);
            }

            history.Timestamps.Add(timestamp);
            history.Values.Add(value);
        }

        public string Get(string key, int timestamp)
        {
            if (!_histories.TryGetValue(key, out var history) || history.Timestamps.Count == 0)
            {
                return string.Empty;
            }

            var sequence = new DynamicArraySequence<int>(history.Timestamps);
            var floorIndex = BinarySearch.UpperBound(sequence, timestamp) - 1;

            return floorIndex < 0 ? string.Empty : history.Values.Get(floorIndex);
        }

        private sealed record History
        {
            public DynamicArray<int> Timestamps { get; } = new();

            public DynamicArray<string> Values { get; } = new();
        }
    }

    private sealed class LinearFloorScanTimeMap : ITimeMap
    {
        private readonly Dictionary<string, List<(int Timestamp, string Value)>> _histories = [];

        public void Set(string key, string value, int timestamp)
        {
            if (!_histories.TryGetValue(key, out var history))
            {
                history = [];
                _histories[key] = history;
            }

            history.Add((timestamp, value));
        }

        public string Get(string key, int timestamp)
        {
            if (!_histories.TryGetValue(key, out var history))
            {
                return string.Empty;
            }

            var floorValue = string.Empty;

            foreach (var (recorded, value) in history)
            {
                if (recorded > timestamp)
                {
                    break;
                }

                floorValue = value;
            }

            return floorValue;
        }
    }
}
