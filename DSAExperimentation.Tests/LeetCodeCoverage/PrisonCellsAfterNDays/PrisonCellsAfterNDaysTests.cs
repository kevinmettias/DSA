using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrisonCellsAfterNDays;

// LeetCode 957. Prison Cells After N Days: the 8-cell state space has only 256
// possible encodings, so simulating all of N (up to 10^9) days directly is
// wasteful - the sequence of states must eventually repeat. This repo's own
// HashMap<int,int> maps each encoded state to the day index it was first seen
// (the same "have I seen this key before" role TwoSumTests already uses HashMap
// for, just keyed on a packed state instead of a value), letting the simulation
// detect the cycle and jump straight to day N modulo the cycle length instead of
// looping N times.
public sealed partial class PrisonCellsAfterNDaysTests
{
    [Fact]
    public void NextDayStates_ClassicExample_MatchesSevenDaySimulation()
    {
        int[] cells = [0, 1, 0, 1, 1, 0, 0, 1];

        var result = SimulateAfterNDays(cells, n: 7);

        Assert.Equal([0, 0, 1, 1, 0, 0, 0, 0], result);
    }

    [Fact]
    public void SimulateAfterNDays_LargeN_UsesCycleDetectionToMatchBruteForce()
    {
        int[] cells = [1, 0, 0, 1, 0, 0, 1, 0];
        const int n = 1_000_000;

        var expected = BruteForceSimulate(cells, n);
        var actual = SimulateAfterNDays(cells, n);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SimulateAfterNDays_ZeroDays_ReturnsInputUnchanged()
    {
        int[] cells = [0, 1, 0, 1, 1, 0, 0, 1];

        var result = SimulateAfterNDays(cells, n: 0);

        Assert.Equal(cells, result);
    }

    private static int[] SimulateAfterNDays(int[] cells, int n)
    {
        var seenAtDay = new HashMap<int, int>();
        var state = Encode(cells);
        var day = 0;

        while (day < n)
        {
            if (seenAtDay.TryGetValue(state, out var firstSeenDay))
            {
                var finalState = JumpToCycleEnd(state, day, firstSeenDay, n);
                return Decode(finalState);
            }

            seenAtDay.Set(state, day);
            state = NextState(state);
            day++;
        }

        return Decode(state);
    }

    private static int JumpToCycleEnd(int state, int day, int firstSeenDay, int n)
    {
        var cycleLength = day - firstSeenDay;
        var remaining = (n - day) % cycleLength;

        for (var i = 0; i < remaining; i++)
        {
            state = NextState(state);
        }

        return state;
    }

    private static int NextState(int state)
    {
        var next = 0;

        for (var i = 1; i < 7; i++)
        {
            var left = (state >> (i - 1)) & 1;
            var right = (state >> (i + 1)) & 1;

            if (left == right)
            {
                next |= 1 << i;
            }
        }

        return next;
    }

    private static int Encode(int[] cells)
    {
        var state = 0;

        for (var i = 0; i < cells.Length; i++)
        {
            if (cells[i] == 1)
            {
                state |= 1 << i;
            }
        }

        return state;
    }

    private static int[] Decode(int state)
    {
        var cells = new int[8];

        for (var i = 0; i < 8; i++)
        {
            cells[i] = (state >> i) & 1;
        }

        return cells;
    }

    private static int[] BruteForceSimulate(int[] cells, int n)
    {
        var state = Encode(cells);

        for (var day = 0; day < n; day++)
        {
            state = NextState(state);
        }

        return Decode(state);
    }
}
