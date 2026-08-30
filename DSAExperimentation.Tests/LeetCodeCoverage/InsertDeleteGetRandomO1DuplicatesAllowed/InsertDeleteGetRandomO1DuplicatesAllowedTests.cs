using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertDeleteGetRandomO1DuplicatesAllowed;

// LeetCode 381. Insert Delete GetRandom O(1) - Duplicates allowed: LC380's swap-with-
// the-tail trick alone isn't enough once a value can occupy more than one array
// position - Remove needs to pick SOME occurrence of the target value in O(1), which a
// value -> single-index HashMap can no longer represent. This repo's HashMap<TKey,
// DoublyLinkedListNode<TValue>> + DoublyLinkedList<TValue> intrusive-node composition -
// the exact shape LruCache/LfuCache already use for O(1) arbitrary-node removal - gives
// each value its own DoublyLinkedList of "occurrence" nodes, one per array position it
// currently holds; PopBack both picks and removes an occurrence in one O(1) step. A
// second HashMap<int, Node> (array position -> the occurrence node that currently
// represents it) is what lets the post-swap step retarget the moved element's occurrence
// record by mutating one node's Value in place - O(1), no list scan, the same role a
// swapped element's own index update plays in LC380.
public sealed partial class InsertDeleteGetRandomO1DuplicatesAllowedTests
{
    [Fact]
    public void InsertRemoveGetRandom_LeetCodeExampleSequence_TracksMultiplicitiesCorrectly()
    {
        var collection = new RandomizedCollection();

        Assert.True(collection.Insert(1));
        Assert.False(collection.Insert(1));
        Assert.True(collection.Insert(2));
        Assert.Equal(3, collection.Count);
        Assert.Contains(collection.GetRandom(), new[] { 1, 2 });

        Assert.True(collection.Remove(1));
        Assert.Equal(2, collection.Count);
    }

    [Fact]
    public void Remove_OneOfTwoDuplicates_LeavesTheOtherOccurrenceInPlace()
    {
        var collection = new RandomizedCollection();
        collection.Insert(5);
        collection.Insert(5);
        collection.Insert(7);

        Assert.True(collection.Remove(5));
        Assert.Equal(2, collection.Count);

        Assert.True(collection.Remove(5));
        Assert.False(collection.Remove(5));
        Assert.Equal(1, collection.Count);
        Assert.Equal(7, collection.GetRandom());
    }

    [Fact]
    public void Insert_AfterFullyRemovingAValue_ReportsItAsNewAgain()
    {
        var collection = new RandomizedCollection();
        collection.Insert(9);

        Assert.True(collection.Remove(9));
        Assert.True(collection.Insert(9));
    }

    private sealed class RandomizedCollection
    {
        private readonly DynamicArray<int> _values = new();
        private readonly HashMap<int, DoublyLinkedList<int>> _occurrencesByValue = new();
        private readonly HashMap<int, DoublyLinkedListNode<int>> _nodeByPosition = new();
        private readonly Random _random = new(1);

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

            var removedNode = occurrences.PopBack();
            var removedPosition = removedNode.Value;
            _nodeByPosition.TryRemove(removedPosition);

            if (occurrences.Count == 0)
            {
                _occurrencesByValue.TryRemove(value);
            }

            var lastPosition = _values.Count - 1;
            var lastValue = _values.Get(lastPosition);
            _values.Set(removedPosition, lastValue);

            if (removedPosition != lastPosition)
            {
                _nodeByPosition.TryGetValue(lastPosition, out var movedNode);
                movedNode.Value = removedPosition;
                _nodeByPosition.TryRemove(lastPosition);
                _nodeByPosition.Set(removedPosition, movedNode);
            }

            _values.RemoveAt(lastPosition);
            return true;
        }

        public int GetRandom() => _values.Get(_random.Next(_values.Count));
    }
}
