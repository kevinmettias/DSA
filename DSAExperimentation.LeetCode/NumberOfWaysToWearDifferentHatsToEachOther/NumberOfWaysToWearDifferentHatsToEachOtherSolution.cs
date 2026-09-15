using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfWaysToWearDifferentHatsToEachOther;

// LeetCode 1434. Number of Ways to Wear Different Hats to Each Other: count the
// assignments giving every person a hat they like, with no hat worn twice, modulo
// 1e9+7.
//
// Both strategies are the same recurrence, iterating hats outward rather than people
// outward because there are up to 40 hats but at most 10 people, so the "who already
// has a hat" set fits in a bitmask: f(hat, mask) = the number of ways to hand out
// hats 1..hat so that exactly the people in mask still need one. Hat `hat` is either
// left unworn - f(hat - 1, mask) - or given to one person who likes it and still
// needs one. mask == 0 short-circuits to 1 regardless of how many hats remain, which
// bounds recursion depth by hat count rather than by how sparse the mask is.
//
// The two arms differ only in whether that recurrence remembers anything: many
// different hat orderings reach the identical (hat, mask) state, so the unmemoized
// arm re-explores each one from scratch.
internal static class NumberOfWaysToWearDifferentHatsToEachOtherSolution
{
    // The textbook answer: the same plain recursion with no cache behind it - the
    // recurrence below invoked with itself as its own recursion, rather than handed
    // to this repo's Memoizer. It is the arm the memoized strategy beneath has to
    // justify itself against, and putting it here is what finally gets it asserted.
    public static int NumberWaysByBruteForceRecursion(int[][] hats) =>
        NumberWaysByBruteForceRecursion(HatPreferences.Build(hats));

    public static int NumberWaysByBruteForceRecursion(HatPreferences preferences)
    {
        var ways = new WaysByHatAndMask(preferences);

        return (int)ways.Replay((preferences.HatCount, preferences.EveryoneMask), ways);
    }

    // The same recurrence routed through this repo's own Memoizer, keyed on the
    // tuple state (hat, peopleStillNeedingAHat) - exactly the tuple-state shape
    // Memoizer's own doc comment names as its intended use case, and the same
    // "closed set of states, revisited from many different paths" idiom CanIWin
    // already uses with a bare int bitmask.
    public static int NumberWaysByMemoizedBitmask(int[][] hats) =>
        NumberWaysByMemoizedBitmask(HatPreferences.Build(hats));

    public static int NumberWaysByMemoizedBitmask(HatPreferences preferences)
    {
        var totalWays = Memoizer.Memoize<(int Hat, int Mask), long>(
            (preferences.HatCount, preferences.EveryoneMask),
            new WaysByHatAndMask(preferences));

        return (int)totalWays;
    }

    // The recurrence itself, named: hat `hat` is either left unworn, or given to one
    // person who likes it and still needs one. mask == 0 is a finished assignment
    // whatever is left of the hats, which is why it short-circuits first.
    private sealed class WaysByHatAndMask(HatPreferences preferences)
        : IRecurrence<(int Hat, int Mask), long>
    {
        public long Replay((int Hat, int Mask) state, IRecurrence<(int Hat, int Mask), long> rest)
        {
            var (hat, mask) = state;

            if (mask == 0)
            {
                return 1L;
            }

            if (hat == 0)
            {
                return 0L;
            }

            var leaveHatUnworn = rest.Replay((hat - 1, mask), rest);

            return AccumulateAssignments(state, rest, preferences, leaveHatUnworn);
        }
    }

    private static long AccumulateAssignments(
        (int Hat, int Mask) state,
        IRecurrence<(int Hat, int Mask), long> rest,
        HatPreferences preferences,
        long total)
    {
        var (hat, mask) = state;

        foreach (var person in preferences.PeopleWhoLike(hat))
        {
            var personBit = 1 << person;

            if ((mask & personBit) != 0)
            {
                total = (total + rest.Replay((hat - 1, mask & ~personBit), rest)) % ModularArithmetic.Modulo;
            }
        }

        return total;
    }
}
