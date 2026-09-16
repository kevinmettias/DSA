using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.New21Game;

// LeetCode 837. New 21 Game: Alice keeps drawing a uniform random 1..maxPts while
// her running total is below stopAt, then stops; report the probability her final
// total is at most limit.
//
// The state is just the running total, and the probability of winning from a total
// below stopAt is the mean of the probabilities of the maxPts totals one draw away.
// Both strategies answer that same recurrence and differ only in whether a total is
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
    public static double ProbabilityByUnmemoizedRecursion(
        int limit, int stopAt, int maxPts) =>
        Probability(Start, limit, stopAt, maxPts);

    // The same recurrence routed through this repo's own Memoizer, keyed on the
    // running total, so each of the O(stopAt + maxPts) reachable totals is computed
    // once.
    public static double ProbabilityByMemoizedRecursion(
        int limit, int stopAt, int maxPts) =>
        Memoizer.Memoize<int, double>(Start, new WinningProbability(limit, stopAt, maxPts));

    private static double Probability(int points, int limit, int stopAt, int maxPts)
    {
        if (points >= stopAt)
        {
            return points <= limit ? 1.0 : 0.0;
        }

        var total = 0.0;

        for (var draw = 1; draw <= maxPts; draw++)
        {
            total += Probability(points + draw, limit, stopAt, maxPts);
        }

        return total / maxPts;
    }

    // The recurrence, as a named type: the chance of finishing at or below the limit from
    // a running total is the mean of the chances from each total one draw away - or the
    // settled 1-or-0 once the total has reached the stop point and drawing has ended.
    private sealed class WinningProbability(int limit, int stopAt, int faceCount)
        : IRecurrence<int, double>
    {
        public double Replay(int state, IRecurrence<int, double> rest)
        {
            if (state >= stopAt)
            {
                return state <= limit ? 1.0 : 0.0;
            }

            var total = 0.0;

            for (var draw = 1; draw <= faceCount; draw++)
            {
                total += rest.Replay(state + draw, rest);
            }

            return total / faceCount;
        }
    }
}
