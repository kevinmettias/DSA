namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A node of a rooted n-ary tree over dense integer ids - the shape LeetCode hands
// out as a parent array (prevRoom[], parent[], ...), where edges point parent ->
// child and index 0 is the root.
//
// Distinct from DataStructures' BinaryTreeNode, which fixes arity at two, and from
// the general graph contracts, which promise no ancestry at all.
internal sealed class RootedTreeNode(int id)
{
    public int Id { get; } = id;

    public List<RootedTreeNode> Children { get; } = [];

    public override string ToString() => Id.ToString();
}
