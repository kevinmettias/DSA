namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie as a map: a key is a path from the root, one edge per letter, and the value sits at
// the node the path ends on. Three things tell one trie from another, and a configuration injects
// each (the owner's rulings of 2026-11-02 and 2026-11-05):
// - TKey, what a key is and the order its letters are read in: TextKey, a string's characters from
//   the first, or Int32BitsKey, an int's bits from the most significant.
// - TStore, how a node keeps its children: AlphabetSlots over an alphabet of at most 64 letters, a
//   slot per letter (Dense) or packed in letter order (Packed), or HashedChildren over any
//   character, in a hash map, with no order and so no tree-topology witness.
// - The node's payload, the node class (ITrieNode). A map's keys may end anywhere, one on another's
//   path, so its node is TrieNode, which carries a value and whether a key ends there. BitTrie's
//   keys all end at depth 32, so its node, BitTrieNode, carries nothing.
// TLetter, one letter's type, joins the key to the store, so a key can only be walked over a store
// that takes its letters. The walk itself is TrieWalk's, written once for every configuration.
// Trie<TStore, TValue> is the word trie, derived from this one rather than wrapping it, so it is
// one object from its root, as the segment trees are (§13.10).
/// <summary>A map keyed by letter sequences that also answers prefix queries, in time linear in the key or prefix.</summary>
/// <typeparam name="TKey">What a key is, and the order its letters are read in.</typeparam>
/// <typeparam name="TLetter">One letter of a key, the label on one edge.</typeparam>
/// <typeparam name="TStore">How each node keeps its children.</typeparam>
/// <typeparam name="TValue">The value stored under each key.</typeparam>
public abstract class Trie<TKey, TLetter, TStore, TValue>
    where TKey : struct, ITrieKey<TLetter>
    where TStore : struct, ILetteredTrieChildStore<TStore, TrieNode<TStore, TValue>, TLetter>
{
    private readonly TrieNode<TStore, TValue> _root = new();

    /// <summary>Gets how many distinct keys are stored.</summary>
    public int Count { get; private set; }

    /// <summary>Gets the root, the node the empty key ends on.</summary>
    public TrieNode<TStore, TValue> Root => _root;

    private protected Trie()
    {
    }

    /// <summary>Stores a value under a key, replacing any value already stored there.</summary>
    /// <param name="key">The key to store under; a key of no letters is a key like any other.</param>
    /// <param name="value">The value to store.</param>
    public void Set(TKey key, TValue value)
    {
        var current = TrieWalk<TrieNode<TStore, TValue>, TStore, TLetter>.GetOrAddPath(_root, key);

        if (!current.HasValue)
        {
            Count++;
        }

        current.HasValue = true;
        current.Value = value;
    }

    /// <summary>Reports whether a value is stored under a key.</summary>
    /// <param name="key">The key to look for.</param>
    /// <returns>True when the key itself was stored; a key that is only a prefix of stored keys is not.</returns>
    public bool HasKey(TKey key) => FindNode(key)?.HasValue ?? false;

    /// <summary>Reads the value stored under a key.</summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The value stored under the key, when there is one.</param>
    /// <returns>True when a value is stored under the key.</returns>
    public bool TryGetValue(TKey key, out TValue value)
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

    /// <summary>Reports whether any stored key starts with every letter of a prefix.</summary>
    /// <param name="prefix">The prefix to look for; every key starts with a prefix of no letters.</param>
    /// <returns>True when some stored key starts with the prefix, so a prefix of no letters asks whether any key is stored.</returns>
    public bool HasPrefix(TKey prefix) => IsPrefix(prefix);

    // The first letters of a key are a key of their own, KeyPrefix, so a prefix query walks them as
    // it walks any key. A text trie can ask about a word's first letters without cutting the word,
    // and a bit trie, whose keys are all 32 letters long, can ask about a value's high bits at all.
    /// <summary>Reports whether any stored key starts with the first letters of a key.</summary>
    /// <param name="key">The key whose first letters make the prefix.</param>
    /// <param name="length">How many of the key's letters the prefix takes, from 0 to the key's length.</param>
    /// <returns>True when some stored key starts with those letters, so a length of 0 asks whether any key is stored.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative or more than the key's length.</exception>
    public bool HasPrefix(TKey key, int length) => IsPrefix(new KeyPrefix<TKey, TLetter>(key, length));

    // The root exists from construction, unlike every node a key's path creates, so finding a node
    // for a prefix of no letters says nothing about whether any key is stored.
    private bool IsPrefix<TPrefix>(TPrefix prefix)
        where TPrefix : struct, ITrieKey<TLetter>
        => prefix.Length == 0 ? Count > 0 : FindNode(prefix) is not null;

    private TrieNode<TStore, TValue>? FindNode<TWalked>(TWalked key)
        where TWalked : struct, ITrieKey<TLetter>
        => TrieWalk<TrieNode<TStore, TValue>, TStore, TLetter>.Find(_root, key);
}

// The word trie: the one trie over TextKey, so its keys are strings, read a character at a time
// from the first. TextKey converts from a string implicitly, so a caller sets, finds and
// prefix-queries with plain strings, and through IPrefixMap the trie is the string map the contract
// suite holds it to. It adds no state and no walk of its own.
/// <summary>A string-keyed map that also answers prefix queries, in time linear in the key or prefix.</summary>
/// <typeparam name="TStore">How each node keeps its children, keyed by character.</typeparam>
/// <typeparam name="TValue">The value stored under each key.</typeparam>
public sealed class Trie<TStore, TValue> : Trie<TextKey, char, TStore, TValue>, IPrefixMap<TValue>
    where TStore : struct, ILetteredTrieChildStore<TStore, TrieNode<TStore, TValue>, char>
{
    /// <inheritdoc/>
    void IPrefixMap<TValue>.Set(string key, TValue value) => Set(key, value);

    /// <inheritdoc/>
    bool IPrefixMap<TValue>.HasKey(string key) => HasKey(key);

    /// <inheritdoc/>
    bool IPrefixMap<TValue>.TryGetValue(string key, out TValue value) => TryGetValue(key, out value);

    /// <inheritdoc/>
    bool IPrefixMap<TValue>.HasPrefix(string prefix) => HasPrefix(prefix);
}
