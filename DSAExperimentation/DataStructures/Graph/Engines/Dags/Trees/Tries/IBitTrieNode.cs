using System.Diagnostics.CodeAnalysis;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A node of a trie over an int's bits, one child per binary digit kept in BinarySlots, whose
// payload also says which children hold a value. The maximum-XOR walk (MaxXorWalk) asks that of the
// child it would like to take, and the answer is the payload's:
// - BitTrieNode carries nothing and nothing is ever removed from its trie, so a child that exists
//   holds a value: its path was laid down by one.
// - CountedBitTrieNode counts the values whose path passes through it, and a removal lowers the
//   counts but leaves the nodes, so a child holds a value only while its count is positive.
/// <summary>A bit trie's node, whose payload says whether a child holds any value.</summary>
/// <typeparam name="TSelf">The node type itself.</typeparam>
public interface IBitTrieNode<TSelf> : ITrieNode<TSelf, BinarySlots<TSelf>>
    where TSelf : class, IBitTrieNode<TSelf>
{
    /// <summary>Reports whether a child, or its absence, holds any value.</summary>
    /// <param name="child">The child to ask about, or <c>null</c> when there is none.</param>
    /// <returns>
    /// True when some held value's path passes through the child, which is then not null.
    /// </returns>
    static abstract bool IsLive([NotNullWhen(true)] TSelf? child);
}
