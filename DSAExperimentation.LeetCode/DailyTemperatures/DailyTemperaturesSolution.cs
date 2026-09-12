using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.DailyTemperatures;

// LeetCode 739. Daily Temperatures: for each day, how many days until a warmer one.
//
// The naive baseline rescans forward from every day until it finds a warmer one -
// O(n^2) worst case on a strictly decreasing run. The composed strategy walks once,
// keeping a monotonic decreasing Stack<int> of day indices still waiting for a
// warmer day; each newly warmer day pops every colder day still on top and records
// its wait as the index gap (NextGreaterElementI/II precedent for this repo's own
// Stack) - every day is pushed once and popped at most once.
internal static class DailyTemperaturesSolution
{
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
        var result = new int[temperatures.Length];
        var pendingIndices = new RepoIntStack();

        for (var day = 0; day < temperatures.Length; day++)
        {
            while (pendingIndices.TryPeek(out var previousDay) && temperatures[previousDay] < temperatures[day])
            {
                pendingIndices.TryPop(out _);
                result[previousDay] = day - previousDay;
            }

            pendingIndices.Push(day);
        }

        return result;
    }
}
