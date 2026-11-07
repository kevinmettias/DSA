using System.Numerics;

namespace DSAExperimentation.Algorithms.GameSearch;

// Alpha-beta's cutoff: once the mover is sure of as much as the opponent will allow, the opponent
// would never let the position arise, so its remaining moves need no search.
internal readonly struct AlphaBetaCutoff<TScore> : ICutoffRule<TScore>
    where TScore : INumber<TScore>
{
    public static bool IsCutoff(TScore alpha, TScore beta) => alpha >= beta;
}
