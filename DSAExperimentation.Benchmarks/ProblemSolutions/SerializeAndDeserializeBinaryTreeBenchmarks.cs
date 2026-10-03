using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SerializeAndDeserializeBinaryTreeSolution's, the same
// methods SerializeAndDeserializeBinaryTreeSolutionTests proves correct. Each arm builds the
// serialized string, rebuilds the tree from it in full and returns the rebuilt root, as
// object because BinaryTreeNode<int> is internal and a [Benchmark] method must be public.
public class SerializeAndDeserializeBinaryTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(2_000, 8_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public object? StringConcatRoundTrip() =>
        SerializeAndDeserializeBinaryTreeSolution.DeserializeByStringConcat(
            SerializeAndDeserializeBinaryTreeSolution.SerializeByStringConcat(_root));

    [Benchmark]
    public object? QueueRoundTrip() =>
        SerializeAndDeserializeBinaryTreeSolution.DeserializeByQueue(
            SerializeAndDeserializeBinaryTreeSolution.SerializeByQueue(_root));
}
