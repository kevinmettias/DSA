using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinCostToConnectAllPoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinCostToConnectAllPointsSolution's, the same
// methods MinCostToConnectAllPointsTests proves correct. [GlobalSetup] draws the
// random point cloud - the workload sizing and seed are the measurement decision
// that stays here - and each arm is handed LeetCode's own int[][] shape, because
// materializing the complete Manhattan-distance graph is exactly the cost the
// Kruskal arm is on trial for and must not be charged to setup.
[MemoryDiagnoser]
public class MinCostToConnectAllPointsBenchmarks
{
    private const int RandomSeed = 1584; // LeetCode problem number

    private const int CoordinateBound = 1_000;

    private int[][] _points = [];

    [Params(50, 200)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, PointCount)
            .Select(_ => new[] { random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DensePrim() => MinCostToConnectAllPointsSolution.MinCostConnectPointsByDensePrim(_points);

    [Benchmark]
    public int KruskalMst() => MinCostToConnectAllPointsSolution.MinCostConnectPointsByKruskalMst(_points);
}
