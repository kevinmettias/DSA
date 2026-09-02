using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.RecoverBinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RecoverBinarySearchTreeSolution's, the same methods
// RecoverBinarySearchTreeTests proves correct. Both strategies mutate the tree they
// are handed - LeetCode's actual recoverTree operation - so [IterationSetup] rebuilds
// a fresh corrupted BST before every iteration rather than reusing the one
// [GlobalSetup] built, which a single successful recovery would leave sorted.
[MemoryDiagnoser]
public class RecoverBinarySearchTreeBenchmarks
{
    [Params(100, 5_000)]
    public int Size;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(Size);

    [IterationSetup]
    public void IterationSetup() => _root = RecoverBinarySearchTreeWorkloads.BuildCorruptedBst(Size);

    [Benchmark(Baseline = true)]
    public void ManualRecursiveScan() => RecoverBinarySearchTreeSolution.RecoverByManualRecursiveScan(_root);

    [Benchmark]
    public void InOrderTraversalHooks() => RecoverBinarySearchTreeSolution.RecoverByInOrderHooks(_root);
}
