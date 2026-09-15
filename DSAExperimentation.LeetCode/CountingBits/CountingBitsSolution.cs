using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CountingBits;

// LeetCode 338. Counting Bits: for every value from 0 to n, count its set bits.
//
// The baseline recomputes each value's popcount independently with Brian
// Kernighan's trick (v &= v - 1 drops the lowest set bit each iteration). The
// composed strategy instead leans on the classic DP recurrence
// ans[i] = ans[i >> 1] + (i & 1) - i's bit count is i>>1's bit count plus i's own
// low bit - driven through Memoizer, the same Memoize<TState,TResult> composition
// HouseRobberSolution/HouseRobberIISolution already use for a different recurrence.
internal static class CountingBitsSolution
{
    // Deliberately BCL: nothing here but the textbook per-value bit-clearing loop,
    // the arm the composed recurrence below has to justify itself against.
    public static int[] CountBitsByPerNumberLoop(int n)
    {
        var result = new int[n + 1];

        for (var i = 0; i <= n; i++)
        {
            var value = i;
            var count = 0;

            while (value != 0)
            {
                value &= value - 1;
                count++;
            }

            result[i] = count;
        }

        return result;
    }

    public static int[] CountBitsByMemoizedRecurrence(int n)
    {
        var result = new int[n + 1];

        for (var i = 0; i <= n; i++)
        {
            result[i] = Memoizer.Memoize<int, int>(i, new BitsShiftedFromHalf());
        }

        return result;
    }

    // The recurrence, as a named type: a value's bit count is the bit count of the
    // value halved, plus the value's own low bit.
    private sealed class BitsShiftedFromHalf : IRecurrence<int, int>
    {
        public int Replay(int value, IRecurrence<int, int> rest)
        {
            if (value == 0)
            {
                return 0;
            }

            return rest.Replay(value >> 1, rest) + (value & 1);
        }
    }
}
