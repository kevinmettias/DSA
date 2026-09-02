using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumCostPathWithAlternatingDirectionsIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostPathWithAlternatingDirectionsIIISolution's,
// the same methods MinimumCostPathWithAlternatingDirectionsIIITests proves agree.
// The workload is a square grid seeded once in [GlobalSetup], so what's measured is
// the state-space search itself (up to 2 * Side * Side nodes) - a hand-rolled BCL
// PriorityQueue+Dictionary against ShortestPath.Dijkstra composed with
// AlternatingGridTopology's on-the-fly edges.
[MemoryDiagnoser]
public class MinimumCostPathWithAlternatingDirectionsIIIBenchmarks
{
    private const int Seed = 4003;
    private const int MaxPenalty = 100_000;

    [Params(30, 300)]
    public int Side;

    private int[][] _penalty = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _penalty = [.. Enumerable.Range(0, Side).Select(_ =>
            Enumerable.Range(0, Side).Select(_ => random.Next(0, MaxPenalty + 1)).ToArray())];
    }

    [Benchmark(Baseline = true)]
    public long BclDijkstra() =>
        MinimumCostPathWithAlternatingDirectionsIIISolution.MinCostByBclDijkstra(Side, Side, _penalty);

    [Benchmark]
    public long StateDijkstra() =>
        MinimumCostPathWithAlternatingDirectionsIIISolution.MinCostByStateDijkstra(Side, Side, _penalty);
}
