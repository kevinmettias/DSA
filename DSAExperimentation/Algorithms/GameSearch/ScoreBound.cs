namespace DSAExperimentation.Algorithms.GameSearch;

// What a value a fail-soft search returned says about the position's true value at that depth:
// exactly it, when it fell inside the window; at least it, when it reached beta and the search cut
// off; at most it, when it stayed at or below alpha and every move failed low.
internal enum ScoreBound : byte
{
    Exact,
    AtLeast,
    AtMost,
}
