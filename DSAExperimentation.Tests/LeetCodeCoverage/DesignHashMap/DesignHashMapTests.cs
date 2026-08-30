using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignHashMap;

// LeetCode 706. Design HashMap: a thin int-key/int-value wrapper directly over this
// repo's own HashMap<TKey,TValue> - Put/Get/Remove map 1:1 onto Set/TryGetValue/
// TryRemove, with Get's LeetCode-mandated "-1 for missing key" contract absorbed at this
// call site rather than pushed into HashMap itself (whose own TryGetValue already
// generalizes past any one sentinel value).
public sealed partial class DesignHashMapTests
{
    [Fact]
    public void PutGetRemove_LeetCodeExampleSequence_MatchesExpectedResults()
    {
        var map = new MyHashMap();

        map.Put(1, 1);
        map.Put(2, 2);
        Assert.Equal(1, map.Get(1));
        Assert.Equal(-1, map.Get(3));

        map.Put(2, 1); // update an existing key's value
        Assert.Equal(1, map.Get(2));

        map.Remove(2);
        Assert.Equal(-1, map.Get(2));
    }

    [Fact]
    public void Remove_KeyNeverInserted_IsANoOp()
    {
        var map = new MyHashMap();

        map.Remove(42);

        Assert.Equal(-1, map.Get(42));
    }

    private sealed class MyHashMap
    {
        private readonly HashMap<int, int> _entries = new();

        public void Put(int key, int value) => _entries.Set(key, value);

        public int Get(int key) => _entries.TryGetValue(key, out var value) ? value : -1;

        public void Remove(int key) => _entries.TryRemove(key);
    }
}
