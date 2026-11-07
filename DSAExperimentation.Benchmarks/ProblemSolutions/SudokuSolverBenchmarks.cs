using DSAExperimentation.LeetCode.Solutions.SudokuSolver;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: every arm is SudokuSolverSolution's, the same methods
// SudokuSolverSolutionTests proves correct. The first two walk the identical search
// tree over the same fixed puzzle (there is no natural "size" axis to scale the way
// TwoSum's input length does - the board is always 9x9, the same reason
// NQueensBenchmarks fixes Size rather than [Params]-ing it), so they isolate
// the constant-factor cost of Backtrack.TrySearch's generic delegate dispatch
// from an equivalent purpose-built recursion. The third searches a different
// tree, Algorithm X's over Sudoku's exact cover, branching on the constraint
// fewest placements still meet, and builds its dancing links on every call, so
// it times that build as well as the search. The puzzle is LeetCode 37's own
// example, kept because the input space is fixed at one 9x9 board. Every
// strategy fills its board argument in place, so every iteration needs a
// pristine copy: each arm copies the parsed puzzle into a scratch board
// [GlobalSetup] allocated once. The copy is timed on purpose because the
// strategies mutate their input, and every arm pays the same cost.
public class SudokuSolverBenchmarks
{
    private char[][] _puzzle = [];
    private char[][] _board = [];

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
    public void Setup()
    {
        _puzzle = Puzzle.Select(row => row.ToCharArray()).ToArray();
        _board = Puzzle.Select(row => new char[row.Length]).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool TrySolveBySpecializedRecursion()
    {
        ResetBoard();
        return SudokuSolverSolution.TrySolveBySpecializedRecursion(_board);
    }

    [Benchmark]
    public bool TrySolveByBacktrackEngine()
    {
        ResetBoard();
        return SudokuSolverSolution.TrySolveByBacktrackEngine(_board);
    }

    [Benchmark]
    public bool TrySolveByExactCover()
    {
        ResetBoard();
        return SudokuSolverSolution.TrySolveByExactCover(_board);
    }

    private void ResetBoard()
    {
        for (var row = 0; row < _puzzle.Length; row++)
        {
            _puzzle[row].CopyTo(_board[row], 0);
        }
    }
}
