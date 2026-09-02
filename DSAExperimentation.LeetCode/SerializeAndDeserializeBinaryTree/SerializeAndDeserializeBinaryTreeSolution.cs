using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.SerializeAndDeserializeBinaryTree;

// LeetCode 297. Serialize and Deserialize Binary Tree: design an algorithm to
// serialize a binary tree to a string and deserialize that string back to the
// exact same tree, using a "#" null-marker sentinel for missing children.
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
internal static class SerializeAndDeserializeBinaryTreeSolution
{
    private const string NullMarker = "#";
    private const char TokenSeparator = ',';

    // The textbook answer: string += in a closure, deliberately written without
    // this repo's primitives - it is the arm the composed solution below has to
    // justify itself against.
    public static string SerializeByStringConcat(BinaryTreeNode<int>? root)
    {
        var accumulated = "";

        void Visit(BinaryTreeNode<int>? node)
        {
            if (node is null)
            {
                accumulated += NullMarker + TokenSeparator;
                return;
            }

            accumulated += node.Value + TokenSeparator.ToString();
            Visit(node.Left);
            Visit(node.Right);
        }

        Visit(root);
        return accumulated;
    }

    public static BinaryTreeNode<int>? DeserializeByStringConcat(string data)
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

    // Accumulate tokens in a List<string> and join once, rather than reallocating
    // a string on every node.
    public static string SerializeByQueue(BinaryTreeNode<int>? root)
    {
        var tokens = new List<string>();
        WriteNode(root, tokens);
        return string.Join(TokenSeparator, tokens);
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

    // This repo's own Queue<string> replays the tokens back into a tree by
    // dequeuing them in the exact preorder they were written - no index bookkeeping.
    public static BinaryTreeNode<int>? DeserializeByQueue(string data)
    {
        var tokens = new RepoQueue();

        foreach (var token in data.Split(TokenSeparator))
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
}
