using DSAExperimentation.LeetCode.MinimumTimeVisitingAllPoints;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeVisitingAllPointsSolution's - open-coded
// max(|dx|, |dy|) against Algorithms.ShortestPaths' ChebyshevHeuristic witness, applied purely
// for its distance computation rather than an actual search. PointCount stops at LC 1266's
// own bound of 100 points.
public class MinimumTimeVisitingAllPointsBenchmarks
{
    private const int CoordinateMagnitude = 1_000;
    private const int Seed = 1;

    private int[][] _points = [];

    [Params(10, 100)]
    public int PointCount { get; set; }

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
