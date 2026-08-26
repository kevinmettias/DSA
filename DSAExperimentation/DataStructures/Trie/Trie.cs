namespace DSAExperimentation.DataStructures.Trie;

// Set/HasKey/TryGetValue/HasPrefix are O(m) in key/prefix length m, contingent on
// TrieNode<TValue>.Children giving O(1) average-case per-character dispatch (see
// TrieNode.cs) - a slower Children representation would silently degrade this to
// O(m * n) with no compiler error.
//
// There is no Topology witness here: every TrieNode<TValue> is created internally via
// Children.Set(ch, new TrieNode<TValue>()), so no caller-supplied shape could ever
// violate the prefix-tree invariant - stronger than DisjointSet's own "no topology axis"
// case, which only avoids external violation by convention rather than by construction.
// A hypothetical compressed/radix trie would still only ever be a second
// Representation (identical answers, different node layout/constants), never a second
// Topology, the same way Min-heap vs. Max-heap is a real Topology choice (differing
// observable output) but union-by-rank vs. union-by-size is not.
internal sealed class Trie<TValue>
{
    private const string KeyNotPresentMessage = "The given key was not present in the trie.";

    private readonly TrieNode<TValue> _root = new();

    public int Count { get; private set; }

    public void Set(string key, TValue value)
    {
        var current = _root;

        foreach (var ch in key)
        {
            if (!current.Children.TryGetValue(ch, out var next))
            {
                next = new TrieNode<TValue>();
                current.Children.Set(ch, next);
            }

            current = next;
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
            // the standard TryGetValue/TryParse out-parameter contract this mirrors.
            value = default!;
            return false;
        }

        value = node.Value;
        return true;
    }

    // A convenience throwing form beside TryGetValue's recoverable one, the same
    // pairing Heap.Peek/Stack.Peek/Deque.PeekFront/Queue.Peek/HashMap.Get already use
    // for their own "not present" precondition.
    public TValue Get(string key)
        => TryGetValue(key, out var value)
            ? value
            : ThrowKeyNotPresent();

    private static TValue ThrowKeyNotPresent() => throw new InvalidOperationException(KeyNotPresentMessage);

    // The empty prefix is a special case: _root exists unconditionally from
    // construction (unlike every other node, which is only ever created while Set
    // walks an actual key), so "found a node" alone would wrongly report a match on
    // a trie with zero keys.
    public bool HasPrefix(string prefix) => prefix.Length == 0 ? Count > 0 : FindNode(prefix) is not null;

    private TrieNode<TValue>? FindNode(string key)
    {
        var current = _root;

        foreach (var ch in key)
        {
            if (!current.Children.TryGetValue(ch, out var next))
            {
                return null;
            }

            current = next;
        }

        return current;
    }
}
