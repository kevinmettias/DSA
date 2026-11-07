namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The suffix tree's edge labels: every edge is a range of the one text the tree indexes, its start
// and length kept on the node below it (SuffixTreeNode), so a label is read from the text and
// never copied. The tree's walk takes this with the text, and the letter at an offset along an
// edge is the text's at the edge's start plus that offset.
internal readonly struct SuffixTreeEdges(string text) : ITrieEdgeLabels<SuffixTreeNode, char>
{
    public int LengthOf(SuffixTreeNode node) => node.Length;

    public bool HasLetterAt(SuffixTreeNode node, int offset, char letter) =>
        text[node.Start + offset] == letter;
}
