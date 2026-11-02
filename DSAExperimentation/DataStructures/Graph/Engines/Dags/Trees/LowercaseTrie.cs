using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// The bounded-alphabet counterpart to Trie<TValue> (DataStructures/Trie/) - same
// Set/HasKey/TryGetValue/HasPrefix contract and O(m)-in-key-length complexity claim
// (now a real O(1) worst-case per character, not Trie<TValue>'s O(1)-average
// hash-dependent claim, since LowercaseTrieNode.Children is a direct array index),
// but committing to exactly the 26 lowercase English letters is what lets
// LowercaseTrieNode's children be a fixed slot array instead of a HashMap - which is
// what earns this type its real ITreeTopology witness (LowercaseTrieTopology.cs) and,
// through it, free reuse of every generic Tree-tier engine (TreeMetrics,
// LowestCommonAncestor, DepthFirstTraversal, ...) - see LowercaseTrieTests for a
// worked LowestCommonAncestor-as-longest-common-prefix example. Keys outside 'a'..'z'
// are a real, checked precondition violation, not silently accepted the way the
// general Trie<TValue> accepts any char - the bounded alphabet is the whole reason
// this type exists alongside Trie<TValue>, not an incidental restriction.
/// <summary>
/// A trie mapping lowercase-letter keys ('a' to 'z' only) to values, with one child slot per letter.
/// </summary>
public sealed class LowercaseTrie<TValue>
{
    private readonly LowercaseTrieNode<TValue> _root = new();

    /// <summary>The number of distinct keys stored.</summary>
    public int Count { get; private set; }

    /// <summary>The root node, which stands for the empty key and exists even in an empty trie.</summary>
    public LowercaseTrieNode<TValue> Root => _root;

    /// <summary>Stores a value under a key, replacing any value the key already had.</summary>
    /// <param name="key">The key, made only of the letters 'a' to 'z'; it may be empty.</param>
    /// <param name="value">The value to store.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="key"/> holds a character outside 'a' to 'z'; the path built for the
    /// characters before it stays, so <see cref="HasPrefix"/> may then match a prefix no stored key has.
    /// </exception>
    public void Set(string key, TValue value)
    {
        var current = _root;

        foreach (var ch in key)
        {
            current = current.Children[EnglishLowercase.IndexOf(ch)] ??= new LowercaseTrieNode<TValue>();
        }

        if (!current.HasValue)
        {
            Count++;
        }

        current.HasValue = true;
        current.Value = value;
    }

    /// <summary>Checks whether a key itself was stored, not merely a longer key beginning with it.</summary>
    /// <param name="key">The key to look up.</param>
    /// <returns><c>true</c> when the key was set; <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="key"/> holds a character outside 'a' to 'z' and the characters before it
    /// spell a path in the trie.
    /// </exception>
    public bool HasKey(string key) => FindNode(key)?.HasValue ?? false;

    /// <summary>Looks up the value stored under a key.</summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The stored value when the key is found, or default when it is not.</param>
    /// <returns><c>true</c> when the key was set; <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="key"/> holds a character outside 'a' to 'z' and the characters before it
    /// spell a path in the trie.
    /// </exception>
    public bool TryGetValue(string key, out TValue value)
    {
        var node = FindNode(key);

        if (node is null || !node.HasValue)
        {
            // presumption: allow -- value is only meaningful when this returns true,
            // the standard TryGetValue/TryParse out-parameter contract this mirrors
            // (same shape as Trie<TValue>.TryGetValue).
            value = default!;
            return false;
        }

        value = node.Value;
        return true;
    }

    // The empty prefix is a special case: _root exists unconditionally from
    // construction (unlike every other node, which is only ever created while Set
    // walks an actual key), so "found a node" alone would wrongly report a match on a
    // trie with zero keys - identical reasoning to Trie<TValue>.HasPrefix.
    /// <summary>Checks whether any stored key starts with a prefix, including a key equal to it.</summary>
    /// <param name="prefix">The prefix to look for; an empty prefix matches whenever a key is stored.</param>
    /// <returns><c>true</c> when a stored key starts with the prefix; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="prefix"/> holds a character outside 'a' to 'z' and the characters before it
    /// spell a path in the trie.
    /// </exception>
    public bool HasPrefix(string prefix) => prefix.Length == 0 ? Count > 0 : FindNode(prefix) is not null;

    private LowercaseTrieNode<TValue>? FindNode(string key)
    {
        var current = _root;

        foreach (var ch in key)
        {
            current = current.Children[EnglishLowercase.IndexOf(ch)];

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }
}
