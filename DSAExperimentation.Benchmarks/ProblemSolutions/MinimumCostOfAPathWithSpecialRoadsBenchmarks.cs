using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumCostOfAPathWithSpecialRoads;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostOfAPathWithSpecialRoadsSolution's, the same
// methods MinimumCostOfAPathWithSpecialRoadsTests proves correct. [GlobalSetup] draws
// the random start, target and special-road list - already LeetCode's own argument
// shape, so no hoisted overload is needed - and each arm then builds its own view of
// the point graph, which is part of what that strategy costs. The graph is dense
// (every pair of the ~2*SpecialRoadCount+2 points is directly connected), so the
// array-scan baseline is expected to win: the well-known dense-graph case where a
// heap's O(log V) bookkeeping costs more than it saves.
[MemoryDiagnoser]
public class MinimumCostOfAPathWithSpecialRoadsBenchmarks
{
    private const int Seed = 2662; // LC problem number
    private const int CoordinateUpperBound = 1_000;
    private const int CostUpperBound = 500;

    [Params(20, 100)]
    public int SpecialRoadCount;

    private int[] _start = null!;
    private int[] _target = null!;
    private int[][] _specialRoads = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _start = [random.Next(CoordinateUpperBound), random.Next(CoordinateUpperBound)];
        _target = [random.Next(CoordinateUpperBound), random.Next(CoordinateUpperBound)];
        _specialRoads = [.. Enumerable.Range(0, SpecialRoadCount)
            .Select(_ => new[]
            {
                random.Next(CoordinateUpperBound), random.Next(CoordinateUpperBound),
                random.Next(CoordinateUpperBound), random.Next(CoordinateUpperBound),
                random.Next(1, CostUpperBound),
            })];
    }

    [Benchmark(Baseline = true)]
    public int ArrayDijkstra() =>
        MinimumCostOfAPathWithSpecialRoadsSolution.MinimumCostByArrayScanDijkstra(_start, _target, _specialRoads);

    [Benchmark]
    public int RepoHeapDijkstra() =>
        MinimumCostOfAPathWithSpecialRoadsSolution.MinimumCostByHeapDijkstra(_start, _target, _specialRoads);
}
