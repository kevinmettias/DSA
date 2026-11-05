using DSAExperimentation.Domain.Games.TicTacToe;

namespace DSAExperimentation.LeetCode.Solutions.FindWinnerOnATicTacToeGame;

// LC 1275's promise about its input: the moves follow the rules of tic-tac-toe, and the game ends when
// a player completes a line, so no move follows a win. LeetCode's judge never poses an input that
// breaks it. The two strategies read on past a win and can see a second one. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class FindWinnerOnATicTacToeGamePromise
{
    private const int Size = TicTacToeRules.BoardSize;

    public static bool IsKeptBy(int[][] moves)
    {
        var marks = new TicTacToePlayer?[Size, Size];

        for (var index = 0; index < moves.Length - 1; index++)
        {
            var (row, column) = (moves[index][0], moves[index][1]);
            var player = TicTacToeRules.PlayerMaking(index);
            marks[row, column] = player;

            if (IsALineCompleted(marks, row, column, player))
            {
                return false;
            }
        }

        return true;
    }

    // Whether the square's row, its column, or a diagonal it lies on now holds the player's mark throughout.
    private static bool IsALineCompleted(TicTacToePlayer?[,] marks, int row, int column, TicTacToePlayer player)
    {
        var line = Enumerable.Range(0, Size).ToList();
        var fillsTheRow = line.All(at => marks[row, at] == player);
        var fillsTheColumn = line.All(at => marks[at, column] == player);
        var fillsTheDiagonal = TicTacToeRules.IsOnDiagonal(row, column) && line.All(at => marks[at, at] == player);
        var fillsTheAntiDiagonal = TicTacToeRules.IsOnAntiDiagonal(row, column) && line.All(at => marks[at, Size - 1 - at] == player);
        return fillsTheRow || fillsTheColumn || fillsTheDiagonal || fillsTheAntiDiagonal;
    }
}
