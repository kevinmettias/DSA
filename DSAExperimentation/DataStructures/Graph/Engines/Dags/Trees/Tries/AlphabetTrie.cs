using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The bounded-alphabet counterpart to Trie<TValue> (DataStructures/Trie/): the same
// Set/HasKey/TryGetValue/HasPrefix contract, O(1) worst case per character rather than Trie's hash
// average, because committing to a fixed alphabet lets a node find a child by the letter's slot. That
// same bound is what earns the trie a real ITreeTopology witness (AlphabetTrieTopology), and with it
// every generic tree engine.
//
// Two choices are type arguments, so a trie compiles only what it chose: the alphabet, which says
// which characters are letters and which slot each takes (EnglishLowercase, EnglishLetters with case
// folded, DnaBases, ...), and the layout, which says where a node stores its children (DenseTrieLayout
// reads a slot per letter; PackedTrieLayout stores only the children present). A character outside the
// alphabet is a checked precondition violation, not silently accepted the way Trie<TValue> takes any
// char: the bounded alphabet is the whole reason this type exists beside it.
/// <summary>A trie mapping keys written in a fixed alphabet to values.</summary>
/// <typeparam name="TAlphabet">The letters a key is written in, with at most 64 letters.</typeparam>
/// <typeparam name="TLayout">Where each node stores its children.</typeparam>
/// <typeparam name="TValue">The value a key maps to.</typeparam>
public sealed class AlphabetTrie<TAlphabet, TLayout, TValue>
    where TAlphabet : struct, IAlphabet
    where TLayout : struct, ITrieLayout
{
    private const int MaxAlphabetSize = sizeof(ulong) * 8;
    private const string AlphabetTooLargeMessage = "An alphabet trie's node mask has one bit per letter, so its alphabet has at most 64 letters.";

    private readonly AlphabetTrieNode<TAlphabet, TLayout, TValue> _root = new();

    /// <summary>The number of distinct keys stored.</summary>
    public int Count { get; private set; }

    /// <summary>The root node, which stands for the empty key and exists even in an empty trie.</summary>
    public AlphabetTrieNode<TAlphabet, TLayout, TValue> Root => _root;

    /// <summary>Creates an empty trie.</summary>
    /// <exception cref="NotSupportedException"><typeparamref name="TAlphabet"/> has more than 64 letters.</exception>
    public AlphabetTrie()
    {
        if (TAlphabet.Size > MaxAlphabetSize)
        {
            throw new NotSupportedException(AlphabetTooLargeMessage);
        }
    }

    /// <summary>Stores a value under a key, replacing any value the key already had.</summary>
    /// <param name="key">The key, made only of letters of the alphabet; it may be empty.</param>
    /// <param name="value">The value to store.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="key"/> holds a character outside the alphabet; the path built for the
    /// characters before it stays, so <see cref="HasPrefix"/> may then match a prefix no stored key has.
    /// </exception>
    public void Set(string key, TValue value)
    {
        var current = _root;

        foreach (var ch in key)
        {
            current = current.GetOrAddChild(TAlphabet.IndexOf(ch));
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
    /// Thrown when <paramref name="key"/> holds a character outside the alphabet and the characters before it
    /// spell a path in the trie.
    /// </exception>
    public bool HasKey(string key) => FindNode(key)?.HasValue ?? false;

    /// <summary>Looks up the value stored under a key.</summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The stored value when the key is found, or default when it is not.</param>
    /// <returns><c>true</c> when the key was set; <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="key"/> holds a character outside the alphabet and the characters before it
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

    // The empty prefix is a special case: _root exists unconditionally from construction (unlike every
    // other node, which is only ever created while Set walks an actual key), so "found a node" alone
    // would wrongly report a match on a trie with zero keys - identical reasoning to
    // Trie<TValue>.HasPrefix.
    /// <summary>Checks whether any stored key starts with a prefix, including a key equal to it.</summary>
    /// <param name="prefix">The prefix to look for; an empty prefix matches whenever a key is stored.</param>
    /// <returns><c>true</c> when a stored key starts with the prefix; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="prefix"/> holds a character outside the alphabet and the characters before
    /// it spell a path in the trie.
    /// </exception>
    public bool HasPrefix(string prefix) => prefix.Length == 0 ? Count > 0 : FindNode(prefix) is not null;

    private AlphabetTrieNode<TAlphabet, TLayout, TValue>? FindNode(string key)
    {
        var current = _root;

        foreach (var ch in key)
        {
            current = current.ChildAt(TAlphabet.IndexOf(ch));

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }
}
