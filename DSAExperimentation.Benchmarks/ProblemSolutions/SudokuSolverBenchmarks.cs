using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sudoku Solver (LC 37): a hand-specialized recursive solver vs. this repo's
// generic Backtrack.TrySearch engine closed over the same board/placement
// shape the test uses. Both walk the identical search tree over the same
// fixed puzzle (there is no natural "size" axis to scale the way TwoSum's
// input length does - the board is always 9x9, the same reason
// NQueensBenchmarks fixes Size rather than [Params]-ing it), so this isolates
// the constant-factor cost of Backtrack.TrySearch's generic delegate dispatch
// from an equivalent purpose-built recursion.
[MemoryDiagnoser]
public class SudokuSolverBenchmarks
{
    private char[][] _puzzle = null!;

    [GlobalSetup]
    public void Setup() => _puzzle = Puzzle.Select(row => row.ToCharArray()).ToArray();

    [Benchmark(Baseline = true)]
    public bool SpecializedRecursive() => SolveSpecialized(Clone(_puzzle));

    [Benchmark]
    public bool BacktrackEngine() => SolveWithBacktrackEngine(Clone(_puzzle));

    private static bool SolveSpecialized(char[][] board)
    {
        bool Search()
        {
            var cell = FindEmptyCell(board);
            if (cell is null)
            {
                return true;
            }

            var (row, col) = cell.Value;

            for (var digit = '1'; digit <= '9'; digit++)
            {
                if (!IsValidPlacement(board, row, col, digit))
                {
                    continue;
                }

                board[row][col] = digit;

                if (Search())
                {
                    return true;
                }

                board[row][col] = '.';
            }

            return false;
        }

        return Search();
    }

    private static bool SolveWithBacktrackEngine(char[][] board)
    {
        var state = new State(board);

        return Backtrack.TrySearch<State, Placement>(state, new BacktrackingSteps<State, Placement>(
            IsSolution: s => FindEmptyCell(s.Board) is null,
            Candidates: s =>
            {
                var cell = FindEmptyCell(s.Board);
                if (cell is null)
                {
                    return [];
                }

                var (row, col) = cell.Value;

                return Enumerable.Range(1, 9)
                    .Select(digit => (char)('0' + digit))
                    .Where(digit => IsValidPlacement(s.Board, row, col, digit))
                    .Select(digit => new Placement(row, col, digit));
            },
            Choose: (s, placement) => s.Board[placement.Row][placement.Col] = placement.Digit,
            Unchoose: (s, placement) => s.Board[placement.Row][placement.Col] = '.',
            OnSolution: _ => true));
    }

    private static char[][] Clone(char[][] board) => board.Select(row => (char[])row.Clone()).ToArray();

    private static (int Row, int Col)? FindEmptyCell(char[][] board)
    {
        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                if (board[row][col] == '.')
                {
                    return (row, col);
                }
            }
        }

        return null;
    }

    private static bool IsValidPlacement(char[][] board, int row, int col, char digit)
    {
        for (var i = 0; i < 9; i++)
        {
            if (board[row][i] == digit || board[i][col] == digit)
            {
                return false;
            }
        }

        var boxRow = row / 3 * 3;
        var boxCol = col / 3 * 3;

        for (var r = boxRow; r < boxRow + 3; r++)
        {
            for (var c = boxCol; c < boxCol + 3; c++)
            {
                if (board[r][c] == digit)
                {
                    return false;
                }
            }
        }

        return true;
    }

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

    private sealed class State(char[][] board)
    {
        public char[][] Board { get; } = board;
    }

    private readonly record struct Placement(int Row, int Col, char Digit);
}
