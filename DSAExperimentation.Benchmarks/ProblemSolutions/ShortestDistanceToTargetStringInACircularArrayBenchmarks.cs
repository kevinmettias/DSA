using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ShortestDistanceToTargetStringInACircularArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// ShortestDistanceToTargetStringInACircularArraySolution's, the same methods
// ShortestDistanceToTargetStringInACircularArrayTests proves correct. A direct
// min(diff, n - diff) linear scan over the raw words array is measured against
// this repo's own BFS - Reduce.Graph + DistanceMapReduceAlgebra over a
// CircularArrayGraph, where each node's two edges stand in for "one step left" /
// "one step right" around the circle. The target sits diametrically opposite
// startIndex (CircularArrayWorkloads), so neither approach gets to short-circuit
// on an immediate neighbor match.
//
// The BFS arm is handed a prepared CircularArrayGraph so node construction is
// charged to [GlobalSetup] rather than to the search (#17.4); the scan arm takes
// LeetCode's own words[] because that is already its input.
[MemoryDiagnoser]
public class ShortestDistanceToTargetStringInACircularArrayBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string[] _words = null!;
    private CircularArrayGraph _graph;

    [GlobalSetup]
    public void Setup()
    {
        _words = CircularArrayWorkloads.BuildWords(Length);
        _graph = CircularArrayGraph.Build(_words);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() =>
        ShortestDistanceToTargetStringInACircularArraySolution.ClosestTargetByLinearScan(
            _words, CircularArrayWorkloads.Target, CircularArrayWorkloads.StartIndex);

    [Benchmark]
    public int ReduceGraphBfs() =>
        ShortestDistanceToTargetStringInACircularArraySolution.ClosestTargetByReduceGraph(
            _graph, CircularArrayWorkloads.Target, CircularArrayWorkloads.StartIndex);
}
