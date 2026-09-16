using DSAExperimentation.LeetCode.SudokuSolver;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SudokuSolver;

// Harness only. Both strategies are SudokuSolverSolution's; LeetCode does not
// read a return value - it inspects `board` after the call - so each theory
// mutates its own parsed copy and checks the mutated board is a complete,
// valid solution.
public sealed partial class SudokuSolverTests
{
    public static TheoryData<string[]> Examples =>
        new()
        {
            new[]
            {
                "53..7....",
                "6..195...",
                ".98....6.",
                "8...6...3",
                "4..8.3..1",
                "7...2...6",
                ".6....28.",
                "...419..5",
                "....8..79",
            },
            // Already solved: exercises the immediate IsSolution/FindEmptyCell
            // "nothing left to place" branch both strategies share.
            new[]
            {
                "534678912",
                "672195348",
                "198342567",
                "859761423",
                "426853791",
                "713924856",
                "961537284",
                "287419635",
                "345286179",
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrySolveBySpecializedRecursion_LeetCodeExamples_FillsBoardWithCompleteValidSolution(string[] rows)
    {
        var board = ParseBoard(rows);

        var found = SudokuSolverSolution.TrySolveBySpecializedRecursion(board);

        Assert.True(found);
        AssertIsCompleteAndValid(board);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TrySolveByBacktrackEngine_LeetCodeExamples_FillsBoardWithCompleteValidSolution(string[] rows)
    {
        var board = ParseBoard(rows);

        var found = SudokuSolverSolution.TrySolveByBacktrackEngine(board);

        Assert.True(found);
        AssertIsCompleteAndValid(board);
    }

    private static char[][] ParseBoard(string[] rows) => rows.Select(r => r.ToCharArray()).ToArray();

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
                var box = CollectBox(board, boxRow, boxCol);

                AssertIsPermutationOfOneToNine(box);
            }
        }
    }

    private static List<char> CollectBox(char[][] board, int boxRow, int boxCol)
    {
        var box = new List<char>();

        for (var r = boxRow; r < boxRow + 3; r++)
        {
            for (var c = boxCol; c < boxCol + 3; c++)
            {
                box.Add(board[r][c]);
            }
        }

        return box;
    }

    private static void AssertIsPermutationOfOneToNine(IEnumerable<char> digits) =>
        Assert.Equal("123456789", new string(digits.OrderBy(d => d).ToArray()));
}
