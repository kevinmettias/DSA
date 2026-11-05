using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Children keyed by any character, in this repo's HashMap: the store a trie injects when its keys are
// drawn from no alphabet small enough to slot. A child is found in O(1) on average, the hash map's own
// caveat. The map keeps no order, so this store is not ordered and its trie has no tree-topology
// witness.
/// <summary>A trie node's children keyed by any character, in a hash map.</summary>
/// <typeparam name="TValue">The value the trie stores under each key.</typeparam>
public readonly struct HashedChildren<TValue> : ITrieChildStore<HashedChildren<TValue>, TValue>
{
    private readonly HashMap<char, TrieNode<HashedChildren<TValue>, TValue>> _children;

    private HashedChildren(HashMap<char, TrieNode<HashedChildren<TValue>, TValue>> children) => _children = children;

    /// <inheritdoc/>
    public static HashedChildren<TValue> Empty() => new(new HashMap<char, TrieNode<HashedChildren<TValue>, TValue>>());

    /// <inheritdoc/>
    public TrieNode<HashedChildren<TValue>, TValue>? ChildFor(char key)
    {
        if (!_children.TryGetValue(key, out var child))
        {
            return null;
        }

        return child;
    }

    /// <inheritdoc/>
    public TrieNode<HashedChildren<TValue>, TValue> GetOrAddChildFor(char key)
    {
        if (_children.TryGetValue(key, out var child))
        {
            return child;
        }

        var added = new TrieNode<HashedChildren<TValue>, TValue>();
        _children.Set(key, added);

        return added;
    }
}
