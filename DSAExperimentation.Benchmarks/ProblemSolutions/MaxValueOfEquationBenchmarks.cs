using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaxValueOfEquation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxValueOfEquationSolution's, the same methods
// MaxValueOfEquationTests proves correct. k is set wide enough that almost every
// pair stays in-window, forcing BOTH strategies through close to their full
// O(n^2)/O(n) shapes instead of an early window-shrink making the all-pairs scan
// look artificially competitive. Point construction is charged to [GlobalSetup].
[MemoryDiagnoser]
public class MaxValueOfEquationBenchmarks
{
    private const int K = 1_000_000;

    private const int MaxXStep = 5;

    private const int YCoordinateRange = 1_000;

    [Params(500, 4_000)]
    public int Length;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var x = 0;
        _points = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                x += random.Next(1, MaxXStep);
                return new[] { x, random.Next(-YCoordinateRange, YCoordinateRange) };
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int AllPairsScan() => MaxValueOfEquationSolution.FindMaxValueOfEquationByAllPairsScan(_points, K);

    [Benchmark]
    public int MonotonicDeque() => MaxValueOfEquationSolution.FindMaxValueOfEquationByMonotonicDeque(_points, K);
}
