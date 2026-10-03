using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MergeBSTsToCreateSingleBST;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeBSTsToCreateSingleBSTSolution's, the same methods
// MergeBSTsToCreateSingleBSTSolutionTests proves correct. Every tree in the chain is a tiny
// two-node stub whose leaf value equals the next tree's root value, so the merge
// always succeeds and the only thing TreeCount changes is how "find the tree whose
// root matches this leaf" is answered - a linear rescan of the tree list
// (O(TreeCount) per leaf, O(TreeCount^2) overall) vs. this repo's own
// HashMap<int,BinaryTreeNode<int>> (O(1) per leaf), the same "index it instead of
// rescanning" contrast TwoSumBenchmarks draws.
//
// The forest itself is built once in [GlobalSetup], but it cannot be handed to a
// hoisted overload: splicing consumes it, so each invocation clones the template
// first - identically for both arms, exactly as the pre-migration harness did.
// Each arm returns LeetCode's real answer, the merged root, as object because
// BinaryTreeNode<int> is internal and a public [Benchmark] method cannot name it.
public class MergeBSTsToCreateSingleBSTBenchmarks
{
    private List<BinaryTreeNode<int>> _template = new();

    [Params(50, 500)]
    public int TreeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _template = MergeBinarySearchTreeWorkloads.BuildChain(TreeCount);

    [Benchmark(Baseline = true)]
    public object? CanMergeByLinearScan() =>
        MergeBSTsToCreateSingleBSTSolution.CanMergeByLinearScan(
            MergeBinarySearchTreeWorkloads.Clone(_template));

    [Benchmark]
    public object? CanMergeByHashMapIndex() =>
        MergeBSTsToCreateSingleBSTSolution.CanMergeByHashMapIndex(
            MergeBinarySearchTreeWorkloads.Clone(_template));
}
