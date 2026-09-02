using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfWaysToWearDifferentHatsToEachOther;

// LeetCode 1434. Number of Ways to Wear Different Hats to Each Other: the
// standard "iterate hats outward, assign-to-people bitmask" DP, expressed as a
// memoized recursion over the tuple state (hat, peopleAssignedMask) via this
// repo's own Memoizer - exactly the tuple-state shape its own doc comment names
// as the intended use case ("a tuple like (int Row, int Col) for 2D DP"), the
// same "closed set of states, revisited from many different paths" idiom
// CanIWinTests.cs already proves with a bare int bitmask. f(hat, mask) = number
// of ways to assign hats 1..hat so that exactly the people in mask already have
// one; mask == 0 short-circuits to 1 way (nothing left to assign) regardless of
// how many hats remain, keeping recursion depth bounded by hat count rather than
// by how sparse the remaining mask is.
public sealed class NumberOfWaysToWearDifferentHatsToEachOtherTests
{
    private const int Modulo = 1_000_000_007;
    private const int MaxHat = 40;

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberWays_EachPersonGetsADistinctLikedHat_MatchesDirectEnumeration(int[][] hats, int expected)
        => Assert.Equal(expected, NumberWays(hats));

    // Expected counts verified independently by direct enumeration of every
    // person-to-hat assignment respecting each person's own preference list and
    // requiring all assigned hats be pairwise distinct.
    public static IEnumerable<object[]> Examples()
    {
        yield return [new[] { new[] { 3, 4 }, new[] { 4, 5 } }, 3];
        yield return [new[] { new[] { 3, 5, 1 }, new[] { 3, 5 } }, 4];
        yield return [new[] { new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3, 4 }, new[] { 1, 2, 3, 4 } }, 24];
    }

    private static int NumberWays(int[][] hats)
    {
        var peopleCount = hats.Length;
        var hatToPeople = BuildHatToPeople(hats);
        var fullMask = (1 << peopleCount) - 1;

        var totalWays = Memoizer.Memoize<(int Hat, int Mask), long>(
            (MaxHat, fullMask), (state, waysFor) => WaysFor(state, waysFor, hatToPeople));

        return (int)totalWays;
    }

    private static List<int>[] BuildHatToPeople(int[][] hats)
    {
        var hatToPeople = new List<int>[MaxHat + 1];
        for (var hat = 1; hat <= MaxHat; hat++)
        {
            hatToPeople[hat] = [];
        }

        for (var person = 0; person < hats.Length; person++)
        {
            foreach (var hat in hats[person])
            {
                hatToPeople[hat].Add(person);
            }
        }

        return hatToPeople;
    }

    private static long WaysFor((int Hat, int Mask) state, Func<(int Hat, int Mask), long> waysFor, List<int>[] hatToPeople)
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

        var total = waysFor((hat - 1, mask));
        return AccumulateAssignments(state, waysFor, hatToPeople, total);
    }

    private static long AccumulateAssignments(
        (int Hat, int Mask) state, Func<(int Hat, int Mask), long> waysFor, List<int>[] hatToPeople, long total)
    {
        var (hat, mask) = state;

        foreach (var person in hatToPeople[hat])
        {
            var personBit = 1 << person;
            if ((mask & personBit) != 0)
            {
                total = (total + waysFor((hat - 1, mask & ~personBit))) % Modulo;
            }
        }

        return total;
    }
}
