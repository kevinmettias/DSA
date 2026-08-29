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
internal sealed class LowercaseTrie<TValue>
{
    private const string InvalidKeyCharacterMessage = "Key must contain only lowercase English letters ('a'-'z').";

    private readonly LowercaseTrieNode<TValue> _root = new();

    public int Count { get; private set; }

    public LowercaseTrieNode<TValue> Root => _root;

    public void Set(string key, TValue value)
    {
        var current = _root;

        foreach (var ch in key)
        {
            current = current.Children[IndexOf(ch)] ??= new LowercaseTrieNode<TValue>();
        }

        if (!current.HasValue)
        {
            Count++;
        }

        current.HasValue = true;
        current.Value = value;
    }

    public bool HasKey(string key) => FindNode(key)?.HasValue ?? false;

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
    public bool HasPrefix(string prefix) => prefix.Length == 0 ? Count > 0 : FindNode(prefix) is not null;

    private LowercaseTrieNode<TValue>? FindNode(string key)
    {
        var current = _root;

        foreach (var ch in key)
        {
            current = current.Children[IndexOf(ch)];

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    private static int IndexOf(char ch)
    {
        var index = ch - 'a';

        if (index < 0 || index >= LowercaseTrieNode<TValue>.AlphabetSize)
        {
            throw new ArgumentOutOfRangeException(nameof(ch), ch, InvalidKeyCharacterMessage);
        }

        return index;
    }
}
