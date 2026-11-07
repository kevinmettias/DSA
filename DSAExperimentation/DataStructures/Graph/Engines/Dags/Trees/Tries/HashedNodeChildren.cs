using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Children keyed by any character, in this repo's HashMap, for a node class of a configuration's
// own: what HashedChildren is to a map's TrieNode, for any node class, as BinarySlots is for any
// bit-trie node. Both exist because a store names the node class its children are. TrieNode is
// generic over its store, so a store for it must name TrieNode itself and be generic over the
// value; a node class that is not generic over its store, as the suffix tree's is not, takes a
// store generic over the node class. TrieNodeChildren and TrieChildren are the same pair of tree
// witnesses.
//
// A child is found in O(1) on average, the hash map's own caveat. The map keeps no order, so this
// store is not ordered and its trie has no tree-topology witness; a walk over every child reads
// Letters or Nodes, each a copy taken when read, in no order. A trie whose edges carry runs of
// letters splits an edge where a key leaves it partway, and hangs the node that was below the edge
// under the split, a node that already exists, so the store also puts a given node under a letter
// (SetChildFor), which GetOrAddChildFor cannot, since it makes its own.
/// <summary>
/// A trie node's children keyed by any character, in a hash map, for any node class.
/// </summary>
/// <typeparam name="TNode">The node class the children are.</typeparam>
public readonly struct HashedNodeChildren<TNode>
    : ILetteredTrieChildStore<HashedNodeChildren<TNode>, TNode, char>
    where TNode : class, ITrieNode<TNode, HashedNodeChildren<TNode>>
{
    private readonly HashMap<char, TNode> _children;

    /// <summary>
    /// Gets the letters the node has a child for, copied when read, in no particular order.
    /// </summary>
    public IEnumerable<char> Letters => _children.Keys;

    /// <summary>Gets the node's children, copied when read, in no particular order.</summary>
    public IEnumerable<TNode> Nodes => _children.Values;

    private HashedNodeChildren(HashMap<char, TNode> children) => _children = children;

    /// <inheritdoc/>
    public static HashedNodeChildren<TNode> Empty() => new(new HashMap<char, TNode>());

    /// <inheritdoc/>
    public TNode? ChildFor(char key)
    {
        if (!_children.TryGetValue(key, out var child))
        {
            return null;
        }

        return child;
    }

    /// <inheritdoc/>
    public TNode GetOrAddChildFor(char key)
    {
        if (_children.TryGetValue(key, out var child))
        {
            return child;
        }

        var added = TNode.New();
        _children.Set(key, added);

        return added;
    }

    /// <summary>Puts a node under a letter, in place of the child there if there is one.</summary>
    /// <param name="key">The letter on the edge to the node.</param>
    /// <param name="child">The node to put there, which may already belong to the trie.</param>
    public void SetChildFor(char key, TNode child) => _children.Set(key, child);
}
