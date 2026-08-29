namespace DSAExperimentation.DataStructures.Trie;

// Set/HasKey/TryGetValue/HasPrefix are O(m) in key/prefix length m, contingent on
// TrieNode<TValue>.Children giving O(1) average-case per-character dispatch (see
// TrieNode.cs) - a slower Children representation would silently degrade this to
// O(m * n) with no compiler error.
//
// No ITreeTopology witness here - NOT because there's "nothing to parametrize"
// (GridTopology/BinaryTreeTopology have nothing to parametrize either and still
// implement the base interface fine; determinism was never the actual test). The real
// reason is Representation: every generic Graph engine's IChildren.Get(int) is called
// in a plain sequential 0..Count-1 sweep (DepthFirstWalk.cs/BreadthFirstWalk.cs/
// TopDownWalk.cs), which the interface's own doc comment still states as an O(1)-per-
// call obligation - and TrieNode<TValue>.Children is a HashMap<char,·>, which cannot
// honestly promise indexed O(1) Get the way a fixed slot array can (see
// SparseArrayChildren.cs, "e.g. a trie node's per-character children" - written for
// exactly this case). Bound the alphabet to something a fixed slot array can size for
// (26 lowercase letters) and that mismatch disappears: see
// Graph/Engines/Dags/Trees/LowercaseTrie.cs, a real ITreeTopology witness that reuses
// TreeMetrics/LowestCommonAncestor/every other generic Tree-tier engine for free. This
// general, arbitrary-char-keyed Trie<TValue> stays witness-less because its whole
// value is NOT being bounded to a small alphabet - narrowing it to gain a witness
// would defeat the type's own purpose, not just cost something.
//
// A hypothetical compressed/radix trie would still only ever be a second
// Representation (identical answers, different node layout/constants), never a second
// Topology, the same way Min-heap vs. Max-heap is a real Topology choice (differing
// observable output) but union-by-rank vs. union-by-size is not.
internal sealed class Trie<TValue>
{
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
