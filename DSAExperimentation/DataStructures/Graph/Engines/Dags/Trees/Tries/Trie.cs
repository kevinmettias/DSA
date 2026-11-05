namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie: a key is a path from the root, one edge per character, and the value sits at the node
// the path ends on. What distinguishes one trie from another is only how a node keeps its children,
// and that is the TStore a configuration injects (the owner's ruling, 2026-11-02):
// - AlphabetSlots<TAlphabet, TLayout, TValue> keys children by an alphabet's letters, at most 64, in a
//   slot per letter (Dense) or packed in letter order (Packed). Its children are ordered, so the trie
//   has a tree-topology witness (TrieTopology) and every generic tree engine walks it.
// - HashedChildren<TValue> keys children by any character in a hash map: no alphabet to commit to, no
//   order, and no witness.
// The walk below is written once for both. A problem whose keys are drawn from a small alphabet says
// so by injecting AlphabetSlots, and one whose keys are not injects HashedChildren.
/// <summary>A string-keyed map that also answers prefix queries, in time linear in the key or prefix.</summary>
/// <typeparam name="TStore">How each node keeps its children.</typeparam>
/// <typeparam name="TValue">The value stored under each key.</typeparam>
public sealed class Trie<TStore, TValue> : IPrefixMap<TValue>
    where TStore : struct, ITrieChildStore<TStore, TValue>
{
    private readonly TrieNode<TStore, TValue> _root = new();

    /// <inheritdoc/>
    public int Count { get; private set; }

    /// <summary>Gets the root, the node the empty key ends on.</summary>
    public TrieNode<TStore, TValue> Root => _root;

    /// <inheritdoc/>
    public void Set(string key, TValue value)
    {
        var current = _root;

        foreach (var ch in key)
        {
            current = current.Children.GetOrAddChildFor(ch);
        }

        if (!current.HasValue)
        {
            Count++;
        }

        current.HasValue = true;
        current.Value = value;
    }

    /// <inheritdoc/>
    public bool HasKey(string key) => FindNode(key)?.HasValue ?? false;

    /// <inheritdoc/>
    public bool TryGetValue(string key, out TValue value)
    {
        var node = FindNode(key);

        if (node is null || !node.HasValue)
        {
            // presumption: allow -- value is only meaningful when this returns true, the standard
            // TryGetValue out-parameter contract.
            value = default!;
            return false;
        }

        value = node.Value;
        return true;
    }

    // The root exists from construction, unlike every node a key's path creates, so finding a node for
    // the empty prefix says nothing about whether any key is stored.
    /// <inheritdoc/>
    public bool HasPrefix(string prefix) => prefix.Length == 0 ? Count > 0 : FindNode(prefix) is not null;

    private TrieNode<TStore, TValue>? FindNode(string key)
    {
        var current = _root;

        foreach (var ch in key)
        {
            current = current.Children.ChildFor(ch);

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }
}
