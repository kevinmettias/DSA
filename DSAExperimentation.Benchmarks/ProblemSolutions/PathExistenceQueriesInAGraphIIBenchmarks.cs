using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathExistenceQueriesInAGraphIISolution's, the same
// methods PathExistenceQueriesInAGraphIITests proves correct. Sorting nums by
// value is preprocessing either strategy needs, so [GlobalSetup] builds it once
// via SortedByValueGraph and hands each arm the prepared shape its own hoisted
// overload takes - plain sorted arrays for the BFS baseline, the graph witness
// for the binary-lifting arm - so answering the queries is what gets measured,
// not the sort.
[MemoryDiagnoser]
public class PathExistenceQueriesInAGraphIIBenchmarks
{
    // LC problem number, reused as the deterministic workload seed.
    private const int Seed = 3534;
    private const int MaxDiff = 500;
    private const int QueryCount = 300;

    private SortedByValueGraph _graph = null!;

    private int[][] _queries = [];
    [Params(500, 4_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed + NodeCount);
        var nums = Enumerable.Range(0, NodeCount).Select(_ => random.Next(0, 100_000)).ToArray();
        _graph = SortedByValueGraph.Build(nums);
        _queries = [.. Enumerable.Range(0, QueryCount).Select(_ => new[] { random.Next(NodeCount), random.Next(NodeCount) })];
    }

    [Benchmark(Baseline = true)]
    public int[] RangeBfs() => PathExistenceQueriesInAGraphIISolution.MinDistancesByRangeBfs(
        _graph.SortedValues, _graph.PositionOf, MaxDiff, _queries);

    [Benchmark]
    public int[] BinaryLifting() =>
        PathExistenceQueriesInAGraphIISolution.MinDistancesByBinaryLifting(_graph, MaxDiff, _queries);
}
