using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PrisonCellsAfterNDays;

// LeetCode 957. Prison Cells After N Days: a cell is occupied tomorrow exactly
// when both its neighbors match today, and the two end cells are always vacant
// from day one onwards because they have no pair of neighbors.
//
// Packing the 8 cells into the low bits of an int makes a day's transition a
// handful of shifts, which is all the naive arm needs. The state space is only
// 256 encodings wide, so the sequence of days must repeat almost immediately -
// and that is what the composed arm exploits: this repo's own HashMap<int,int>
// records the day each encoded state was first seen (the same "have I seen this
// key before" role TwoSumSolution uses HashMap for, keyed on a packed state
// instead of a value), so the walk can skip to N modulo the cycle length rather
// than stepping through all of N, which LeetCode allows to reach 10^9.
internal static class PrisonCellsAfterNDaysSolution
{
    private const int CellCount = 8;

    // The two end cells never have both neighbors, so only 1..6 can ever be
    // occupied after the first transition.
    private const int FirstInteriorCell = 1;
    private const int InteriorCellUpperBound = CellCount - 1;

    // The textbook answer: step one day at a time, all N of them. Deliberately
    // written without this repo's primitives - it is the arm the composed
    // solution below has to justify itself against.
    public static int[] CellsAfterNDaysByDailySimulation(int[] cells, int n)
    {
        var state = Encode(cells);

        for (var day = 0; day < n; day++)
        {
            state = NextState(state);
        }

        return Decode(state);
    }

    // Record each encoded state against the day it was first seen in this repo's
    // own HashMap<int,int>; the first repeat closes a cycle, and the remaining
    // days collapse to (n - day) modulo that cycle's length.
    public static int[] CellsAfterNDaysByCycleDetection(int[] cells, int n)
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

        for (var i = FirstInteriorCell; i < InteriorCellUpperBound; i++)
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
        var cells = new int[CellCount];

        for (var i = 0; i < CellCount; i++)
        {
            cells[i] = (state >> i) & 1;
        }

        return cells;
    }
}
