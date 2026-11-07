using System.Numerics;

namespace DSAExperimentation.Algorithms.GameSearch;

// Whether a depth-limited search may stop trying a position's moves: once what the mover is sure
// of (alpha) reaches what the opponent will allow (beta), no later move can change the value the
// position's parent sees. Alpha-beta stops there; plain negamax tries every move anyway. A static
// member of a struct, so each search compiles its own rule, and the rule that never stops folds
// the test away.
internal interface ICutoffRule<TScore>
    where TScore : INumber<TScore>
{
    // Whether to stop trying moves, given the window as it now stands.
    static abstract bool IsCutoff(TScore alpha, TScore beta);
}
