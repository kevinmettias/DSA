namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// One node of a Trie<TStore, TValue>: its children, kept however the store keeps them, and an optional
// value. The store is a struct field handed out by reference, so a child added through Children is added
// to this node's own store, not to a copy of it.
/// <summary>A node of a trie: its children, kept by the trie's child store, and an optional value.</summary>
/// <typeparam name="TStore">How the node keeps its children.</typeparam>
/// <typeparam name="TValue">The value the trie stores under each key.</typeparam>
public sealed class TrieNode<TStore, TValue>
    where TStore : struct, ITrieChildStore<TStore, TValue>
{
    private TStore _children = TStore.Empty();

    /// <summary>Gets the node's children, by reference, so that adding a child changes this node's own store.</summary>
    public ref TStore Children => ref _children;

    /// <summary>Gets or sets whether a key ends at this node.</summary>
    public bool HasValue { get; set; }

    // presumption: allow -- Value is only meaningful once HasValue is true (set by Trie.Set); every
    // reader (HasKey/TryGetValue) checks HasValue first.
    /// <summary>Gets or sets the value stored under the key that ends here, meaningful only when <see cref="HasValue"/> is true.</summary>
    public TValue Value { get; set; } = default!;

    internal TrieNode()
    {
    }
}
