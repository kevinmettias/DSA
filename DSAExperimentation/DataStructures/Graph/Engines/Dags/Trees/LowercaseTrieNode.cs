namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A trie node bounded to the 26 lowercase English letters, backed by a fixed slot
// array (Graph.Contracts.Ordering.SparseArrayChildren) rather than Trie.TrieNode<TValue>'s
// open HashMap<char,TrieNode<TValue>>. The bounded alphabet is what earns IChildren's
// O(1)-indexed-Get obligation (ARCHITECTURE.md §8) honestly: a fixed-size array
// supports it directly, a hash-keyed map does not (see Trie.cs's own doc comment on
// why it stays witness-less) - this is the distinguishing feature that promotes a
// trie from "tree-shaped, no witness" to a real ITreeTopology witness, see
// LowercaseTrieTopology.cs.
/// <summary>
/// A <see cref="LowercaseTrie{TValue}"/> node, with one child slot per letter and an optional value.
/// </summary>
public sealed class LowercaseTrieNode<TValue>
{
    /// <summary>
    /// The child slots, 0 for 'a' through 25 for 'z', null where no path continues with that letter.
    /// </summary>
    public LowercaseTrieNode<TValue>?[] Children { get; } = new LowercaseTrieNode<TValue>[LowercaseAlphabet.Size];

    /// <summary>Whether a key ends at this node, so that <see cref="Value"/> holds its value.</summary>
    public bool HasValue { get; set; }

    // presumption: allow -- Value is only meaningful once HasValue is true (set by
    // LowercaseTrie<TValue>.Set); every reader (HasKey/TryGetValue) already checks
    // HasValue first, the same contract Trie.TrieNode<TValue>.Value already uses.
    /// <summary>
    /// The value stored for the key ending here, meaningful only when <see cref="HasValue"/> is true.
    /// </summary>
    public TValue Value { get; set; } = default!;
}
