namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// The three numbers that make an m,n,k game: a board m cells wide and n tall, and k in a row to
// win. The board is written width first, as Connect Four's 7 by 6 is: 7 columns, 6 rows. They are
// static abstract members of a struct, so MnkGame's arithmetic folds to each preset's constants.
/// <summary>
/// The shape of an m,n,k game: the board's columns and rows, and how many in a row win.
/// </summary>
public interface IMnkShape
{
    /// <summary>Gets m, the board's width: how many columns it has.</summary>
    static abstract int Columns { get; }

    /// <summary>Gets n, the board's height: how many rows it has.</summary>
    static abstract int Rows { get; }

    /// <summary>Gets k, how many cells in a row win: along a row, a column or a diagonal.</summary>
    static abstract int InARow { get; }
}
