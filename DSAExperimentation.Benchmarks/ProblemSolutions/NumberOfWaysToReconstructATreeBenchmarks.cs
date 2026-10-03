using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfWaysToReconstructATree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfWaysToReconstructATreeSolution's, the same
// methods NumberOfWaysToReconstructATreeSolutionTests proves correct. The workload is
// ReconstructTreeWorkloads' star of chains, sized here, so every non-root node
// clears the initial degree check and both arms walk the full
// candidate-parent/subset-check logic rather than short-circuiting. NodeCount stops
// at 500, the highest node label LC 1719 allows.
public class NumberOfWaysToReconstructATreeBenchmarks
{
    private const int ChainCount = 5;

    private int[][] _pairs = [];

    [Params(50, 500)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _pairs = ReconstructTreeWorkloads.BuildStarOfChainsPairs(NodeCount, ChainCount);

    [Benchmark(Baseline = true)]
    public int DictionaryAndLinqSort() =>
        NumberOfWaysToReconstructATreeSolution.CheckWaysByDictionaryAndLinqSort(_pairs);

    [Benchmark]
    public int HashMapAndMergeSort() =>
        NumberOfWaysToReconstructATreeSolution.CheckWaysByHashMapAndMergeSort(_pairs);
}
