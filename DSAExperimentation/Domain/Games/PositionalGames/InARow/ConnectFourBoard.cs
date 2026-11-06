namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// Connect Four: a board 7 columns wide and 6 rows tall, four in a row to win, played under gravity
// (Gravity<ConnectFourBoard>): a piece dropped into a column falls to its lowest open cell. The
// shape and the placement are separate witnesses, so a game of the same shape played by free
// placement is the 7,6,4 game, and gravity costs that game nothing.
/// <summary>Connect Four's shape: 7 columns, 6 rows, four in a row to win.</summary>
public readonly struct ConnectFourBoard : IMnkShape
{
    /// <inheritdoc/>
    public static int Columns => 7;

    /// <inheritdoc/>
    public static int Rows => 6;

    /// <inheritdoc/>
    public static int InARow => 4;
}
