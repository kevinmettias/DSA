namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// k or more in a row win: freestyle gomoku's rule, and every positional game's, since a run longer
// than k holds a run of k. The preset.
/// <summary>The overline rule under which k or more in a row win, as in freestyle gomoku.</summary>
public readonly struct KOrMore : IOverlineRule
{
    /// <inheritdoc/>
    public static bool IsOverlineAWin => true;
}
