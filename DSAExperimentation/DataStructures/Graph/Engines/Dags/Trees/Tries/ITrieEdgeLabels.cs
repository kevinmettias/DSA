namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// How a compressed trie reads the label on the edge into each node: a run of letters rather than
// one, which is what path compression changes about a trie (a radix or Patricia trie). Every node
// with a single child folds into its child's edge, so the edges below a branch carry the letters a
// plain trie spells one node at a time. A store still finds a child by the first letter on its
// edge, so the walk asks the labels only about the rest of the edge.
//
// Where a label's letters live is the configuration's own, so they are read through this witness
// rather than kept by the walk: the suffix tree's edges are ranges of the one text it indexes, kept
// on each node as a start and a length (SuffixTreeEdges). A witness that needs state of its own,
// the text there, holds it, and the walk takes the witness as a value, so a struct witness is
// compiled into the walk with its calls direct. An edge of one letter is the uncompressed trie's
// edge, which TrieWalk's plain Find walks with no labels at all.
/// <summary>
/// How a trie whose edges each carry a run of letters reads the label on the edge into a node.
/// </summary>
/// <typeparam name="TNode">The node class whose incoming edge the label is on.</typeparam>
/// <typeparam name="TLetter">One letter of a label.</typeparam>
public interface ITrieEdgeLabels<TNode, TLetter>
    where TNode : class
{
    /// <summary>Counts the letters on the edge into a node.</summary>
    /// <param name="node">The node below the edge; never the root, which no edge enters.</param>
    /// <returns>How many letters label the edge, at least one.</returns>
    int LengthOf(TNode node);

    /// <summary>Reports whether a letter of the edge into a node is the given one.</summary>
    /// <param name="node">The node below the edge.</param>
    /// <param name="offset">
    /// The letter's place along the edge, from 0 for its first to <see cref="LengthOf"/> - 1.
    /// </param>
    /// <param name="letter">The letter to compare with.</param>
    /// <returns>True when the edge has that letter at that place.</returns>
    bool HasLetterAt(TNode node, int offset, TLetter letter);
}
