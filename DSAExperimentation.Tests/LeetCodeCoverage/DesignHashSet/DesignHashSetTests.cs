using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignHashSet;

// LeetCode 705. Design HashSet: this repo's own Set<int> (itself HashMap<Element,bool>
// -backed, the same primitive ContainsDuplicateTests already composes) wired up
// directly as MyHashSet's Add/Remove/Contains - LC705 wants exactly the operations
// Set<Element> already exposes, no adapter logic needed beyond the method names.
public sealed partial class DesignHashSetTests
{
    [Fact]
    public void AddRemoveContains_LeetCodeExampleSequence_TracksMembershipCorrectly()
    {
        var hashSet = new MyHashSet();

        hashSet.Add(1);
        hashSet.Add(2);
        Assert.True(hashSet.Contains(1));
        Assert.False(hashSet.Contains(3));

        hashSet.Add(3);
        Assert.True(hashSet.Contains(3));

        hashSet.Remove(2);
        Assert.False(hashSet.Contains(2));
    }

    private sealed class MyHashSet
    {
        private readonly Set<int> _items = new();

        public void Add(int key) => _items.TryAdd(key);

        public void Remove(int key) => _items.TryRemove(key);

        public bool Contains(int key) => _items.Has(key);
    }
}
