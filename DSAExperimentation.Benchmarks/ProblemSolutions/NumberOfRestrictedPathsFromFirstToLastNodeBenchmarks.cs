using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfRestrictedPathsFromFirstToLastNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfRestrictedPathsFromFirstToLastNodeSolution's,
// the same methods NumberOfRestrictedPathsFromFirstToLastNodeTests proves correct.
// Each is handed the prepared RestrictedPathGraph its hoisted overload takes, so the
// shared Dijkstra distance labelling is charged to [GlobalSetup] and the measured
// difference stays purely how the restricted paths are counted afterwards.
[MemoryDiagnoser]
public class NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks
{
    // Kept modest (<=30), same reasoning as FibonacciBenchmarks: NaiveDfs's blowup
    // here really is O(golden-ratio^N).
    [Params(20, 30)]
    public int N;

    private RestrictedPathGraph _graph = null!;

    [GlobalSetup]
    public void Setup() =>
        _graph = RestrictedPathGraph.Build(
            RestrictedPathWorkloads.NodeCount(N), RestrictedPathWorkloads.BuildTwoStepEdges(N));

    [Benchmark(Baseline = true)]
    public long NaiveDfs() =>
        NumberOfRestrictedPathsFromFirstToLastNodeSolution.CountRestrictedPathsByNaiveDfs(_graph);

    [Benchmark]
    public long DagFoldMemoized() =>
        NumberOfRestrictedPathsFromFirstToLastNodeSolution.CountRestrictedPathsByDagFold(_graph);
}
