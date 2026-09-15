using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidSudoku;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidSudokuSolution's, the same methods
// ValidSudokuTests proves correct.
[MemoryDiagnoser]
public class ValidSudokuBenchmarks
{
    private char[][] _board = [];

    [GlobalSetup]
    public void Setup() => _board =
    [
        ['5', '3', '.', '.', '7', '.', '.', '.', '.'],
        ['6', '.', '.', '1', '9', '5', '.', '.', '.'],
        ['.', '9', '8', '.', '.', '.', '.', '6', '.'],
        ['8', '.', '.', '.', '6', '.', '.', '.', '3'],
        ['4', '.', '.', '8', '.', '3', '.', '.', '1'],
        ['7', '.', '.', '.', '2', '.', '.', '.', '6'],
        ['.', '6', '.', '.', '.', '.', '2', '8', '.'],
        ['.', '.', '.', '4', '1', '9', '.', '.', '5'],
        ['.', '.', '.', '.', '8', '.', '.', '7', '9'],
    ];

    [Benchmark(Baseline = true)]
    public bool BooleanGrid() => ValidSudokuSolution.IsValidByBooleanGrid(_board);

    [Benchmark]
    public bool SetKeys() => ValidSudokuSolution.IsValidBySetKeys(_board);
}
