using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.SmallestNumberInInfiniteSet;

// LeetCode 2336. Smallest Number in Infinite Set: a set that starts out holding every
// positive integer, with PopSmallest() removing and returning its minimum and
// AddBack(num) putting a previously popped number back.
//
// This is a design problem - LeetCode's own shape is a stateful object with two
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// AllOneDataStructureSolution uses for its own instance-API problem.
//
// Both strategies share the one observation that makes the "infinite" set finite: the
// numbers from NextUnused upward have never been produced, so they need no storage at
// all - a single counter represents them. Only numbers pulled back in via AddBack need
// real storage, and PopSmallest can always prefer that collection outright, because
// every value in it was below NextUnused when it was pushed and therefore can never
// exceed the next never-yet-produced number. No comparison between the two sources is
// ever needed.
//
// What separates the arms is how that added-back collection answers "what is the
// smallest, and do you already hold this?": an O(n) linear scan plus an O(n) Contains
// per operation, or O(log n) pop plus O(1) duplicate rejection.
internal static class SmallestNumberInInfiniteSetSolution
{
    // The infinite set is [1, 2, 3, ...], so the first number ever produced is 1.
    private const int FirstNumber = 1;

    // The textbook answer: a plain BCL List<int> of added-back numbers, scanned for its
    // minimum on every pop and scanned again for membership on every add. Deliberately
    // written without this repo's primitives - it is the arm the composition below has
    // to justify itself against.
    public static ISmallestInfiniteSet CreateByListScan() => new ListScanSmallestInfiniteSet();

    // This repo's own Heap<Element, MinHeapOrder<Element>> is already exactly
    // "peek/pop the smallest of a dynamic collection in O(log n)", paired with
    // Set<Element> purely to reject a duplicate AddBack in O(1) instead of letting the
    // same value sit in the heap twice.
    public static ISmallestInfiniteSet CreateByHeapAndSet() => new HeapAndSetSmallestInfiniteSet();

    // LeetCode's SmallestInfiniteSet class surface, as an interface so a harness can
    // replay one call script against either strategy.
    internal interface ISmallestInfiniteSet
    {
        int PopSmallest();

        void AddBack(int num);
    }

    private sealed class ListScanSmallestInfiniteSet : ISmallestInfiniteSet
    {
        private readonly List<int> _addedBack = [];
        private int _nextUnused = FirstNumber;

        public int PopSmallest()
        {
            if (_addedBack.Count == 0)
            {
                return _nextUnused++;
            }

            var minIndex = 0;
            for (var i = 1; i < _addedBack.Count; i++)
            {
                if (_addedBack[i] < _addedBack[minIndex])
                {
                    minIndex = i;
                }
            }

            var smallest = _addedBack[minIndex];
            _addedBack.RemoveAt(minIndex);
            return smallest;
        }

        public void AddBack(int num)
        {
            if (num >= _nextUnused || _addedBack.Contains(num))
            {
                return;
            }

            _addedBack.Add(num);
        }
    }

    private sealed class HeapAndSetSmallestInfiniteSet : ISmallestInfiniteSet
    {
        private readonly Heap<int, MinHeapOrder<int>> _addedBack = new();
        private readonly Set<int> _pending = new();
        private int _nextUnused = FirstNumber;

        public int PopSmallest()
        {
            if (_addedBack.TryPop(out var restored))
            {
                _pending.TryRemove(restored);
                return restored;
            }

            return _nextUnused++;
        }

        public void AddBack(int num)
        {
            if (num >= _nextUnused || !_pending.TryAdd(num))
            {
                return;
            }

            _addedBack.Push(num);
        }
    }
}
