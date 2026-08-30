using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertDeleteGetRandomO1;

// LeetCode 380. Insert Delete GetRandom O(1): this repo's own HashMap<int,int>
// (value -> its position in the backing array) composed with DynamicArray<int> (the
// values themselves) - the MinStack precedent of "compose, don't invent a new
// representation," applied to a design problem instead of an algorithm. Remove swaps
// the removed slot with the last slot before truncating, so DynamicArray.RemoveAt
// always runs on the LAST index - its O(1) path (no shifting), never the O(n) one -
// which is what makes O(1) removal from the middle of an otherwise-unordered array
// possible at all.
public sealed partial class InsertDeleteGetRandomO1Tests
{
    [Fact]
    public void InsertRemoveGetRandom_LeetCodeExampleSequence_TracksMembershipCorrectly()
    {
        var set = new RandomizedSet();

        Assert.True(set.Insert(1));
        Assert.False(set.Remove(2));
        Assert.True(set.Insert(2));
        Assert.Equal(2, set.Count);
        Assert.Contains(set.GetRandom(), new[] { 1, 2 });
        Assert.True(set.Remove(1));
        Assert.False(set.Insert(2));
        Assert.Equal(2, set.GetRandom());
    }

    [Fact]
    public void Remove_MiddleElement_SwapsLastElementIntoItsSlotAndKeepsLookupsConsistent()
    {
        var set = new RandomizedSet();
        set.Insert(10);
        set.Insert(20);
        set.Insert(30);

        Assert.True(set.Remove(10));

        Assert.Equal(2, set.Count);
        Assert.False(set.Remove(10));
        Assert.True(set.Remove(30));
        Assert.True(set.Remove(20));
        Assert.Equal(0, set.Count);
    }

    private sealed class RandomizedSet
    {
        private readonly HashMap<int, int> _indexByValue = new();
        private readonly DynamicArray<int> _values = new();
        private readonly Random _random = new(1);

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

            var lastIndex = _values.Count - 1;
            var lastValue = _values.Get(lastIndex);

            _values.Set(index, lastValue);
            _indexByValue.Set(lastValue, index);

            _values.RemoveAt(lastIndex);
            _indexByValue.TryRemove(value);
            return true;
        }

        public int GetRandom() => _values.Get(_random.Next(_values.Count));
    }
}
