using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaxPointsOnALine;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxPointsOnALineSolution's, the same methods
// MaxPointsOnALineTests proves correct. _points is drawn uniformly at random
// over a wide coordinate range, so the answer stays small (2-3) and neither
// strategy gets to short-circuit on an early large find.
[MemoryDiagnoser]
public class MaxPointsOnALineBenchmarks
{
    private const int RandomSeed = 149; // LC problem number
    private const int CoordinateRange = 1_000;

    private int[][] _points = [];

    [Params(600, 1500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = [.. Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(-CoordinateRange, CoordinateRange), random.Next(-CoordinateRange, CoordinateRange) })];
    }

    [Benchmark(Baseline = true)]
    public int CrossProduct() =>
        MaxPointsOnALineSolution.MaxPointsByCrossProduct(_points);

    [Benchmark]
    public int SlopeGrouping() =>
        MaxPointsOnALineSolution.MaxPointsBySlopeGrouping(_points);
}
