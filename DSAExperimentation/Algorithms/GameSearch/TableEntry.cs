using System.Numerics;

namespace DSAExperimentation.Algorithms.GameSearch;

// What a transposition table remembers of a position's search: the depth it was searched to, the
// value found and what that value bounds, and the best move found, which a later search of the
// position tries first at any depth.
internal readonly record struct TableEntry<TScore>(
    int Depth, TScore Value, ScoreBound Bound, int Move)
    where TScore : INumber<TScore>, IMinMaxValue<TScore>
{
    // Whether the remembered value answers a search of the position to this depth in this window:
    // only a search to the depth it was found at, and then an exact value always, a lower bound
    // when it reaches beta, an upper bound when it stays at or below alpha.
    public bool CanAnswer(int depth, SearchWindow<TScore> window)
    {
        if (depth != Depth)
        {
            return false;
        }

        return Bound switch
        {
            ScoreBound.Exact => true,
            ScoreBound.AtLeast => Value >= window.Beta,
            _ => Value <= window.Alpha,
        };
    }
}
