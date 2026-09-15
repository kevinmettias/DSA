using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountAllPossibleRoutes;

// LeetCode 1575. Count All Possible Routes: count the routes from start to finish
// that never run the fuel tank below zero, modulo 1e9+7. A route may re-visit a
// city, so the answer is a walk count rather than a path count.
//
// The recurrence is the same for both strategies:
//   ways(city, fuel) = [city == finish] + sum over every other city j of
//                      ways(j, fuel - |locations[city] - locations[j]|)
// over the moves that city's remaining fuel can still pay for. Every move costs at
// least one unit (LeetCode guarantees distinct locations), so the state graph is
// well-founded and the recursion terminates.
//
// They differ only in whether the (city, fuel) state is remembered: the baseline
// re-derives it on every path that reaches it - exponential - while the composed
// strategy routes the same recurrence through this repo's own Memoizer keyed on a
// (City, Fuel) tuple state, the 2-D value-tuple state shape CoinChangeII's
// (Index, Remaining) already establishes, collapsing the work to
// O(locations.Length^2 * fuel).
internal static class CountAllPossibleRoutesSolution
{
    // The textbook answer: plain recursion, no cache, BCL arithmetic only. This is
    // the arm the memoized strategy below has to justify itself against.
    public static int CountRoutesByNaiveRecursion(int[] locations, int start, int finish, int fuel) =>
        WaysWithoutCache(locations, finish, start, fuel);

    // Same recurrence driven top-down through Memoizer, so each (city, fuel) state is
    // solved once and shared by every branch that reaches it.
    public static int CountRoutesByMemoizedRecurrence(int[] locations, int start, int finish, int fuel) =>
        Memoizer.Memoize<(int City, int Fuel), int>((start, fuel), new WaysFrom(locations, finish));

    // The rule, named: a state counts one route when it already stands on the finish,
    // plus one route for every city the fuel still left can pay to reach.
    private sealed class WaysFrom(int[] locations, int finish) : IRecurrence<(int City, int Fuel), int>
    {
        public int Replay(
            (int City, int Fuel) state,
            IRecurrence<(int City, int Fuel), int> rest)
        {
            var (city, remaining) = state;
            var total = city == finish ? 1 : 0;

            for (var next = 0; next < locations.Length; next++)
            {
                if (next == city)
                {
                    continue;
                }

                var cost = Math.Abs(locations[city] - locations[next]);
                if (cost <= remaining)
                {
                    var ways = rest.Replay((next, remaining - cost), rest);
                    total = AddModulo(total, ways);
                }
            }

            return total;
        }
    }

    private static int WaysWithoutCache(int[] locations, int finish, int city, int remaining)
    {
        var total = city == finish ? 1 : 0;

        for (var next = 0; next < locations.Length; next++)
        {
            if (next == city)
            {
                continue;
            }

            var cost = Math.Abs(locations[city] - locations[next]);
            if (cost <= remaining)
            {
                var ways = WaysWithoutCache(locations, finish, next, remaining - cost);
                total = AddModulo(total, ways);
            }
        }

        return total;
    }

    private static int AddModulo(int total, int addend) =>
        (int)((total + (long)addend) % ModularArithmetic.Modulo);
}
