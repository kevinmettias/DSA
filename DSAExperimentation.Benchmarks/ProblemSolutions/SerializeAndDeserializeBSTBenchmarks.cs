using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SerializeAndDeserializeBSTSolution's, the same
// methods SerializeAndDeserializeBSTTests proves correct. The workload tree is
// built once in GlobalSetup - via a manual, non-repo insert over a shuffled
// insertion order - so neither arm's timing is charged for tree construction, only
// for the round trip through its own serialization grammar. CountNodes only exists
// to give a [Benchmark] method (which must be public) a public return value for an
// internal BinaryTreeNode<int>, the same technique
// SerializeAndDeserializeBinaryTreeBenchmarks uses.
[MemoryDiagnoser]
public class SerializeAndDeserializeBSTBenchmarks
{
    [Params(500, 20_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

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

        _root = BuildManual(values);
    }

    [Benchmark(Baseline = true)]
    public int NullMarkerQueueRoundTrip() =>
        CountNodes(SerializeAndDeserializeBSTSolution.DeserializeByNullMarkerQueue(
            SerializeAndDeserializeBSTSolution.SerializeByNullMarkerQueue(_root)));

    [Benchmark]
    public int PreOrderValueOnlyRoundTrip() =>
        CountNodes(SerializeAndDeserializeBSTSolution.DeserializeByBstInsert(
            SerializeAndDeserializeBSTSolution.SerializeByPreOrderValues(_root)));

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

    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
}
