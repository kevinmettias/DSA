using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DiagonalTraverse;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DiagonalTraverseSolution's, the same methods
// DiagonalTraverseTests proves correct.
[MemoryDiagnoser]
public class DiagonalTraverseBenchmarks
{
    private int[][] _matrix = [];

    [Params(50, 300)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var value = 0;
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => value++).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] DirectionToggleWalk() => DiagonalTraverseSolution.FindDiagonalOrderByDirectionToggle(_matrix);

    [Benchmark]
    public int[] DiagonalGroupsWithStackReversal() =>
        DiagonalTraverseSolution.FindDiagonalOrderByStackReversal(_matrix);
}
