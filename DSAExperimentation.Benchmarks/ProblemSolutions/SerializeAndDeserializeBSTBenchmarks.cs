using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SerializeAndDeserializeBSTSolution's, the same
// methods SerializeAndDeserializeBSTSolutionTests proves correct. The workload tree is
// built once in GlobalSetup - via a manual, non-repo insert over a shuffled
// insertion order - so neither arm's timing is charged for tree construction, only
// for the round trip through its own serialization grammar. Each arm returns the
// rebuilt tree's root, as object because BinaryTreeNode<int> is internal and a
// [Benchmark] method must be public.
public class SerializeAndDeserializeBSTBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(500, 10_000)]
    public int NodeCount { get; set; }

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

        // Stops when the walk reaches the null child slot the value belongs in, where the new node is linked and the method returns.
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

    [Benchmark(Baseline = true)]
    public object? NullMarkerQueueRoundTrip() =>
        SerializeAndDeserializeBSTSolution.DeserializeByNullMarkerQueue(
            SerializeAndDeserializeBSTSolution.SerializeByNullMarkerQueue(_root));

    [Benchmark]
    public object? PreOrderValueOnlyRoundTrip() =>
        SerializeAndDeserializeBSTSolution.DeserializeByBstInsert(
            SerializeAndDeserializeBSTSolution.SerializeByPreOrderValues(_root));
}
