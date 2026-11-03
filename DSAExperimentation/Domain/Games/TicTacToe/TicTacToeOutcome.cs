namespace DSAExperimentation.Domain.Games.TicTacToe;

/// <summary>How a tic-tac-toe game stands after some number of moves.</summary>
public enum TicTacToeOutcome
{
    /// <summary>X has completed a line.</summary>
    XWins,

    /// <summary>O has completed a line.</summary>
    OWins,

    /// <summary>Every square is marked and nobody has completed a line.</summary>
    Draw,

    /// <summary>Squares remain and nobody has completed a line.</summary>
    Pending,
}
