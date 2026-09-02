using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.InsertDeleteGetRandomO1DuplicatesAllowed;

// LeetCode 381. Insert Delete GetRandom O(1) - Duplicates allowed: LC380's swap-with-
// the-tail trick alone isn't enough once a value can occupy more than one array
// position - Remove needs to pick SOME occurrence of the target value in O(1), which a
// value -> single-index HashMap can no longer represent.
//
// An instance API (insert/remove/getRandom) rather than a pure function, so "every
// strategy for the problem" (§17.3) takes the form of two full classes implementing
// the shared IRandomizedCollection surface below, instead of two static methods
// sharing an <Operation>By<Strategy> name - the same shape
// InsertDeleteGetRandomO1Solution already uses for its own Design-category problem (LC
// 380). A Design problem's whole point is a sequence of mutating calls against one
// instance, so there is no separate "prepare input" step to hoist into a benchmark's
// [GlobalSetup]; each [Benchmark] arm constructs its own instance and replays the same
// call script instead.
internal static class InsertDeleteGetRandomO1DuplicatesAllowedSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without restating
    // it.
    internal interface IRandomizedCollection
    {
        int Count { get; }

        bool Insert(int value);

        bool Remove(int value);

        int GetRandom();
    }

    // The textbook answer: a plain BCL List<int>. Remove does a linear IndexOf scan to
    // find SOME occurrence of the target value, then an indexed removal that shifts
    // every later element - deliberately without this repo's primitives, the arm the
    // linked-occurrences strategy below has to justify itself against.
    internal sealed class RandomizedCollectionByListScan : IRandomizedCollection
    {
        private readonly List<int> _values = [];
        private readonly Random _random = new();

        public int Count => _values.Count;

        public bool Insert(int value)
        {
            var isNewValue = !_values.Contains(value);
            _values.Add(value);
            return isNewValue;
        }

        public bool Remove(int value)
        {
            var index = _values.IndexOf(value);
            if (index < 0)
            {
                return false;
            }

            _values.RemoveAt(index);
            return true;
        }

        public int GetRandom() => _values[_random.Next(_values.Count)];
    }

    // This repo's own HashMap<int, DoublyLinkedList<int>> (value -> its occurrence
    // nodes) + DynamicArray<int> (the values themselves) - the exact intrusive-node
    // shape LruCache/LfuCache already use for O(1) arbitrary-node removal - gives each
    // value its own DoublyLinkedList of "occurrence" nodes, one per array position it
    // currently holds; PopBack both picks and removes an occurrence in one O(1) step.
    // A second HashMap<int, DoublyLinkedListNode<int>> (array position -> the
    // occurrence node that currently represents it) is what lets the post-swap step
    // retarget the moved element's occurrence record by mutating one node's Value in
    // place - O(1), no list scan, the same role a swapped element's own index update
    // plays in LC380.
    internal sealed class RandomizedCollectionByLinkedOccurrences : IRandomizedCollection
    {
        private readonly DynamicArray<int> _values = new();
        private readonly HashMap<int, DoublyLinkedList<int>> _occurrencesByValue = new();
        private readonly HashMap<int, DoublyLinkedListNode<int>> _nodeByPosition = new();
        private readonly Random _random = new();

        public int Count => _values.Count;

        public bool Insert(int value)
        {
            var isNewValue = !_occurrencesByValue.TryGetValue(value, out var occurrences);
            if (isNewValue)
            {
                occurrences = new DoublyLinkedList<int>();
                _occurrencesByValue.Set(value, occurrences);
            }

            var position = _values.Count;
            _values.Add(value);

            var node = new DoublyLinkedListNode<int> { Value = position };
            occurrences.AddFront(node);
            _nodeByPosition.Set(position, node);

            return isNewValue;
        }

        public bool Remove(int value)
        {
            if (!_occurrencesByValue.TryGetValue(value, out var occurrences) || occurrences.Count == 0)
            {
                return false;
            }

            var removedPosition = PopOccurrence(value, occurrences);
            SwapOutPosition(removedPosition);
            return true;
        }

        public int GetRandom() => _values.Get(_random.Next(_values.Count));

        private int PopOccurrence(int value, DoublyLinkedList<int> occurrences)
        {
            var removedNode = occurrences.PopBack();
            var removedPosition = removedNode.Value;
            _nodeByPosition.TryRemove(removedPosition);

            if (occurrences.Count == 0)
            {
                _occurrencesByValue.TryRemove(value);
            }

            return removedPosition;
        }

        private void SwapOutPosition(int removedPosition)
        {
            var lastPosition = _values.Count - 1;
            var lastValue = _values.Get(lastPosition);
            _values.Set(removedPosition, lastValue);

            if (removedPosition != lastPosition)
            {
                MoveTrackedNode(lastPosition, removedPosition);
            }

            _values.RemoveAt(lastPosition);
        }

        private void MoveTrackedNode(int fromPosition, int toPosition)
        {
            _nodeByPosition.TryGetValue(fromPosition, out var movedNode);
            movedNode.Value = toPosition;
            _nodeByPosition.TryRemove(fromPosition);
            _nodeByPosition.Set(toPosition, movedNode);
        }
    }
}
