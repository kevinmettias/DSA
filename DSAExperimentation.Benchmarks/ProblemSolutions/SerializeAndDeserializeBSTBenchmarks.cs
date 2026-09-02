using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<string>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Serialize and Deserialize BST (LC 449): the general-binary-tree approach (LC 297,
// see SerializeAndDeserializeBinaryTreeBenchmarks) writes a null-marker token for
// every empty child and replays them through a Queue<string> - roughly double the
// token count and dequeues an n-node BST actually needs - vs. exploiting the BST
// ordering invariant directly: a pre-order VALUE-ONLY sequence, replayed through
// this repo's own BinarySearchTree<int>.Insert, reconstructs the identical shape
// with no null markers and no queue at all, since Insert's own comparison walk on
// each value IS the walk that produced pre-order in the first place.
[MemoryDiagnoser]
public class SerializeAndDeserializeBSTBenchmarks
{
    private const string NullMarker = "#";

    [Params(500, 20_000)]
    public int NodeCount;

    private int[] _insertionOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, NodeCount).ToArray();
        var random = new Random(1);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _insertionOrder = values;
    }

    [Benchmark(Baseline = true)]
    public int NullMarkerQueueRoundTrip()
    {
        var root = BuildManual(_insertionOrder);

        var tokens = new List<string>();
        WriteWithNullMarkers(root, tokens);
        var serialized = string.Join(',', tokens);

        return CountNodes(ReadWithNullMarkers(serialized));
    }

    [Benchmark]
    public int PreOrderValueOnlyRoundTrip()
    {
        var root = BuildManual(_insertionOrder);

        var tokens = new List<string>();
        WriteValuesOnly(root, tokens);
        var serialized = string.Join(',', tokens);

        return CountNodes(ReadViaBstInsert(serialized));
    }

    private static BinaryTreeNode<int> BuildManual(int[] values)
    {
        var root = new BinaryTreeNode<int>(values[0]);

        for (var i = 1; i < values.Length; i++)
        {
            InsertManual(root, values[i]);
        }

        return root;
    }

    private static void InsertManual(BinaryTreeNode<int> root, int value)
    {
        var node = root;

        while (true)
        {
            if (value < node.Value)
            {
                if (node.Left is null)
                {
                    node.Left = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Left;
            }
            else
            {
                if (node.Right is null)
                {
                    node.Right = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Right;
            }
        }
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

    private static BinaryTreeNode<int>? ReadWithNullMarkers(string data)
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

    private static BinaryTreeNode<int>? ReadViaBstInsert(string data)
    {
        var tree = new BinarySearchTree<int>();

        foreach (var token in data.Split(','))
        {
            tree.Insert(int.Parse(token));
        }

        return tree.Root;
    }

    private static int CountNodes(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
}
