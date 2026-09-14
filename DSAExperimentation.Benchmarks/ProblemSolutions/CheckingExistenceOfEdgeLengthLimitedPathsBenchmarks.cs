using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckingExistenceOfEdgeLengthLimitedPaths;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// CheckingExistenceOfEdgeLengthLimitedPathsSolution's, the same methods
// CheckingExistenceOfEdgeLengthLimitedPathsTests proves correct. The workload is a
// seeded random multigraph with three edges and two queries per node, so the
// baseline pays one full graph walk per query - O(q * (n + e)) - against the
// offline sweep's single pass over both sorted lists, O((e + q) log(e + q) +
// (e + q) * alpha(n)). Generating the edge list and the queries is [GlobalSetup]'s
// job, so only the answering is measured.
[MemoryDiagnoser]
public class CheckingExistenceOfEdgeLengthLimitedPathsBenchmarks
{
    private const int RandomSeed = 1697; // LC problem number
    private const int EdgeCountPerNodeMultiplier = 3;
    private const int QueryCountPerNodeMultiplier = 2;
    private const int MaxEdgeWeight = 1_000_000;

    [Params(100, 2_000)]
    public int NodeCount;

    private int[][] _edgeList = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edgeCount = NodeCount * EdgeCountPerNodeMultiplier;
        var queryCount = NodeCount * QueryCountPerNodeMultiplier;

        _edgeList = Enumerable.Range(0, edgeCount)
            .Select(_ => new[] { random.Next(NodeCount), random.Next(NodeCount), random.Next(1, MaxEdgeWeight) })
            .ToArray();

        _queries = Enumerable.Range(0, queryCount)
            .Select(_ => new[] { random.Next(NodeCount), random.Next(NodeCount), random.Next(1, MaxEdgeWeight) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool[] DfsPerQuery() =>
        CheckingExistenceOfEdgeLengthLimitedPathsSolution.DistanceLimitedPathsExistByPerQueryDfs(
            NodeCount, _edgeList, _queries);

    [Benchmark]
    public bool[] OfflineDisjointSetSweep() =>
        CheckingExistenceOfEdgeLengthLimitedPathsSolution.DistanceLimitedPathsExistByOfflineDisjointSet(
            NodeCount, _edgeList, _queries);
}
