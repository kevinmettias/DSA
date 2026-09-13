using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.New21Game;

// LeetCode 837. New 21 Game: Alice keeps drawing a uniform random 1..maxPts while
// her running total is below k, then stops; report the probability her final total
// is at most n.
//
// The state is just the running total, and the probability of winning from a total
// below k is the mean of the probabilities of the maxPts totals one draw away. Both
// strategies answer that same recurrence and differ only in whether a total is
// remembered: the baseline re-derives every total once per draw order that reaches
// it (exponential, since many orders sum to the same points), the memoized strategy
// computes each total once via this repo's own Memoizer<TState,TResult> - the same
// (state)->probability shape SoupServingsSolution uses over its pair of remaining
// soup amounts, here over a single running total.
internal static class New21GameSolution
{
    private const int Start = 0;

    // The textbook baseline: plain recursion over the running total with no caching,
    // so a total reachable by many draw orders is recomputed once per order.
    // Deliberately written without this repo's primitives - it is the arm the
    // memoized strategy has to justify itself against.
    public static double ProbabilityByUnmemoizedRecursion(int n, int k, int maxPts) =>
        Probability(Start, n, k, maxPts);

    private static double Probability(int points, int n, int k, int maxPts)
    {
        if (points >= k)
        {
            return points <= n ? 1.0 : 0.0;
        }

        var total = 0.0;

        for (var draw = 1; draw <= maxPts; draw++)
        {
            total += Probability(points + draw, n, k, maxPts);
        }

        return total / maxPts;
    }

    // The same recurrence routed through this repo's own Memoizer, keyed on the
    // running total, so each of the O(k + maxPts) reachable totals is computed once.
    public static double ProbabilityByMemoizedRecursion(int n, int k, int maxPts)
    {
        return Memoizer.Memoize<int, double>(Start, Recurrence);

        // Closes over n, k and maxPts so the memo key stays the running total alone.
        double Recurrence(int points, Func<int, double> probability)
        {
            if (points >= k)
            {
                return points <= n ? 1.0 : 0.0;
            }

            var total = 0.0;

            for (var draw = 1; draw <= maxPts; draw++)
            {
                total += probability(points + draw);
            }

            return total / maxPts;
        }
    }
}
