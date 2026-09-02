using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SerializeAndDeserializeBinaryTreeSolution's, the same
// methods SerializeAndDeserializeBinaryTreeTests proves correct. Each arm builds the
// serialized string and then rebuilds the tree from it in full; CountNodes only
// exists to give a [Benchmark] method (which must be public) a public return value
// for an internal BinaryTreeNode<int>, the same technique
// ConstructBinaryTreeFromPreorderAndInorderTraversalBenchmarks uses.
[MemoryDiagnoser]
public class SerializeAndDeserializeBinaryTreeBenchmarks
{
    [Params(2_000, 8_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int StringConcatRoundTrip() =>
        CountNodes(SerializeAndDeserializeBinaryTreeSolution.DeserializeByStringConcat(
            SerializeAndDeserializeBinaryTreeSolution.SerializeByStringConcat(_root)));

    [Benchmark]
    public int QueueRoundTrip() =>
        CountNodes(SerializeAndDeserializeBinaryTreeSolution.DeserializeByQueue(
            SerializeAndDeserializeBinaryTreeSolution.SerializeByQueue(_root)));

    private static int CountNodes(BinaryTreeNode<int>? node) =>
        node is null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);
}
