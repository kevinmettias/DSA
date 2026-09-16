using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.FindIfPathExistsInGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindIfPathExistsInGraphSolution's, the same methods
// FindIfPathExistsInGraphTests proves correct. Source and destination sit in two
// disjoint spanning trees on every run - the worst case for the search arm, which
// must exhaust the whole source component before concluding no path exists - so
// both strategies do real, comparable work.
[MemoryDiagnoser]
public class FindIfPathExistsInGraphBenchmarks
{
    private const int RandomSeed = 1971; // LC problem number

    private int[][] _edges = [];

    private int _source;
    private int _destination;
    [Params(300, 3_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var half = NodeCount / AlgorithmConstants.HalvingFactor;
        var edges = new List<int[]>();

        // Two separate spanning trees - [0, half) and [half, NodeCount) - so
        // source (0) and destination (NodeCount - 1) never share a component.
        for (var i = 1; i < half; i++)
        {
            edges.Add([random.Next(i), i]);
        }

        for (var i = half + 1; i < NodeCount; i++)
        {
            edges.Add([half + random.Next(i - half), i]);
        }

        _edges = [.. edges];
        _source = 0;
        _destination = NodeCount - 1;
    }

    [Benchmark(Baseline = true)]
    public bool HasPathByDepthFirstSearch() =>
        FindIfPathExistsInGraphSolution.HasPathByDepthFirstSearch(NodeCount, _edges, _source, _destination);

    [Benchmark]
    public bool HasPathByDisjointSet() =>
        FindIfPathExistsInGraphSolution.HasPathByDisjointSet(NodeCount, _edges, _source, _destination);
}
