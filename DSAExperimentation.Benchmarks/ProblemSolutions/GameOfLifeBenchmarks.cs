using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GameOfLife;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GameOfLifeSolution's, the same methods
// GameOfLifeTests proves correct. Each iteration clones the pristine board
// before advancing, since the solution mutates in place and [GlobalSetup]
// runs once per benchmark, not once per invocation.
[MemoryDiagnoser]
public class GameOfLifeBenchmarks
{
    // LC 289.
    private const int RandomSeed = 289;
    private const int LiveCellExclusiveBound = 2;

    private int[][] _board = [];

    [Params(50, 300)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _board = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, LiveCellExclusiveBound)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[][] FullBoardCopy()
    {
        var board = Clone(_board);
        GameOfLifeSolution.AdvanceByFullBoardCopy(board);
        return board;
    }

    [Benchmark]
    public int[][] SetSnapshot()
    {
        var board = Clone(_board);
        GameOfLifeSolution.AdvanceBySetSnapshot(board);
        return board;
    }

    private static int[][] Clone(int[][] matrix) =>
        matrix.Select(row => (int[])row.Clone()).ToArray();
}
