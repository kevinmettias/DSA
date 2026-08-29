namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Two named child slots by bit value (0/1), not a HashMap<int,BitTrieNode> the way
// TrieNode<TValue>.Children is - replicating BinaryTreeNode<TValue>'s "two named
// slots" PATTERN, per ARCHITECTURE.md §5 step 5, not its type: paying hash
// computation plus a bucket/entries/free-list array for a fixed 2-key alphabet
// would be exactly the Representation-cost mismatch §2 warns "what reaching them
// costs" is a real, if unstated, contract. No TValue payload either, unlike
// TrieNode<TValue> or BinaryTreeNode<TValue> (which mandates a non-nullable Value
// at construction) - Insert/TryMaxXor never need one, only structural presence.
// The same two named slots are what earns this type its real ITreeTopology witness
// (BitTrieTopology.cs, BitTrieChildren.cs) - see BitTrie.cs's own doc comment.
//
// { get; set; }, not TrieNode's { get; }-only Children: this minimal Insert-only
// scope never reassigns a child once set (no Remove/rebalance), so a settable
// property is more permissive than strictly needed, but BitTrie.Insert is the only
// writer and always writes via ??=, so there is no actual mutation-safety gap to
// close with a narrower API.
internal sealed class BitTrieNode
{
    public BitTrieNode? Zero { get; set; }
    public BitTrieNode? One { get; set; }
}
