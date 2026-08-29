using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// Computed on demand from Zero/One, the same on-demand shape BinaryTreeChildren uses
// for Left/Right - two named slots, not a stored array, so this costs nothing beyond
// what BitTrieNode already carries. Count/Get compact away a missing slot
// (Zero-then-One order), the same compaction BinaryTreeChildren does for Left/Right -
// but unlike BinaryTreeChildren's own InOrderTraversal carve-out, nothing in this
// domain needs "was this specifically the 0-branch or the 1-branch" positional
// identity preserved: Insert/TryMaxXor never go through IChildren at all (both stay
// hand-written against Zero/One directly), so this type exists purely to open BitTrie
// to generic Tree-tier engines (TreeMetrics, LowestCommonAncestor, ...), which only
// ever ask "how many children, and give me each one."
internal readonly struct BitTrieChildren(BitTrieNode node) : IChildren<BitTrieNode>
{
    public int Count
    {
        get
        {
            var count = 0;

            if (node.Zero is not null)
            {
                count++;
            }

            if (node.One is not null)
            {
                count++;
            }

            return count;
        }
    }

    public BitTrieNode Get(int index)
    {
        if (node.Zero is not null)
        {
            if (index == 0)
            {
                return node.Zero;
            }

            index--;
        }

        if (node.One is not null && index == 0)
        {
            return node.One;
        }

        throw new IndexOutOfRangeException();
    }
}
