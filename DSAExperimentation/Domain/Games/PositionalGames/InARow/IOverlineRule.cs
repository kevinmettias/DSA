namespace DSAExperimentation.Domain.Games.PositionalGames.InARow;

// Whether a run longer than k, an overline, wins an m,n,k game. Freestyle gomoku says yes: five or
// more in a row win, and so does every positional game, since a run longer than k holds a run of k
// and the winning sets (MnkGame's lines) are the same either way. Standard gomoku says no: only
// exactly five win, and six in a row win nothing. That rule forbids a pattern rather than adding a
// set, so no family of winning sets states it and PositionalGame cannot play it; a game judged on
// the board's masks can (InARowGame). A static abstract constant of a struct, so the rule a game is
// played by folds into its code and k or more pays nothing for the variant it does not play.
/// <summary>Whether a run of more than k in a row wins, or only exactly k.</summary>
public interface IOverlineRule
{
    /// <summary>Gets whether a run longer than k wins.</summary>
    static abstract bool IsOverlineAWin { get; }
}
