using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Solutions.ConvertSortedArrayToBinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is
// ConvertSortedArrayToBinarySearchTreeSolution's, the same method
// ConvertSortedArrayToBinarySearchTreeSolutionTests proves correct. The original
// benchmark's two [Benchmark] arms called the identical private helper - one
// real strategy, not two - so there is only one arm here too.
public class ConvertSortedArrayToBinarySearchTreeBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public BinaryTreeNode<int>? MidpointRecursion() =>
        ConvertSortedArrayToBinarySearchTreeSolution.BuildByMidpointRecursion(_values);
}
