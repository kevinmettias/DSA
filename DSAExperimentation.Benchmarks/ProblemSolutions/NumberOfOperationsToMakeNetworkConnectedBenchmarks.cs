using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfOperationsToMakeNetworkConnected;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfOperationsToMakeNetworkConnectedSolution's,
// the same methods NumberOfOperationsToMakeNetworkConnectedTests proves correct -
// a DFS flood fill over a freshly built adjacency list (baseline, the textbook
// approach and NumberOfProvincesBenchmarks' own precedent for this DFS-vs-Union-Find
// contrast) against this repo's own DisjointSet. A spanning tree is generated first
// so every run has >= n-1 cables (the interesting, non-trivial case), then extra
// random edges give Union-Find real merge-avoiding work to do.
[MemoryDiagnoser]
public class NumberOfOperationsToMakeNetworkConnectedBenchmarks
{
    private const int RandomSeed = 1319; // LC problem number

    [Params(50, 300)]
    public int ComputerCount;

    private int[][] _connections = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var connections = new List<int[]>();

        for (var i = 1; i < ComputerCount; i++)
        {
            connections.Add([random.Next(i), i]);
        }

        for (var e = 0; e < ComputerCount; e++)
        {
            var a = random.Next(ComputerCount);
            var b = random.Next(ComputerCount);

            if (a != b)
            {
                connections.Add([a, b]);
            }
        }

        _connections = [.. connections];
    }

    [Benchmark(Baseline = true)]
    public int DepthFirstFloodFill() =>
        NumberOfOperationsToMakeNetworkConnectedSolution.MakeConnectedByDepthFirstFloodFill(
            ComputerCount, _connections);

    [Benchmark]
    public int DisjointSetUnionFind() =>
        NumberOfOperationsToMakeNetworkConnectedSolution.MakeConnectedByDisjointSet(
            ComputerCount, _connections);
}
