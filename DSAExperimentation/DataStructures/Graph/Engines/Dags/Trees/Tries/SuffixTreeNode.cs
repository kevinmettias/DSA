namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// One node of a SuffixTree, the one trie over ranges of one text. Its children sit in a hashed
// store keyed by the first letter on each child's edge, since a text's characters come from no
// alphabet small enough to slot, and the rest is the payload a suffix tree needs, which is this
// node class (ITrieNode):
// - the edge into the node, a range of the tree's text by its start and length, read through
//   SuffixTreeEdges, so no edge copies its letters;
// - the start of the suffix that ends at the node, if one does;
// - how many suffixes run through the node.
// Every node has a store whatever its child count, so a leaf that turns out to be a prefix of a
// later suffix, since no sentinel ends the text, takes children with no special case.
internal sealed class SuffixTreeNode
    : ITrieNode<SuffixTreeNode, HashedNodeChildren<SuffixTreeNode>>
{
    private HashedNodeChildren<SuffixTreeNode> _children =
        HashedNodeChildren<SuffixTreeNode>.Empty();

    public ref HashedNodeChildren<SuffixTreeNode> Children => ref _children;

    public int Start { get; set; }

    public int Length { get; set; }

    // -1 = not a suffix end; else the 0-based starting index (into the owning SuffixTree's text)
    // of the suffix that ends exactly at this node.
    public int SuffixStart { get; set; } = -1;

    public bool IsSuffixEnd => SuffixStart >= 0;

    // How many suffixes run through this node, its own included when it ends one: the number of
    // times the string its path spells occurs in the text. Set once, by the build, when the node
    // leaves the rightmost path and no later suffix can reach it.
    public int SuffixCount { get; set; }

    static SuffixTreeNode ITrieNode<SuffixTreeNode, HashedNodeChildren<SuffixTreeNode>>.New() =>
        new();
}
