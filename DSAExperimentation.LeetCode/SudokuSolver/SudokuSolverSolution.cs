using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.SudokuSolver;

// LeetCode 37. Sudoku Solver: fill every '.' cell of a partially filled 9x9
// board so every row, column, and 3x3 box holds each digit 1-9 exactly once.
// LeetCode does not read a return value - it inspects `board` after the call
// - so both strategies leave the caller's array holding the solved puzzle and
// return whether a solution was found.
//
// The composed strategy is this repo's generic Backtrack.TrySearch engine
// (choose/explore/unchoose over one shared mutable state), closed over a 9x9
// board and a (Row, Col, Digit) placement choice - exactly the case
// Backtrack.cs/BacktrackingSteps.cs's own doc comments name Sudoku as the
// reason DepthFirstSearch's fresh-immutable-snapshot shape can't serve.
// TrySearch's Unchoose runs on every frame as `stopped = true` unwinds back
// up the call stack (TryEachCandidate calls it unconditionally, before
// checking `stopped`), so by the time TrySearch returns, every placement on
// the winning path has already been undone again. OnSolution therefore
// snapshots the board first - the same "capture inside OnSolution" precedent
// PermutationSequenceBenchmarks.cs's `found = ...` already uses - and the
// snapshot is copied back into the caller's own array once the search
// returns, so the mutation LeetCode actually checks still happens.
internal static class SudokuSolverSolution
{
    private const int BoardSize = 9;
    private const int BoxSize = 3;

    // The textbook answer: a hand-specialized recursion that places a digit,
    // recurses, and only undoes the placement on a failing branch, so a
    // successful path leaves the board mutated with the solution and no
    // separate copy-back step is needed. Deliberately written without this
    // repo's primitives - it is the arm the composed engine has to justify
    // itself against.
    public static bool TrySolveBySpecializedRecursion(char[][] board) => Search(board);

    // This repo's own choose/explore/unchoose engine, closed over the same
    // board/placement shape the specialized recursion above walks.
    public static bool TrySolveByBacktrackEngine(char[][] board)
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

        if (found && solution is not null)
        {
            CopyInto(board, solution);
        }

        return found;
    }

    private static IEnumerable<Placement> CandidatePlacements(char[][] board)
    {
        var cell = FindEmptyCell(board);
        if (cell is null)
        {
            return [];
        }

        var (row, col) = cell.Value;

        return Enumerable.Range(1, BoardSize)
            .Select(digit => (char)('0' + digit))
            .Where(digit => IsValidPlacement(board, row, col, digit))
            .Select(digit => new Placement(row, col, digit));
    }

    private static char[][] CloneBoard(char[][] board) => board.Select(row => (char[])row.Clone()).ToArray();

    private static void CopyInto(char[][] target, char[][] source)
    {
        for (var row = 0; row < BoardSize; row++)
        {
            Array.Copy(source[row], target[row], BoardSize);
        }
    }

    private static bool Search(char[][] board)
    {
        var cell = FindEmptyCell(board);
        if (cell is null)
        {
            return true;
        }

        var (row, col) = cell.Value;

        for (var digit = '1'; digit <= '9'; digit++)
        {
            if (TryPlaceDigit(board, row, col, digit))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryPlaceDigit(char[][] board, int row, int col, char digit)
    {
        if (!IsValidPlacement(board, row, col, digit))
        {
            return false;
        }

        board[row][col] = digit;

        if (Search(board))
        {
            return true;
        }

        board[row][col] = '.';
        return false;
    }

    private static (int Row, int Col)? FindEmptyCell(char[][] board)
    {
        for (var row = 0; row < BoardSize; row++)
        {
            for (var col = 0; col < BoardSize; col++)
            {
                if (board[row][col] == '.')
                {
                    return (row, col);
                }
            }
        }

        return null;
    }

    private static bool IsValidPlacement(char[][] board, int row, int col, char digit) =>
        !HasRowOrColumnConflict(board, row, col, digit) && !HasBoxConflict(board, row, col, digit);

    private static bool HasRowOrColumnConflict(char[][] board, int row, int col, char digit)
    {
        for (var i = 0; i < BoardSize; i++)
        {
            if (board[row][i] == digit || board[i][col] == digit)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasBoxConflict(char[][] board, int row, int col, char digit)
    {
        var boxRow = row / BoxSize * BoxSize;
        var boxCol = col / BoxSize * BoxSize;

        for (var r = boxRow; r < boxRow + BoxSize; r++)
        {
            for (var c = boxCol; c < boxCol + BoxSize; c++)
            {
                if (board[r][c] == digit)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private sealed record State(char[][] Board);

    private readonly record struct Placement(int Row, int Col, char Digit);
}
