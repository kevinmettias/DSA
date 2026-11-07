using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.SerializeAndDeserializeBST;

// LeetCode 449. Serialize and Deserialize BST: unlike the general Binary Tree
// version (LC 297, see SerializeAndDeserializeBinaryTreeSolution), a BST's own
// ordering invariant means a pre-order VALUE sequence alone (no null markers) is
// enough to reconstruct the exact same shape.
//
// LeetCode leaves the encoding free and judges only the round trip: its judge
// builds the search tree from its level order, serializes it, deserializes that
// string with a fresh codec, and prints the copy, which must be the tree it
// started from. Each strategy is that round trip (RoundTripBy<Strategy>), over its
// codec's two halves in a nested class (<Strategy>Codec.Serialize and
// .Deserialize), which a test or a benchmark can call on their own.
//
// NullMarkerQueue is the naive baseline: it ignores the BST invariant entirely and
// falls back to LC 297's general null-marker grammar - a token for every missing
// child, replayed through a plain BCL Queue<string> - deliberately written without
// exploiting anything about this tree being a BST. PreOrderValues instead emits
// pre-order VALUES ONLY and replays them through this repo's own
// BinarySearchTree<int>.Insert, whose own less-than/greater-than walk is exactly
// the walk that produced pre-order in the first place - no null markers, no queue.
[LeetCodeQuestion("serialize-and-deserialize-bst", LeetCodeAnswerOrder.AsWritten)]
internal static class SerializeAndDeserializeBSTSolution
{
    private const string NullMarker = "#";
    private const char TokenSeparator = ',';

    public static BinaryTreeNode<int>? RoundTripByNullMarkerQueue(BinaryTreeNode<int>? root)
    {
        var text = NullMarkerQueueCodec.Serialize(root);
        return NullMarkerQueueCodec.Deserialize(text);
    }

    public static BinaryTreeNode<int>? RoundTripByPreOrderValues(BinaryTreeNode<int>? root)
    {
        var text = PreOrderValuesCodec.Serialize(root);
        return PreOrderValuesCodec.Deserialize(text);
    }

    // LC 297's general grammar, a "#" for every missing child, which reads back any
    // binary tree and so any search tree too; LeetCode accepts it, as it accepts any
    // encoding that reads back as the same tree, though its follow-up asks for a
    // more compact one.
    internal static class NullMarkerQueueCodec
    {
        public static string Serialize(BinaryTreeNode<int>? root)
        {
            var tokens = new List<string>();
            WriteWithNullMarkers(root, tokens);
            return string.Join(TokenSeparator, tokens);
        }

        public static BinaryTreeNode<int>? Deserialize(string data)
        {
            var tokens = new Queue<string>(data.Split(TokenSeparator));
            return ReadWithNullMarkers(tokens);
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
    }

    // The values alone, in pre-order, rebuilt by inserting them into a search tree
    // in that order: LeetCode promises a search tree, whose shape its pre-order
    // values fix, so this compact encoding reads back as the same tree.
    internal static class PreOrderValuesCodec
    {
        public static string Serialize(BinaryTreeNode<int>? root)
        {
            var tokens = new List<string>();
            WriteValuesOnly(root, tokens);
            return string.Join(TokenSeparator, tokens);
        }

        public static BinaryTreeNode<int>? Deserialize(string data)
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
    }
}
