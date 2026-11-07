using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Domain.Games.Sudoku;

namespace DSAExperimentation.LeetCode.Solutions.SudokuSolver;

// LC 37's promise about its input: "It is guaranteed that the input board has only one solution." LeetCode's
// judge never poses another board. Most of the dataset's generated boards break it - of its 19, 11 have no
// solution and four have more than one - and a board without exactly one solution has no answer to judge and is
// no input of LeetCode's to time. A harness that runs them asks this first. It counts the board's solutions,
// stopping at a second, with the library's backtracking engine over Sudoku's rule (Domain/Games/Sudoku), and
// branches on the empty cell with the fewest digits left, so a board with no solution fails fast.
internal static class SudokuSolverPromise
{
    private const int SolutionsTellingTwoApart = 2;

    public static bool IsKeptBy(char[][] board)
    {
        var cells = board.Select(row => (char[])row.Clone()).ToArray();

        return IsEachGivenAllowed(cells) && SolutionsUpToTwo(cells) == 1;
    }

    // Whether each given digit may stand where it is, the others left in place: none repeats in a unit.
    private static bool IsEachGivenAllowed(char[][] cells) =>
        CellsOf(cells).All(cell => IsGivenAllowed(cells, cell));

    private static bool IsGivenAllowed(char[][] cells, (int Row, int Col) cell)
    {
        var digit = cells[cell.Row][cell.Col];

        if (digit == ClassicSudoku.EmptyCell)
        {
            return true;
        }

        cells[cell.Row][cell.Col] = ClassicSudoku.EmptyCell;
        var isAllowed = Sudoku<ClassicSudoku>.CanPlace(cells, cell.Row, cell.Col, digit);
        cells[cell.Row][cell.Col] = digit;

        return isAllowed;
    }

    private static int SolutionsUpToTwo(char[][] cells)
    {
        var solutions = 0;

        Backtrack.TrySearch(cells, new BacktrackingSteps<char[][], Placement>(
            IsSolution: board => Sudoku<ClassicSudoku>.IsComplete(board),
            Candidates: PlacementsAtTheTightestCell,
            Choose: (board, placement) => board[placement.Row][placement.Col] = placement.Digit,
            Unchoose: (board, placement) => board[placement.Row][placement.Col] = ClassicSudoku.EmptyCell,
            OnSolution: _ => ++solutions == SolutionsTellingTwoApart));

        return solutions;
    }

    // Every digit the empty cell with the fewest left may take; none when some empty cell can take none.
    private static List<Placement> PlacementsAtTheTightestCell(char[][] board)
    {
        var options = CellsOf(board)
            .Where(cell => board[cell.Row][cell.Col] == ClassicSudoku.EmptyCell)
            .Select(cell => DigitsFor(board, cell))
            .MinBy(placements => placements.Count);

        return options ?? [];
    }

    private static List<Placement> DigitsFor(char[][] board, (int Row, int Col) cell) =>
        [.. Enumerable.Range(0, SudokuDigits.Size)
            .Select(SudokuDigits.LetterAt)
            .Where(digit => Sudoku<ClassicSudoku>.CanPlace(board, cell.Row, cell.Col, digit))
            .Select(digit => new Placement(cell.Row, cell.Col, digit))];

    private static IEnumerable<(int Row, int Col)> CellsOf(char[][] cells) =>
        Enumerable.Range(0, cells.Length).SelectMany(row => Enumerable.Range(0, cells[row].Length).Select(col => (row, col)));

    private readonly record struct Placement(int Row, int Col, char Digit);
}
