using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Mirrors BinaryTreeTopology/GridTopology exactly: a one-line witness closing
// ITreeTopology over an already-public Representation (SparseArrayChildren), reused
// as-is per ARCHITECTURE.md §5 step 5's carve-out (composing another domain's already-
// public concrete Representation is fine; only forcing that domain's own type to
// implement a new interface for you is prohibited). A test-only, IsWord-only trie fixture
// rehearsed this composition first; this type is that same shape promoted to a real,
// TValue-generic, production Operations type (LowercaseTrie.cs), and the fixture is gone.
/// <summary>
/// The tree topology witness for <see cref="LowercaseTrieNode{TValue}"/>, whose children are its filled letter slots.
/// </summary>
public readonly struct LowercaseTrieTopology<TValue>
    : ITreeTopology<LowercaseTrieNode<TValue>, SparseArrayChildren<LowercaseTrieNode<TValue>>>
{
    /// <summary>Lists a trie node's children, its filled letter slots in alphabetical order.</summary>
    /// <param name="node">The trie node whose children to list.</param>
    /// <returns>A view that scans the node's 26 slots on each access, skipping the empty ones.</returns>
    public static SparseArrayChildren<LowercaseTrieNode<TValue>> GetChildren(LowercaseTrieNode<TValue> node) => new(node.Children);
}
