using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Time Visiting All Points (LC 1266): inline max(|dx|,|dy|) arithmetic per
// consecutive pair vs. this repo's own IPathHeuristic<TNode,TWeight> contract,
// closed over ChebyshevHeuristic (the same king-move-distance witness
// ShortestPath.AStar composes for an 8-directional grid), applied here purely for
// its distance computation rather than an actual search.
[MemoryDiagnoser]
public class MinimumTimeVisitingAllPointsBenchmarks
{
    [Params(200, 5_000)]
    public int PointCount;

    private (int X, int Y)[] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _points = Enumerable.Range(0, PointCount)
            .Select(_ => (random.Next(-1_000, 1_000), random.Next(-1_000, 1_000)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InlineChebyshevSum()
    {
        var total = 0;

        for (var i = 1; i < _points.Length; i++)
        {
            var dx = Math.Abs(_points[i].X - _points[i - 1].X);
            var dy = Math.Abs(_points[i].Y - _points[i - 1].Y);
            total += Math.Max(dx, dy);
        }

        return total;
    }

    [Benchmark]
    public int PathHeuristicChebyshevSum()
    {
        var total = 0;

        for (var i = 1; i < _points.Length; i++)
        {
            var from = new WeightedGridNode(_points[i - 1].X, _points[i - 1].Y);
            var to = new WeightedGridNode(_points[i].X, _points[i].Y);

            total += ChebyshevHeuristic.Estimate(from, to);
        }

        return total;
    }
}
