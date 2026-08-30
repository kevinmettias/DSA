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

    [Theory]
    [InlineData(new[] { 2, 3, 6, 8, 4 }, 1, 3, 5, 4)]
    [InlineData(new[] { 4, 3, 1 }, 1, 0, 6, 5)]
    [InlineData(new[] { 5, 2, 1 }, 0, 2, 3, 0)]
    public void CountRoutes_LeetCodeExamples_ReturnsRouteCount(int[] locations, int start, int finish, int fuel, int expected)
        => Assert.Equal(expected, CountRoutes(locations, start, finish, fuel));

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
