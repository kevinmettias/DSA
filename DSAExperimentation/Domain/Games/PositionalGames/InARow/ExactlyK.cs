namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// Only exactly k in a row win: standard gomoku's rule, under which six in a row win nothing for
// either player. A run of k wins when the cells just beyond both of its ends are not the player's.
/// <summary>
/// The overline rule under which only exactly k in a row win, as in standard gomoku.
/// </summary>
public readonly struct ExactlyK : IOverlineRule
{
    /// <inheritdoc/>
    public static bool IsOverlineAWin => false;
}
