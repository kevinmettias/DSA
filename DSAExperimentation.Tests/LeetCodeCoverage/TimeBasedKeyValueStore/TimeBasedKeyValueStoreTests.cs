using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TimeBasedKeyValueStore;

// LeetCode 981. Time Based Key-Value Store: HashMap<string,History> keyed by
// `key`, each holding parallel DynamicArray<int>/DynamicArray<string> histories
// (Timestamps/Values) appended in the (problem-guaranteed) strictly increasing
// timestamp order Set is called in - the same parallel-array shape
// OnlineElectionTests already uses for "leader at index i". Get floors to the
// largest timestamp <= the query via this repo's own BinarySearch.UpperBound over
// a DynamicArraySequence<int> witness, minus one - OnlineElectionTests' exact
// "last vote at or before t" idiom, reused here for "last value set at or before
// t".
public sealed partial class TimeBasedKeyValueStoreTests
{
    [Fact]
    public void SetThenGet_LeetCodeExampleSequence_ReturnsFloorTimestampValue()
    {
        var store = new TimeMap();

        store.Set("foo", "bar", 1);

        Assert.Equal("bar", store.Get("foo", 1));
        Assert.Equal("bar", store.Get("foo", 3));

        store.Set("foo", "bar2", 4);

        Assert.Equal("bar2", store.Get("foo", 4));
        Assert.Equal("bar2", store.Get("foo", 5));
    }

    [Fact]
    public void Get_TimestampBeforeAnySet_ReturnsEmptyString()
    {
        var store = new TimeMap();
        store.Set("foo", "bar", 5);

        Assert.Equal(string.Empty, store.Get("foo", 1));
    }

    [Fact]
    public void Get_UnknownKey_ReturnsEmptyString()
    {
        var store = new TimeMap();

        Assert.Equal(string.Empty, store.Get("missing", 10));
    }

    private sealed class TimeMap
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

        private sealed class History
        {
            public DynamicArray<int> Timestamps { get; } = new();
            public DynamicArray<string> Values { get; } = new();
        }
    }
}
