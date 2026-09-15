using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.InsertDeleteGetRandomO1;

// LeetCode 380. Insert Delete GetRandom O(1): a set of distinct integers
// supporting O(1) insert, remove, and uniform-random element access.
//
// An instance API (insert/remove/getRandom) rather than a pure function, so
// "every strategy for the problem" (§17.3) takes the form of two full classes
// implementing the shared IRandomizedSet surface below, instead of two static
// methods sharing an <Operation>By<Strategy> name - the same shape
// DesignTaskManagerSolution/ImplementRouterSolution already use for their own
// Design-category problems. A Design problem's whole point is a sequence of
// mutating calls against one instance, so there is no separate "prepare
// input" step to hoist into a benchmark's [GlobalSetup]; each [Benchmark] arm
// constructs its own instance and replays the same call script instead.
internal static class InsertDeleteGetRandomO1Solution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface IRandomizedSet
    {
        int Count { get; }

        bool Insert(int value);

        bool Remove(int value);

        int GetRandom();
    }

    // The textbook answer: a plain BCL List<int>, an O(n) Contains scan on
    // every Insert, an O(n) shifting Remove - deliberately without this
    // repo's primitives, the arm the hashmap+swap-remove strategy below has
    // to justify itself against.
    internal sealed class RandomizedSetByListScan : IRandomizedSet
    {
        private readonly List<int> _values = [];
        private readonly Random _random = new();

        public int Count => _values.Count;

        public bool Insert(int value)
        {
            if (_values.Contains(value))
            {
                return false;
            }

            _values.Add(value);
            return true;
        }

        public bool Remove(int value) => _values.Remove(value);

        public int GetRandom() => _values[_random.Next(_values.Count)];
    }

    // This repo's own HashMap<int,int> (value -> its position in the backing
    // array) composed with DynamicArray<int> (the values themselves) - the
    // MinStack precedent of "compose, don't invent a new representation,"
    // applied to a design problem instead of an algorithm. Remove swaps the
    // removed slot with the last slot before truncating, so
    // DynamicArray.RemoveAt always runs on the LAST index - its O(1) path (no
    // shifting), never the O(n) one - which is what makes O(1) removal from
    // the middle of an otherwise-unordered array possible at all.
    internal sealed class RandomizedSetByHashMapSwapRemove : IRandomizedSet
    {
        private readonly HashMap<int, int> _indexByValue = new();
        private readonly DynamicArray<int> _values = new();
        private readonly Random _random = new();

        public int Count => _values.Count;

        public bool Insert(int value)
        {
            if (_indexByValue.HasKey(value))
            {
                return false;
            }

            _values.Add(value);
            _indexByValue.Set(value, _values.Count - 1);
            return true;
        }

        public bool Remove(int value)
        {
            if (!_indexByValue.TryGetValue(value, out var index))
            {
                return false;
            }

            MoveLastIntoSlot(index);
            _indexByValue.TryRemove(value);
            return true;
        }

        // The swap half of swap-remove: the last value takes the vacated slot,
        // both its references agree on that, and only then is the tail dropped -
        // so RemoveAt always runs on the LAST index, its O(1) path.
        private void MoveLastIntoSlot(int index)
        {
            var lastIndex = _values.Count - 1;
            var lastValue = _values.Get(lastIndex);

            _values.Set(index, lastValue);
            _indexByValue.Set(lastValue, index);

            _values.RemoveAt(lastIndex);
        }

        public int GetRandom() => _values.Get(_random.Next(_values.Count));
    }
}
