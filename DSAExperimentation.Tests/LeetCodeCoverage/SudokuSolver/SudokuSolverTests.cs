using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SudokuSolver;

// LeetCode 37. Sudoku Solver: this repo's generic Backtrack.TrySearch engine
// (choose/explore/unchoose over one shared mutable state), closed over a
// 9x9 board and a (Row, Col, Digit) placement choice - exactly the case
// Backtrack.cs/BacktrackingSteps.cs's own doc comments name Sudoku as the
// reason DepthFirstSearch's fresh-immutable-snapshot shape can't serve.
//
// OnSolution snapshots the board before returning true, rather than reading
// it back off `board` once TrySearch returns: Unchoose still runs on every
// frame as `stopped = true` unwinds back up the call stack (TryEachCandidate
// calls it unconditionally, before checking `stopped`), so by the time
// TrySearch itself returns, every placement on the winning path has already
// been undone again. PermutationSequenceBenchmarks.cs's `found = ...` capture
// inside its own OnSolution is this repo's existing precedent for the same
// reason.
public sealed partial class SudokuSolverTests
{
    [Fact]
    public void TrySolve_ClassicPuzzle_FillsBoardWithCompleteValidSolution()
    {
        var board = ParseBoard(Puzzle);

        var solution = TrySolve(board);

        Assert.NotNull(solution);
        AssertIsCompleteAndValid(solution);
    }

    private static char[][]? TrySolve(char[][] board)
    {
        var state = new State(board);
        char[][]? solution = null;

        var found = Backtrack.TrySearch<State, Placement>(state, new BacktrackingSteps<State, Placement>(
            IsSolution: s => FindEmptyCell(s.Board) is null,
            Candidates: s => CandidatePlacements(s.Board),
            Choose: (s, placement) => s.Board[placement.Row][placement.Col] = placement.Digit,
            Unchoose: (s, placement) => s.Board[placement.Row][placement.Col] = '.',
            OnSolution: s =>
            {
                solution = CloneBoard(s.Board);
                return true;
            }));

        return found ? solution : null;
    }

    private static char[][] CloneBoard(char[][] board) => board.Select(row => (char[])row.Clone()).ToArray();

    private static IEnumerable<Placement> CandidatePlacements(char[][] board)
    {
        var cell = FindEmptyCell(board);
        if (cell is null)
        {
            return [];
        }

        var (row, col) = cell.Value;

        return Enumerable.Range(1, 9)
            .Select(digit => (char)('0' + digit))
            .Where(digit => IsValidPlacement(board, row, col, digit))
            .Select(digit => new Placement(row, col, digit));
    }

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

    private static void AssertIsCompleteAndValid(char[][] board)
    {
        for (var i = 0; i < 9; i++)
        {
            AssertIsPermutationOfOneToNine(Enumerable.Range(0, 9).Select(col => board[i][col]));
            AssertIsPermutationOfOneToNine(Enumerable.Range(0, 9).Select(row => board[row][i]));
        }

        for (var boxRow = 0; boxRow < 9; boxRow += 3)
        {
            for (var boxCol = 0; boxCol < 9; boxCol += 3)
            {
                var box = new List<char>();
                for (var r = boxRow; r < boxRow + 3; r++)
                {
                    for (var c = boxCol; c < boxCol + 3; c++)
                    {
                        box.Add(board[r][c]);
                    }
                }

                AssertIsPermutationOfOneToNine(box);
            }
        }
    }

    private static void AssertIsPermutationOfOneToNine(IEnumerable<char> digits) =>
        Assert.Equal("123456789", new string(digits.OrderBy(d => d).ToArray()));

    private static char[][] ParseBoard(string[] rows) => rows.Select(r => r.ToCharArray()).ToArray();

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
