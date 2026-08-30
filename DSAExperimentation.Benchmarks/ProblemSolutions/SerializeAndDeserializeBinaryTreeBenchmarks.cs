using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Serialize and Deserialize Binary Tree (LC 297): a naive round trip that serializes
// by appending onto one shared accumulator string ("+=" reallocates and copies the
// whole string built so far on every node, O(n^2) total across an n-node tree) vs.
// this repo's BinaryTreeNode<TValue> tree paired with a List<string> joined once
// (O(n)) and a Queue<string> that replays the tokens back into a tree by dequeuing
// them in the exact preorder they were written.
[MemoryDiagnoser]
public class SerializeAndDeserializeBinaryTreeBenchmarks
{
    [Params(2_000, 8_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int StringConcatRoundTrip()
    {
        var serialized = ConcatSerialize(_root);
        var restored = ConcatDeserialize(serialized);
        return CountNodes(restored);
    }

    [Benchmark]
    public int QueueRoundTrip()
    {
        var tokens = new List<string>();
        WriteNode(_root, tokens);
        var serialized = string.Join(',', tokens);

        var restored = QueueDeserialize(serialized);
        return CountNodes(restored);
    }

    private static string ConcatSerialize(BinaryTreeNode<int>? root)
    {
        var accumulated = "";
        void Visit(BinaryTreeNode<int>? node)
        {
            if (node is null)
            {
                accumulated += "#,";
                return;
            }

            accumulated += node.Value + ",";
            Visit(node.Left);
            Visit(node.Right);
        }
        Visit(root);
        return accumulated;
    }

    private static BinaryTreeNode<int>? ConcatDeserialize(string data)
    {
        var tokens = data.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var index = 0;

        BinaryTreeNode<int>? Build()
        {
            var token = tokens[index++];
            return token == "#" ? null : new BinaryTreeNode<int>(int.Parse(token)) { Left = Build(), Right = Build() };
        }

        return Build();
    }

    private static void WriteNode(BinaryTreeNode<int>? node, List<string> tokens)
    {
        if (node is null)
        {
            tokens.Add("#");
            return;
        }

        tokens.Add(node.Value.ToString());
        WriteNode(node.Left, tokens);
        WriteNode(node.Right, tokens);
    }

    private static BinaryTreeNode<int>? QueueDeserialize(string data)
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
        if (!tokens.TryDequeue(out var token) || token == "#")
        {
            return null;
        }

        return new BinaryTreeNode<int>(int.Parse(token)) { Left = ReadNode(tokens), Right = ReadNode(tokens) };
    }

    private static int CountNodes(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
}
