using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SudokuSolver;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SudokuSolverSolution's, the same methods
// SudokuSolverTests proves correct. Both walk the identical search tree over
// the same fixed puzzle (there is no natural "size" axis to scale the way
// TwoSum's input length does - the board is always 9x9, the same reason
// NQueensBenchmarks fixes Size rather than [Params]-ing it), so this isolates
// the constant-factor cost of Backtrack.TrySearch's generic delegate dispatch
// from an equivalent purpose-built recursion. Each arm clones the parsed
// puzzle itself - both strategies mutate their board argument in place, so
// every iteration needs its own pristine copy, which [GlobalSetup] cannot
// hand out once.
[MemoryDiagnoser]
public class SudokuSolverBenchmarks
{
    private char[][] _puzzle = [];

    private static readonly string[] Puzzle =
    [
        "53..7....",
        "6..195...",
        ".98....6.",
        "8...6...3",
        "4..8.3..1",
        "7...2...6",
        ".6....28.",
        "...419..5",
        "....8..79",
    ];

    [GlobalSetup]
    public void Setup() => _puzzle = Puzzle.Select(row => row.ToCharArray()).ToArray();

    [Benchmark(Baseline = true)]
    public bool SpecializedRecursive() => SudokuSolverSolution.TrySolveBySpecializedRecursion(Clone(_puzzle));

    [Benchmark]
    public bool BacktrackEngine() => SudokuSolverSolution.TrySolveByBacktrackEngine(Clone(_puzzle));

    private static char[][] Clone(char[][] board) => board.Select(row => (char[])row.Clone()).ToArray();
}
