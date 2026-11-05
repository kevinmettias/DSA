namespace DSAExperimentation.LeetCode.Solutions.CreateGridWithExactlyKPathsI;

// LC 3988 accepts any m x n grid of '.' and '#' with exactly k paths from the top-left cell to the
// bottom-right one, moving right or down through free cells, and the empty array only when no grid has
// that many. The catalog records one grid, so an answer stands when it is empty exactly where the
// recorded one is, or else such a grid. The two strategies block different cells.
internal static class CreateGridWithExactlyKPathsIAnswerCheck
{
    private const char Free = '.';
    private const char Obstacle = '#';

    public static bool IsAccepted(int gridRowCount, int gridColumnCount, int targetPathCount, string[] answer, string[] recorded)
    {
        if (answer.Length == 0 || recorded.Length == 0)
        {
            return answer.Length == recorded.Length;
        }

        var isShaped = answer.Length == gridRowCount && answer.All(row => row.Length == gridColumnCount && row.All(cell => cell is Free or Obstacle));
        return isShaped && PathCountOf(answer) == targetPathCount;
    }

    // The paths reaching each cell are those reaching the cell above plus those reaching the cell to its
    // left, and none reach an obstacle.
    private static long PathCountOf(string[] grid)
    {
        var columns = grid[0].Length;
        var reaching = new long[columns];
        reaching[0] = 1;

        foreach (var row in grid)
        {
            for (var column = 0; column < columns; column++)
            {
                if (row[column] == Obstacle)
                {
                    reaching[column] = 0;
                }
                else if (column > 0)
                {
                    reaching[column] += reaching[column - 1];
                }
            }
        }

        return reaching[columns - 1];
    }
}
