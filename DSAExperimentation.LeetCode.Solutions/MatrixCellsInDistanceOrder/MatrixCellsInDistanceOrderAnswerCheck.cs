namespace DSAExperimentation.LeetCode.Solutions.MatrixCellsInDistanceOrder;

// LC 1030 accepts the coordinates of every cell of the rows x cols matrix, each once, in any order that sorts
// them by Manhattan distance from (rCenter, cCenter), nearest first ("You may return the answer in any order
// that satisfies this condition"). So an answer stands when it lists each cell once as an [r, c] pair and the
// distance never falls from one cell to the next. The recorded answer is one of these, so it does not take
// part. Equidistant cells come in whatever order a strategy leaves them: the closed form's Array.Sort is not
// stable, and the grid BFS reaches them in its own neighbor order, neither of them the catalog's.
internal static class MatrixCellsInDistanceOrderAnswerCheck
{
    public static bool IsAccepted(int rows, int cols, int rCenter, int cCenter, int[][] answer, int[][] recorded)
    {
        if (!answer.All(cell => cell is [_, _]))
        {
            return false;
        }

        var cells = answer.Select(cell => (Row: cell[0], Col: cell[1])).ToList();
        var everyCell = Enumerable.Range(0, rows).SelectMany(row => Enumerable.Range(0, cols).Select(col => (Row: row, Col: col)));

        return cells.Order().SequenceEqual(everyCell) && IsDistanceNeverFalling(cells, rCenter, cCenter);
    }

    // Whether each cell stands at least as far from the center as the cell before it.
    private static bool IsDistanceNeverFalling(List<(int Row, int Col)> cells, int rCenter, int cCenter)
    {
        var distances = cells.Select(cell => Math.Abs(cell.Row - rCenter) + Math.Abs(cell.Col - cCenter)).ToList();
        var following = distances.Skip(1);

        return distances.Zip(following).All(pair => pair.First <= pair.Second);
    }
}
