using System.Numerics;

namespace DSAExperimentation.Algorithms.GameSearch;

// The window a fail-soft depth-limited search runs in: alpha, what the mover is already sure of,
// and beta, what the opponent will allow. A move's reply is searched in the window turned to the
// opponent's side and shifted by the move's gain, (gain - beta, gain - alpha), and an unbounded
// side stays unbounded, so the shift cannot overflow. A value found in the window is exact inside
// it and a bound on the side it fails.
internal readonly record struct SearchWindow<TScore>(TScore Alpha, TScore Beta)
    where TScore : INumber<TScore>, IMinMaxValue<TScore>
{
    // The ends of the widest window.
    private static TScore Lowest => -TScore.MaxValue;

    private static TScore Highest => TScore.MaxValue;

    // The widest window, unbounded on both sides: every score the game can reach lies inside it.
    public static SearchWindow<TScore> Widest => new(Lowest, Highest);

    // The window a move's reply is searched in, seen from the opponent's side.
    public SearchWindow<TScore> ForReply(TScore gain) =>
        new(ReplyBound(gain, Beta, Highest), ReplyBound(gain, Alpha, Lowest));

    // One side of the reply's window: the gain less the mover's bound on the other side, and
    // unbounded when that bound is.
    private static TScore ReplyBound(TScore gain, TScore bound, TScore unbounded)
    {
        if (bound == unbounded)
        {
            return -unbounded;
        }

        return gain - bound;
    }

    // The window once the mover is sure of a value: alpha raised to it when it is higher.
    public SearchWindow<TScore> RaisedTo(TScore value) =>
        this with { Alpha = TScore.Max(Alpha, value) };

    // What a value found in this window says of the true value.
    public ScoreBound BoundOf(TScore value)
    {
        if (value <= Alpha)
        {
            return ScoreBound.AtMost;
        }

        return value >= Beta ? ScoreBound.AtLeast : ScoreBound.Exact;
    }

}
