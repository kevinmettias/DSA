using DSAExperimentation.LeetCode.DiagonalTraverse;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DiagonalTraverseSolution's, the same methods
// DiagonalTraverseSolutionTests proves correct. LC 498 caps the matrix at 10^4
// cells, so the larger Size is 100.
public class DiagonalTraverseBenchmarks
{
    private int[][] _matrix = [];

    [Params(50, 100)]
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
