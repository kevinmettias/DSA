using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SerializeAndDeserializeBST;

// LeetCode 449. Serialize and Deserialize BST: unlike the general Binary Tree
// version (LC 297, see SerializeAndDeserializeBinaryTreeTests), a BST's own
// ordering invariant means a pre-order VALUE sequence alone (no null markers) is
// enough to reconstruct the exact same shape - re-inserting those same values, in
// that same pre-order, into a fresh tree via this repo's own
// BinarySearchTree<TValue>.Insert always re-derives identical left/right
// placement, since Insert's own less-than/greater-than walk is exactly the walk
// that produced pre-order in the first place. Serialize itself is a thin local
// pre-order walk over BinaryTreeNode<TValue>, the same shape
// SerializeAndDeserializeBinaryTreeTests already uses for its own WriteNode - just
// without the null-marker token, since a BST never needs one to round-trip.
public sealed partial class SerializeAndDeserializeBSTTests
{
    [Fact]
    public void SerializeThenDeserialize_ClassicExample_RoundTripsToSameShape()
    {
        // [5,3,6,2,4]
        var root = new BinaryTreeNode<int>(5) { Left = new(3) { Left = new(2), Right = new(4) }, Right = new(6) };

        var restored = Deserialize(Serialize(root));

        Assert.Equal(PreOrder(root), PreOrder(restored));
    }

    [Fact]
    public void SerializeThenDeserialize_EmptyTree_RoundTripsToNull()
        => Assert.Null(Deserialize(Serialize(null)));

    [Fact]
    public void SerializeThenDeserialize_SingleNode_RoundTrips()
    {
        var root = new BinaryTreeNode<int>(42);

        var restored = Deserialize(Serialize(root));

        Assert.Equal(PreOrder(root), PreOrder(restored));
    }

    private static string Serialize(BinaryTreeNode<int>? root)
    {
        var tokens = new List<string>();
        WritePreOrder(root, tokens);
        return string.Join(',', tokens);
    }

    private static void WritePreOrder(BinaryTreeNode<int>? node, List<string> tokens)
    {
        if (node is null)
        {
            return;
        }

        tokens.Add(node.Value.ToString());
        WritePreOrder(node.Left, tokens);
        WritePreOrder(node.Right, tokens);
    }

    private static BinaryTreeNode<int>? Deserialize(string data)
    {
        if (data.Length == 0)
        {
            return null;
        }

        var tree = new BinarySearchTree<int>();

        foreach (var token in data.Split(','))
        {
            tree.Insert(int.Parse(token));
        }

        return tree.Root;
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root)
        => root is null ? [] : [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
}
