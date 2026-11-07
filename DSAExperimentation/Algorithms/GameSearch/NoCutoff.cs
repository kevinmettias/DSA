using System.Numerics;

namespace DSAExperimentation.Algorithms.GameSearch;

// No cutoff: every move of every position within the depth is searched, which is plain negamax.
internal readonly struct NoCutoff<TScore> : ICutoffRule<TScore>
    where TScore : INumber<TScore>
{
    public static bool IsCutoff(TScore alpha, TScore beta) => false;
}
