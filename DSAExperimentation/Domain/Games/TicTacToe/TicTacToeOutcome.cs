namespace DSAExperimentation.Domain.Games.TicTacToe;

/// <summary>How a tic-tac-toe game stands after some number of moves.</summary>
public enum TicTacToeOutcome
{
    /// <summary>Player A has completed a line.</summary>
    AWins,

    /// <summary>Player B has completed a line.</summary>
    BWins,

    /// <summary>Every square is marked and nobody has completed a line.</summary>
    Draw,

    /// <summary>Squares remain and nobody has completed a line.</summary>
    Pending,
}
