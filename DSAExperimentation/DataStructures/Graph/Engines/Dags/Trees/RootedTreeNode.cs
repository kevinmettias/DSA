namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A node of a rooted n-ary tree over dense integer ids - the shape LeetCode hands
// out as a parent array (prevRoom[], parent[], ...), where edges point parent ->
// child and index 0 is the root.
//
// Distinct from DataStructures' BinaryTreeNode, which fixes arity at two, and from
// the general graph contracts, which promise no ancestry at all.
/// <summary>A node of a rooted n-ary tree whose nodes are numbered with dense integer ids.</summary>
/// <param name="id">The node's id, from 0 to n - 1 in a tree of n nodes.</param>
public sealed class RootedTreeNode(int id)
{
    /// <summary>The node's id, from 0 to n - 1 in a tree of n nodes.</summary>
    public int Id { get; } = id;

    /// <summary>The node's children, in the order they were added, and empty for a leaf.</summary>
    public List<RootedTreeNode> Children { get; } = [];

    public override string ToString() => Id.ToString();
}
