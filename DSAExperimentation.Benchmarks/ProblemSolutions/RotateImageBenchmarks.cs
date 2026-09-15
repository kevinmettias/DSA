using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RotateImage;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RotateImageSolution's, the same methods
// RotateImageTests proves correct. Each iteration clones the pristine matrix
// before rotating, since the solution mutates in place and [GlobalSetup] runs
// once per benchmark, not once per invocation.
[MemoryDiagnoser]
public class RotateImageBenchmarks
{
    private const int MaxCellValueExclusive = 1_000;

    private int[][] _matrix = [];

    [Params(50, 300)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _matrix[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _matrix[r][c] = random.Next(1, MaxCellValueExclusive);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] ArrayReverse()
    {
        var matrix = Clone(_matrix);
        RotateImageSolution.RotateByArrayReverse(matrix);
        return matrix;
    }

    [Benchmark]
    public int[][] StackReverse()
    {
        var matrix = Clone(_matrix);
        RotateImageSolution.RotateByStackReverse(matrix);
        return matrix;
    }

    private static int[][] Clone(int[][] matrix)
    {
        var copy = new int[matrix.Length][];

        for (var r = 0; r < matrix.Length; r++)
        {
            copy[r] = (int[])matrix[r].Clone();
        }

        return copy;
    }
}
