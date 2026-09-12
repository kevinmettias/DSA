using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.DesignHashSet;

// LeetCode 705. Design HashSet: a set of non-negative integers supporting
// Add/Remove/Contains without using a built-in set type.
//
// An instance API rather than a pure function, so "every strategy for the
// problem" (§17.3) takes the form of two full classes implementing the
// shared IMyHashSet surface below, instead of two static methods sharing an
// <Operation>By<Strategy> name - the same shape InsertDeleteGetRandomO1Solution
// already uses for its own Design-category problem. There is no separate
// "prepare input" step to hoist into a benchmark's [GlobalSetup]; each
// [Benchmark] arm constructs its own instance and replays the same call
// script instead.
internal static class DesignHashSetSolution
{
    // The shared surface both strategies implement, so the test and
    // benchmark harnesses can replay one call script against either
    // strategy without restating it.
    internal interface IMyHashSet
    {
        void Add(int key);

        void Remove(int key);

        bool Contains(int key);
    }

    // The textbook answer: a plain BCL List<int>, an O(n) Contains scan
    // before every Add (to preserve set semantics) and before every
    // Contains/Remove call - deliberately without this repo's primitives,
    // the arm the Set<int>-backed strategy below has to justify itself
    // against.
    internal sealed class MyHashSetByListScan : IMyHashSet
    {
        private readonly List<int> _values = [];

        public void Add(int key)
        {
            if (!_values.Contains(key))
            {
                _values.Add(key);
            }
        }

        public void Remove(int key) => _values.Remove(key);

        public bool Contains(int key) => _values.Contains(key);
    }

    // This repo's own Set<int> (itself HashMap<Element,bool>-backed, the
    // same primitive ContainsDuplicateTests already composes) wired up
    // directly - LC 705 wants exactly the operations Set<Element> already
    // exposes, no adapter logic beyond the method names.
    internal sealed class MyHashSetBySetBacked : IMyHashSet
    {
        private readonly Set<int> _items = new();

        public void Add(int key) => _items.TryAdd(key);

        public void Remove(int key) => _items.TryRemove(key);

        public bool Contains(int key) => _items.Has(key);
    }
}
