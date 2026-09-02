using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountAllPossibleRoutes;

// LeetCode 1575. Count All Possible Routes: the recurrence
// ways(city, fuel) = [city == finish] + sum over every other city j reachable with
// the fuel remaining of ways(j, fuel - cost) - via this repo's own Memoizer keyed on
// a (City, Fuel) tuple state, the same 2-D tuple-state shape CoinChangeIITests'
// (Index, Remaining) and ParallelCoursesIITests' bitmask state already establish for
// this exact "Memoizer over a value-tuple state" idiom.
public sealed partial class CountAllPossibleRoutesTests
{
    private const int Mod = 1_000_000_007;

    public static TheoryData<int[], RouteQuery, int> LocationsAndQueryToExpectedRouteCount => new()
    {
        { new[] { 2, 3, 6, 8, 4 }, new RouteQuery(1, 3, 5), 4 },
        { new[] { 4, 3, 1 }, new RouteQuery(1, 0, 6), 5 },
        { new[] { 5, 2, 1 }, new RouteQuery(0, 2, 3), 0 },
    };

    [Theory]
    [MemberData(nameof(LocationsAndQueryToExpectedRouteCount))]
    public void CountRoutes_LeetCodeExamples_ReturnsRouteCount(int[] locations, RouteQuery query, int expected)
    {
        var actual = CountRoutes(locations, query.Start, query.Finish, query.Fuel);
        Assert.Equal(expected, actual);
    }

    public readonly record struct RouteQuery(int Start, int Finish, int Fuel);

    private static int CountRoutes(int[] locations, int start, int finish, int fuel)
    {
        return Memoizer.Memoize<(int City, int Fuel), int>((start, fuel), WaysFrom);

        int WaysFrom((int City, int Fuel) state, Func<(int City, int Fuel), int> ways)
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
                    total = (total + ways((next, remaining - cost))) % Mod;
                }
            }

            return total;
        }
    }
}
