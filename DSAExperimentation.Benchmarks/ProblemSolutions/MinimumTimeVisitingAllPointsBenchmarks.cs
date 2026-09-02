using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumTimeVisitingAllPoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeVisitingAllPointsSolution's - open-coded
// max(|dx|, |dy|) against Algorithms.ShortestPaths' ChebyshevHeuristic witness, applied purely
// for its distance computation rather than an actual search.
[MemoryDiagnoser]
public class MinimumTimeVisitingAllPointsBenchmarks
{
    private const int CoordinateMagnitude = 1_000;
    private const int Seed = 1;

    [Params(200, 5_000)]
    public int PointCount;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _points = [.. Enumerable.Range(0, PointCount).Select(_ => new[]
        {
            random.Next(-CoordinateMagnitude, CoordinateMagnitude),
            random.Next(-CoordinateMagnitude, CoordinateMagnitude),
        })];
    }

    [Benchmark(Baseline = true)]
    public int InlineChebyshevSum() =>
        MinimumTimeVisitingAllPointsSolution.MinTimeByInlineChebyshev(_points);

    [Benchmark]
    public int PathHeuristicChebyshevSum() =>
        MinimumTimeVisitingAllPointsSolution.MinTimeByPathHeuristic(_points);
}
