using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.LeetCode.PathExistenceQueriesInAGraphI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathExistenceQueriesInAGraphISolution's, the same
// methods PathExistenceQueriesInAGraphITests proves correct. The composed arm is
// handed a prebuilt DisjointSet - one union pass over the reduced adjacency - so
// that one-time cost is charged to [GlobalSetup], not to the queries being
// measured.
[MemoryDiagnoser]
public class PathExistenceQueriesInAGraphIBenchmarks
{
    private const int Seed = 3532;
    private const int QueryCount = 500;

    [Params(500, 5_000)]
    public int NodeCount;

    private int[] _nums = null!;
    private int _maxDiff;
    private int[][] _queries = null!;
    private DisjointSet _groups = null!;

    [GlobalSetup]
    public void Setup()
    {
        (_nums, _maxDiff) = ProximityQueryWorkloads.BuildNums(NodeCount, Seed);
        _queries = ProximityQueryWorkloads.BuildQueries(NodeCount, QueryCount, Seed);
        _groups = ProximityGroups.Build(NodeCount, _nums, _maxDiff);
    }

    [Benchmark(Baseline = true)]
    public bool[] BruteForceBfs() =>
        PathExistenceQueriesInAGraphISolution.PathExistenceQueriesByBruteForceBfs(NodeCount, _nums, _maxDiff, _queries);

    [Benchmark]
    public bool[] DisjointSet() =>
        PathExistenceQueriesInAGraphISolution.PathExistenceQueriesByDisjointSet(_groups, _queries);
}
