using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfProvinces;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfProvincesSolution's, the same methods
// NumberOfProvincesTests proves correct. Both still touch every cell of the n x n
// matrix, so this is not a different asymptotic class - Union-Find's edge is
// near-constant-time merging via path compression/union-by-rank instead of DFS's
// own recursion and visited-array bookkeeping.
[MemoryDiagnoser]
public class NumberOfProvincesBenchmarks
{
    private const int RandomSeed = 1;

    [Params(50, 300)]
    public int CityCount;

    private int[][] _isConnected = null!;

    [GlobalSetup]
    public void Setup() => _isConnected = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int DepthFirstFloodFill() => NumberOfProvincesSolution.CountProvincesByDepthFirstFloodFill(_isConnected);

    [Benchmark]
    public int DisjointSetUnionFind() => NumberOfProvincesSolution.CountProvincesByDisjointSetUnionFind(_isConnected);
}
