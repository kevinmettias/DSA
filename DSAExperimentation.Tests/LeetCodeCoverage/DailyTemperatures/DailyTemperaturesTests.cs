using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DailyTemperatures;

// LeetCode 739. Daily Temperatures: a monotonic decreasing Stack<int> of pending day
// indices (NextGreaterElementI/II precedent for this repo's own Stack) - each newly
// warmer day pops every still-waiting colder day off the top and records its wait as
// the index gap, one O(n) pass with no per-day rescan.
public sealed partial class DailyTemperaturesTests
{
    [Fact]
    public void DailyTemperatures_ClassicExample_ReturnsWaitDaysPerIndex()
    {
        int[] temperatures = [73, 74, 75, 71, 69, 72, 76, 73];

        var result = DailyTemperatures(temperatures);

        Assert.Equal([1, 1, 4, 2, 1, 1, 0, 0], result);
    }

    [Fact]
    public void DailyTemperatures_StrictlyDecreasing_ReturnsAllZeros()
    {
        int[] temperatures = [80, 70, 60, 50];

        var result = DailyTemperatures(temperatures);

        Assert.Equal([0, 0, 0, 0], result);
    }

    [Fact]
    public void DailyTemperatures_SingleDay_ReturnsZero()
    {
        int[] temperatures = [60];

        var result = DailyTemperatures(temperatures);

        Assert.Equal([0], result);
    }

    private static int[] DailyTemperatures(int[] temperatures)
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
