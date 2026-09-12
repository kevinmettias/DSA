using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SerializeAndDeserializeBST;

// LeetCode 449. Serialize and Deserialize BST: unlike the general Binary Tree
// version (LC 297, see SerializeAndDeserializeBinaryTreeSolution), a BST's own
// ordering invariant means a pre-order VALUE sequence alone (no null markers) is
// enough to reconstruct the exact same shape.
//
// SerializeByNullMarkerQueue/DeserializeByNullMarkerQueue is the naive baseline: it
// ignores the BST invariant entirely and falls back to LC 297's general
// null-marker grammar - a token for every missing child, replayed through a plain
// BCL Queue<string> - deliberately written without exploiting anything about this
// tree being a BST. SerializeByPreOrderValues/DeserializeByBstInsert instead emits
// pre-order VALUES ONLY and replays them through this repo's own
// BinarySearchTree<int>.Insert, whose own less-than/greater-than walk is exactly
// the walk that produced pre-order in the first place - no null markers, no queue.
internal static class SerializeAndDeserializeBSTSolution
{
    private const string NullMarker = "#";
    private const char TokenSeparator = ',';

    public static string SerializeByNullMarkerQueue(BinaryTreeNode<int>? root)
    {
        var tokens = new List<string>();
        WriteWithNullMarkers(root, tokens);
        return string.Join(TokenSeparator, tokens);
    }

    private static void WriteWithNullMarkers(BinaryTreeNode<int>? node, List<string> tokens)
    {
        if (node is null)
        {
            tokens.Add(NullMarker);
            return;
        }

        tokens.Add(node.Value.ToString());
        WriteWithNullMarkers(node.Left, tokens);
        WriteWithNullMarkers(node.Right, tokens);
    }

    public static BinaryTreeNode<int>? DeserializeByNullMarkerQueue(string data)
    {
        var tokens = new Queue<string>(data.Split(TokenSeparator));
        return ReadWithNullMarkers(tokens);
    }

    private static BinaryTreeNode<int>? ReadWithNullMarkers(Queue<string> tokens)
    {
        if (!tokens.TryDequeue(out var token) || token == NullMarker)
        {
            return null;
        }

        return new BinaryTreeNode<int>(int.Parse(token))
        {
            Left = ReadWithNullMarkers(tokens),
            Right = ReadWithNullMarkers(tokens),
        };
    }

    public static string SerializeByPreOrderValues(BinaryTreeNode<int>? root)
    {
        var tokens = new List<string>();
        WriteValuesOnly(root, tokens);
        return string.Join(TokenSeparator, tokens);
    }

    private static void WriteValuesOnly(BinaryTreeNode<int>? node, List<string> tokens)
    {
        if (node is null)
        {
            return;
        }

        tokens.Add(node.Value.ToString());
        WriteValuesOnly(node.Left, tokens);
        WriteValuesOnly(node.Right, tokens);
    }

    public static BinaryTreeNode<int>? DeserializeByBstInsert(string data)
    {
        if (data.Length == 0)
        {
            return null;
        }

        var tree = new BinarySearchTree<int>();

        foreach (var token in data.Split(TokenSeparator))
        {
            tree.Insert(int.Parse(token));
        }

        return tree.Root;
    }
}
