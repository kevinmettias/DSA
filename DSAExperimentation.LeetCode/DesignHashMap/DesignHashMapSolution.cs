using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DesignHashMap;

// LeetCode 706. Design HashMap: an int-key/int-value map supporting
// Put/Get/Remove without using a built-in map type, where Get reports a
// missing key as -1 (LeetCode's own sentinel, absorbed at this call site
// rather than pushed into either strategy's storage).
//
// An instance API rather than a pure function, so "every strategy for the
// problem" (§17.3) takes the form of two full classes implementing the
// shared IMyHashMap surface below, instead of two static methods sharing an
// <Operation>By<Strategy> name - the same shape InsertDeleteGetRandomO1Solution
// already uses for its own Design-category problem. There is no separate
// "prepare input" step to hoist into a benchmark's [GlobalSetup]; each
// [Benchmark] arm constructs its own instance and replays the same call
// script instead.
internal static class DesignHashMapSolution
{
    // The shared surface both strategies implement, so the test and
    // benchmark harnesses can replay one call script against either
    // strategy without restating it.
    internal interface IMyHashMap
    {
        void Put(int key, int value);

        int Get(int key);

        void Remove(int key);
    }

    // The textbook answer: a plain BCL List<(int,int)>, an O(n) linear scan
    // on every Put (to find and update an existing key), Get, and Remove -
    // deliberately without this repo's primitives, the "no hashing at all"
    // arm the HashMap-backed strategy below has to justify itself against.
    internal sealed class MyHashMapByLinearScanList : IMyHashMap
    {
        private readonly List<(int Key, int Value)> _entries = [];

        public void Put(int key, int value)
        {
            for (var i = 0; i < _entries.Count; i++)
            {
                if (_entries[i].Key == key)
                {
                    _entries[i] = (key, value);
                    return;
                }
            }

            _entries.Add((key, value));
        }

        public int Get(int key)
        {
            foreach (var entry in _entries)
            {
                if (entry.Key == key)
                {
                    return entry.Value;
                }
            }

            return -1;
        }

        public void Remove(int key) => _entries.RemoveAll(entry => entry.Key == key);
    }

    // A thin int-key/int-value wrapper directly over this repo's own
    // HashMap<TKey,TValue> - Put/Get/Remove map 1:1 onto Set/TryGetValue/
    // TryRemove, with Get's LeetCode-mandated "-1 for missing key" contract
    // absorbed at this call site rather than pushed into HashMap itself
    // (whose own TryGetValue already generalizes past any one sentinel
    // value).
    internal sealed class MyHashMapByHashMapBacked : IMyHashMap
    {
        private readonly HashMap<int, int> _entries = new();

        public void Put(int key, int value) => _entries.Set(key, value);

        public int Get(int key) => _entries.TryGetValue(key, out var value) ? value : -1;

        public void Remove(int key) => _entries.TryRemove(key);
    }
}
