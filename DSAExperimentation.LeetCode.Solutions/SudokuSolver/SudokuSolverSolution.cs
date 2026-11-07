using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.ExactCover;
using DSAExperimentation.Domain.Games.Sudoku;

namespace DSAExperimentation.LeetCode.Solutions.SudokuSolver;

// LeetCode 37. Sudoku Solver: fill every '.' cell of a partially filled 9x9
// board so every row, column, and 3x3 box holds each digit 1-9 exactly once.
// LeetCode does not read a return value - it inspects `board` after the call
// - so every strategy leaves the caller's array holding the solved puzzle and
// returns whether a solution was found.
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
//
// Both arms read the board's rules from Domain/Games/Sudoku: its size, its digits and
// empty-cell mark, and when a placement is legal; the engine arm also asks when the
// board is solved. Which cell to branch on next (FindEmptyCell) is the search's own
// choice and stays here (ARCHITECTURE §17.5).
//
// The third strategy is Sudoku as exact cover (Domain/Games/Sudoku's SudokuCover)
// solved by Knuth's Algorithm X over dancing links (Algorithms/ExactCover): it
// branches on the constraint the fewest placements still meet, not on the next
// empty cell.
[LeetCodeQuestion("sudoku-solver", LeetCodeAnswerOrder.AsWritten, AnswersInPlace = true, Promise = typeof(SudokuSolverPromise))]
internal static class SudokuSolverSolution
{
    // The textbook answer: a hand-specialized recursion that places a digit,
    // recurses, and only undoes the placement on a failing branch, so a
    // successful path leaves the board mutated with the solution and no
    // separate copy-back step is needed. Deliberately written without this
    // repo's primitives - it is the arm the composed engine has to justify
    // itself against.
    public static bool TrySolveBySpecializedRecursion(char[][] board) => TrySolveBoard(board);

    // This repo's own choose/explore/unchoose engine, closed over the same
    // board/placement shape the specialized recursion above walks.
    public static bool TrySolveByBacktrackEngine(char[][] board)
    {
        var state = new State(board);
        char[][]? solution = null;

        var found = Backtrack.TrySearch<State, Placement>(state, new BacktrackingSteps<State, Placement>(
            IsSolution: s => Sudoku<ClassicSudoku>.IsComplete(s.Board),
            Candidates: s => CandidatePlacements(s.Board),
            Choose: (s, placement) => s.Board[placement.Row][placement.Col] = placement.Digit,
            Unchoose: (s, placement) => s.Board[placement.Row][placement.Col] = ClassicSudoku.EmptyCell,
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

        // A lambda rather than the method group: a delegate bound straight to a static method is
        // called through a thunk the JIT cannot inline, which measured 6-10% on LC 37.
        return Enumerable.Range(0, SudokuDigits.Size)
            .Select(slot => SudokuDigits.LetterAt(slot))
            .Where(digit => Sudoku<ClassicSudoku>.CanPlace(board, row, col, digit))
            .Select(digit => new Placement(row, col, digit));
    }

    private static char[][] CloneBoard(char[][] board) => board.Select(row => (char[])row.Clone()).ToArray();

    private static void CopyInto(char[][] target, char[][] source)
    {
        for (var row = 0; row < ClassicSudoku.Size; row++)
        {
            Array.Copy(source[row], target[row], ClassicSudoku.Size);
        }
    }

    // Knuth's Algorithm X over Sudoku's exact cover: every placement of a digit in a cell a row of
    // the matrix, every cell, row-digit, column-digit and box-digit constraint a column, and the
    // givens chosen before the search. The search takes the constraint the fewest placements still
    // meet, which on a classic puzzle is most often a cell or a unit with one place left, so it
    // seldom guesses. A board with no solution is left as it was.
    public static bool TrySolveByExactCover(char[][] board)
    {
        var givens = SudokuCover<ClassicSudoku>.GivenRows<SudokuDigits>(board);

        if (!AlgorithmX.TryFindCover(default(SudokuCover<ClassicSudoku>), givens, out var cover))
        {
            return false;
        }

        SudokuCover<ClassicSudoku>.Fill<SudokuDigits>(board, cover);

        return true;
    }

    private static bool TrySolveBoard(char[][] board)
    {
        var cell = FindEmptyCell(board);
        if (cell is null)
        {
            return true;
        }

        var (row, col) = cell.Value;

        for (var digit = SudokuDigits.FirstDigit; digit <= SudokuDigits.LastDigit; digit++)
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
        if (!Sudoku<ClassicSudoku>.CanPlace(board, row, col, digit))
        {
            return false;
        }

        board[row][col] = digit;

        if (TrySolveBoard(board))
        {
            return true;
        }

        board[row][col] = ClassicSudoku.EmptyCell;
        return false;
    }

    private static (int Row, int Col)? FindEmptyCell(char[][] board)
    {
        for (var row = 0; row < ClassicSudoku.Size; row++)
        {
            for (var col = 0; col < ClassicSudoku.Size; col++)
            {
                if (board[row][col] == ClassicSudoku.EmptyCell)
                {
                    return (row, col);
                }
            }
        }

        return null;
    }

    private sealed record State(char[][] Board);

    private readonly record struct Placement(int Row, int Col, char Digit);
}
