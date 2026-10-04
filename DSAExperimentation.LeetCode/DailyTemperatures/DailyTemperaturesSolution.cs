using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.DailyTemperatures;

// LeetCode 739. Daily Temperatures: for each day, how many days until a warmer one.
//
// The naive baseline rescans forward from every day until it finds a warmer one -
// O(n^2) worst case on a strictly decreasing run. The composed strategy is one
// NearestBoundary.GreaterToTheRight sweep - each day's nearest strictly warmer day
// ahead, found by a monotonic stack that pushes every day once and pops it at most
// once - and records each wait as the index gap to that day; a day with no warmer
// day ahead keeps the 0 LeetCode expects.
internal static class DailyTemperaturesSolution
{
    private const int NoWarmerDay = -1;

    // The textbook answer: rescan forward from every day until a warmer one turns
    // up. Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int[] WaitDaysByBruteForceScan(int[] temperatures)
    {
        var result = new int[temperatures.Length];

        for (var day = 0; day < temperatures.Length; day++)
        {
            for (var later = day + 1; later < temperatures.Length; later++)
            {
                if (temperatures[later] > temperatures[day])
                {
                    result[day] = later - day;
                    break;
                }
            }
        }

        return result;
    }

    public static int[] WaitDaysByMonotonicStackSweep(int[] temperatures)
    {
        var nextWarmerDay = NearestBoundary.GreaterToTheRight(temperatures, NoWarmerDay);
        var result = new int[temperatures.Length];

        for (var day = 0; day < temperatures.Length; day++)
        {
            if (nextWarmerDay[day] != NoWarmerDay)
            {
                result[day] = nextWarmerDay[day] - day;
            }
        }

        return result;
    }
}
