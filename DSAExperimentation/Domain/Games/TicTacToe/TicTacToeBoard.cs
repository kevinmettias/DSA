using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Domain.Games.TicTacToe;

// Tic-tac-toe as an m,n,k game: the 3,3,3 game, a 3 by 3 board on which three in a row wins, so
// every line runs the whole board. TicTacToeRules reads its side as the constant LC 1275 does. The
// side is internal, so that a solution's own small constants that share a word with it are not
// read as copies (ARCHITECTURE 17.9); the shape's members state it to everyone.
/// <summary>Tic-tac-toe's shape: a 3 by 3 board, three in a row to win.</summary>
public readonly struct TicTacToeBoard : IMnkShape
{
    internal const int Size = 3;

    /// <inheritdoc/>
    public static int Columns => Size;

    /// <inheritdoc/>
    public static int Rows => Size;

    /// <inheritdoc/>
    public static int InARow => Size;
}
