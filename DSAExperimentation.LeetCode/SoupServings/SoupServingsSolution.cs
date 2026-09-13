using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.SoupServings;

// LeetCode 808. Soup Servings: a probability recurrence over the remaining
// (soupA, soupB) amounts, quantized to units of 25ml - each of the four equally
// likely serving choices pours a combined 4 units per turn, so the state space is
// the pair of remaining unit counts.
//
// Both strategies answer the same question and differ only in whether the pair is
// remembered: the baseline re-derives every (a, b) once per serving order that
// reaches it (exponential), the memoized strategy computes each pair once via this
// repo's own Memoizer<TState,TResult> - the same (state)->probability shape
// PredictTheWinnerSolution uses over an interval's (left, right) bounds.
internal static class SoupServingsSolution
{
    private const int ServingSizeMl = 25;
    private const int CeilingRoundingOffset = 24;
    private const double TieProbability = 0.5;
    private const double BranchProbability = 0.25;
    private const int FourUnitPour = 4;
    private const int ThreeUnitPour = 3;
    private const int TwoUnitPour = 2;

    // Past this point the answer is within 1e-5 of 1.0 (LeetCode's own accepted
    // precedent for this problem), so the recursion is short-circuited rather than
    // run out to a state space that keeps growing with n for no observable change
    // in the reported answer. It belongs to the PROBLEM, not to either strategy, so
    // both arms apply it - which is also what lets the baseline be asserted against
    // the same large-n example the memoized arm is.
    private const int LargeNThreshold = 4800;

    // The textbook baseline: plain recursion over the remaining (a, b) pair with no
    // caching at all, so a pair reachable by many different serving orders is
    // recomputed once per order. Deliberately written without this repo's
    // primitives - it is the arm the memoized strategy has to justify itself
    // against.
    public static double ProbabilityByUnmemoizedRecursion(int n)
    {
        if (n >= LargeNThreshold)
        {
            return 1.0;
        }

        var servings = ServingsFor(n);
        return Probability(servings, servings);
    }

    private static double Probability(int a, int b)
    {
        if (a <= 0 && b <= 0)
        {
            return TieProbability;
        }

        if (a <= 0)
        {
            return 1.0;
        }

        if (b <= 0)
        {
            return 0.0;
        }

        return BranchProbability * (
            Probability(a - FourUnitPour, b)
            + Probability(a - ThreeUnitPour, b - 1)
            + Probability(a - TwoUnitPour, b - TwoUnitPour)
            + Probability(a - 1, b - ThreeUnitPour));
    }

    // The same recurrence routed through this repo's own Memoizer, keyed on the
    // remaining (A, B) pair, so each of the O(servings^2) states is computed once.
    public static double ProbabilityByMemoizedRecursion(int n)
    {
        if (n >= LargeNThreshold)
        {
            return 1.0;
        }

        var servings = ServingsFor(n);
        return Memoizer.Memoize<(int A, int B), double>((servings, servings), ProbabilityMemoized);
    }

    private static double ProbabilityMemoized((int A, int B) remaining, Func<(int A, int B), double> probability)
    {
        var (a, b) = remaining;

        if (a <= 0 && b <= 0)
        {
            return TieProbability;
        }

        if (a <= 0)
        {
            return 1.0;
        }

        if (b <= 0)
        {
            return 0.0;
        }

        return BranchProbability * (
            probability((a - FourUnitPour, b))
            + probability((a - ThreeUnitPour, b - 1))
            + probability((a - TwoUnitPour, b - TwoUnitPour))
            + probability((a - 1, b - ThreeUnitPour)));
    }

    // Every pour is a multiple of 25ml, so n millilitres is ceil(n / 25) servings.
    private static int ServingsFor(int n) => (n + CeilingRoundingOffset) / ServingSizeMl;
}
