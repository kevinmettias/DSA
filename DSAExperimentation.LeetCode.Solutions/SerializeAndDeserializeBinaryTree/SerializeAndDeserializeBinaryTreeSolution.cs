using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.SerializeAndDeserializeBinaryTree;

// LeetCode 297. Serialize and Deserialize Binary Tree: design an algorithm to
// serialize a binary tree to a string and deserialize that string back to the
// exact same tree, using a "#" null-marker sentinel for missing children.
//
// LeetCode leaves the encoding free and judges only the round trip: its judge
// builds the tree from its level order, serializes it, deserializes that string
// with a fresh codec, and prints the copy, which must be the tree it started from.
// Each strategy is that round trip (RoundTripBy<Strategy>), over its codec's two
// halves in a nested class (<Strategy>Codec.Serialize and .Deserialize), which a
// test or a benchmark can call on their own.
//
// Both strategies are a preorder walk with the same token grammar - value, then
// left, then right, "#" standing in for a missing child - and differ only in how
// they build the token string and read it back. StringConcat is the naive
// baseline: it appends onto one shared accumulator string, which reallocates and
// copies the whole string built so far on every node (O(n^2) total across an
// n-node tree), and reads tokens back by index into a fixed array. Queue joins a
// List<string> once (O(n)) and replays the tokens back into a tree with this
// repo's own Queue<string>, dequeuing them in the exact preorder they were
// written.
[LeetCodeQuestion("serialize-and-deserialize-binary-tree", LeetCodeAnswerOrder.AsWritten)]
internal static class SerializeAndDeserializeBinaryTreeSolution
{
    private const string NullMarker = "#";
    private const char TokenSeparator = ',';

    public static BinaryTreeNode<int>? RoundTripByStringConcat(BinaryTreeNode<int>? root)
    {
        var text = StringConcatCodec.Serialize(root);
        return StringConcatCodec.Deserialize(text);
    }

    public static BinaryTreeNode<int>? RoundTripByQueue(BinaryTreeNode<int>? root)
    {
        var text = QueueCodec.Serialize(root);
        return QueueCodec.Deserialize(text);
    }

    // The textbook answer: string += in a closure, deliberately written without
    // this repo's primitives - it is the codec the composed one below has to
    // justify itself against. LeetCode accepts it, as it accepts any encoding that
    // reads back as the same tree.
    internal static class StringConcatCodec
    {
        public static string Serialize(BinaryTreeNode<int>? root)
        {
            var accumulated = "";

            void VisitPreorder(BinaryTreeNode<int>? node)
            {
                if (node is null)
                {
                    accumulated += NullMarker + TokenSeparator;
                    return;
                }

                accumulated += node.Value + TokenSeparator.ToString();
                VisitPreorder(node.Left);
                VisitPreorder(node.Right);
            }

            VisitPreorder(root);
            return accumulated;
        }

        public static BinaryTreeNode<int>? Deserialize(string data)
        {
            var tokens = data.Split(TokenSeparator, StringSplitOptions.RemoveEmptyEntries);
            var index = 0;

            BinaryTreeNode<int>? Build()
            {
                var token = tokens[index++];
                return token == NullMarker
                    ? null
                    : new BinaryTreeNode<int>(int.Parse(token)) { Left = Build(), Right = Build() };
            }

            return Build();
        }
    }

    // Accumulate tokens in a List<string> and join once, rather than reallocating
    // a string on every node; this repo's own Queue<string> replays the tokens
    // back into a tree by dequeuing them in the exact preorder they were written -
    // no index bookkeeping. LeetCode accepts it, as it accepts any encoding that
    // reads back as the same tree.
    internal static class QueueCodec
    {
        public static string Serialize(BinaryTreeNode<int>? root)
        {
            var tokens = new List<string>();
            WriteNode(root, tokens);
            return string.Join(TokenSeparator, tokens);
        }

        public static BinaryTreeNode<int>? Deserialize(string data)
        {
            var tokens = new RepoQueue();

            foreach (var token in data.Split(TokenSeparator))
            {
                tokens.Enqueue(token);
            }

            return ReadNode(tokens);
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

        private static BinaryTreeNode<int>? ReadNode(RepoQueue tokens)
        {
            if (!tokens.TryDequeue(out var token) || token == NullMarker)
            {
                return null;
            }

            return new BinaryTreeNode<int>(int.Parse(token)) { Left = ReadNode(tokens), Right = ReadNode(tokens) };
        }
    }
}
