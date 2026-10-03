using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SerializeAndDeserializeBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SerializeAndDeserializeBinaryTreeSolution's, the same
// methods SerializeAndDeserializeBinaryTreeSolutionTests proves correct. Each arm builds the
// serialized string, rebuilds the tree from it in full and returns the rebuilt root, as
// object because BinaryTreeNode<int> is internal and a [Benchmark] method must be public.
//
// The tree is complete - LeetCode's level-order array with no gaps - with values drawn from
// a seeded Random across LC 297's whole [-1000, 1000], so the serialized form carries signs
// and every digit count a value can have.
public class SerializeAndDeserializeBinaryTreeBenchmarks
{
    private const int RandomSeed = 297; // LC problem number
    private const int MinValue = -1_000;
    private const int MaxValue = 1_000;

    private BinaryTreeNode<int> _root = null!;

    [Params(2_000, 8_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var levelOrder = SeededDraws.Values(NodeCount, MinValue, MaxValue + 1, new Random(RandomSeed));
        _root = BinaryTrees.Complete(levelOrder);
    }

    [Benchmark(Baseline = true)]
    public object? StringConcatRoundTrip() =>
        SerializeAndDeserializeBinaryTreeSolution.DeserializeByStringConcat(
            SerializeAndDeserializeBinaryTreeSolution.SerializeByStringConcat(_root));

    [Benchmark]
    public object? QueueRoundTrip() =>
        SerializeAndDeserializeBinaryTreeSolution.DeserializeByQueue(
            SerializeAndDeserializeBinaryTreeSolution.SerializeByQueue(_root));
}
