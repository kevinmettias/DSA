using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.SuffixTree;

// One node of the compressed suffix trie. Every node always owns a Children map regardless of its
// current entry count, mirroring TrieNode's "always has Children plus a nullable-ish terminal
// marker" shape - so a leaf legitimately gaining children later (an earlier-processed suffix that
// turns out to be a proper prefix of a later one, since no sentinel is injected) needs no special
// case: Children.Set on a node that already has SuffixStart >= 0 just works.
//
// Start/Length index into the SuffixTree's own shared, once-copied text - never a per-edge copied
// substring.
internal sealed class SuffixTreeNode
{
    public HashMap<char, SuffixTreeNode> Children { get; } = new();

    public int Start { get; set; }

    public int Length { get; set; }

    // -1 = not a suffix end; else the 0-based starting index (into the owning SuffixTree's text)
    // of the suffix that ends exactly at this node.
    public int SuffixStart { get; set; } = -1;

    public bool IsSuffixEnd => SuffixStart >= 0;
}
