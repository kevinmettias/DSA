using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Mirrors BinaryTreeTopology/GridTopology exactly: a one-line witness closing
// ITreeTopology over an already-public Representation (SparseArrayChildren), reused
// as-is per ARCHITECTURE.md §5 step 5's carve-out (composing another domain's already-
// public concrete Representation is fine; only forcing that domain's own type to
// implement a new interface for you is prohibited). A test-only, IsWord-only trie fixture
// rehearsed this composition first; this type is that same shape promoted to a real,
// TValue-generic, production Operations type (LowercaseTrie.cs), and the fixture is gone.
internal readonly struct LowercaseTrieTopology<TValue>
    : ITreeTopology<LowercaseTrieNode<TValue>, SparseArrayChildren<LowercaseTrieNode<TValue>>>
{
    public static SparseArrayChildren<LowercaseTrieNode<TValue>> GetChildren(LowercaseTrieNode<TValue> node) => new(node.Children);
}
