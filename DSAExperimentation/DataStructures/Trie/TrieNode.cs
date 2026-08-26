using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.Trie;

// Children dispatch (Get/Set per character) is O(1) *average* case, not worst case -
// hash-collision-dependent, the same caveat every HashMap-backed structure in this repo
// carries. Trie.cs's own O(m)-per-operation claim depends on this holding.
internal sealed class TrieNode<TValue>
{
    public HashMap<char, TrieNode<TValue>> Children { get; } = new();

    public bool HasValue { get; set; }

    // presumption: allow -- Value is only meaningful once HasValue is true (set by
    // Trie<TValue>.Set); every reader (HasKey/TryGetValue) already checks HasValue
    // first, the same "only meaningful when the caller's own check passed" contract
    // HashMap.TryGetValue's out-parameter uses.
    public TValue Value { get; set; } = default!;
}
