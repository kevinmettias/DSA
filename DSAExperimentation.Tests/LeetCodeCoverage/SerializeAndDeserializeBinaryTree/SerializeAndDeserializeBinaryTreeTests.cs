using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SerializeAndDeserializeBinaryTree;

// LeetCode 297. Serialize and Deserialize Binary Tree: preorder serialization with a
// "#" null-marker sentinel, using this repo's own BinaryTreeNode<TValue> as the tree
// representation and Queue<string> to walk the serialized tokens back into a tree in
// the exact order they were written - the same recursive preorder shape
// ConstructBinaryTreeFromPreorderAndInorderTraversalTests already builds a tree with.
public sealed partial class SerializeAndDeserializeBinaryTreeTests
{
    private const string NullMarker = "#";

    [Fact]
    public void SerializeThenDeserialize_ClassicExample_RoundTripsPreOrder()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new BinaryTreeNode<int>(2), Right = new BinaryTreeNode<int>(3) { Left = new BinaryTreeNode<int>(4), Right = new BinaryTreeNode<int>(5) } };

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
        WriteNode(root, tokens);
        return string.Join(',', tokens);
    }

    private static void WriteNode(BinaryTreeNode<int>? node, List<string> tokens)
    {
        if (node is null)
        {
            tokens.Add(NullMarker);
            return;
        }

        tokens.Add(node.Value.ToString());
        WriteNode(node.Left, tokens);
        WriteNode(node.Right, tokens);
    }

    private static BinaryTreeNode<int>? Deserialize(string data)
    {
        var tokens = new RepoQueue();

        foreach (var token in data.Split(','))
        {
            tokens.Enqueue(token);
        }

        return ReadNode(tokens);
    }

    private static BinaryTreeNode<int>? ReadNode(RepoQueue tokens)
    {
        if (!tokens.TryDequeue(out var token) || token == NullMarker)
        {
            return null;
        }

        return new BinaryTreeNode<int>(int.Parse(token)) { Left = ReadNode(tokens), Right = ReadNode(tokens) };
    }

    private static int[] PreOrder(BinaryTreeNode<int>? root)
        => root is null ? [] : [root.Value, .. PreOrder(root.Left), .. PreOrder(root.Right)];
}
