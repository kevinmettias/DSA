using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumSwapsToMakeSequencesIncreasing;

// LeetCode 801. Minimum Swaps To Make Sequences Increasing: at each index, either
// keep or swap nums1[i]/nums2[i] so both arrays end up strictly increasing, for the
// minimum total swaps.
//
// Both strategies answer the same two-state recurrence - state (index, wasSwapped)
// -> minimum swaps for that prefix - and differ only in how the states are stored:
// the baseline materializes a flat keep[i]/swap[i] table, while the composed arm
// lets this repo's own Memoizer discover the states the recurrence actually reaches,
// the same HouseRobberII/DecodeWays shape.
//
// Precondition (guaranteed by LC 801's own constraints): at least one valid
// keep/swap assignment exists, so Cost below always finds a valid transition and
// never has to reason about an all-invalid state.
internal static class MinimumSwapsToMakeSequencesIncreasingSolution
{
    // Swapping one index costs one swap; keeping it costs none.
    private const int SwapCost = 1;

    // The textbook answer: a flat keep[i]/swap[i] tabulation over two BCL arrays,
    // walked left to right. Deliberately written without this repo's Memoizer - it
    // is the arm the composed solution below has to justify itself against.
    public static int MinSwapByTabulation(int[] nums1, int[] nums2)
    {
        var arrays = new SwapArrays(nums1, nums2);
        var last = nums1.Length - 1;
        var rows = new TabulationRows(new int[nums1.Length], new int[nums1.Length]);
        rows.Swap[0] = SwapCost;

        for (var i = 1; i < nums1.Length; i++)
        {
            UpdateTabulationStep(arrays, rows, i);
        }

        return Math.Min(rows.Keep[last], rows.Swap[last]);
    }

    private static void UpdateTabulationStep(SwapArrays arrays, TabulationRows rows, int i)
    {
        rows.Keep[i] = int.MaxValue;
        rows.Swap[i] = int.MaxValue;

        if (arrays.Nums1[i] > arrays.Nums1[i - 1] && arrays.Nums2[i] > arrays.Nums2[i - 1])
        {
            rows.Keep[i] = Math.Min(rows.Keep[i], rows.Keep[i - 1]);
            rows.Swap[i] = Math.Min(rows.Swap[i], rows.Swap[i - 1] + SwapCost);
        }

        if (arrays.Nums1[i] > arrays.Nums2[i - 1] && arrays.Nums2[i] > arrays.Nums1[i - 1])
        {
            rows.Keep[i] = Math.Min(rows.Keep[i], rows.Swap[i - 1]);
            rows.Swap[i] = Math.Min(rows.Swap[i], rows.Keep[i - 1] + SwapCost);
        }
    }

    // This repo's own Memoizer closing over the same (index, wasSwapped) recurrence:
    // the answer is the cheaper of the two states at the last index, and every prefix
    // state it depends on is computed once and cached.
    public static int MinSwapByMemoizedTwoState(int[] nums1, int[] nums2)
    {
        var arrays = new SwapArrays(nums1, nums2);
        var last = nums1.Length - 1;

        var keepCost = Memoizer.Memoize<(int Index, bool Swapped), int>(
            (last, false), (state, cost) => Cost(arrays, state, cost));
        var swapCost = Memoizer.Memoize<(int Index, bool Swapped), int>(
            (last, true), (state, cost) => Cost(arrays, state, cost));

        return Math.Min(keepCost, swapCost);
    }

    private static int Cost(SwapArrays arrays, (int Index, bool Swapped) state, Func<(int Index, bool Swapped), int> cost)
    {
        var (i, swapped) = state;

        if (i == 0)
        {
            return swapped ? SwapCost : 0;
        }

        var curA = swapped ? arrays.Nums2[i] : arrays.Nums1[i];
        var curB = swapped ? arrays.Nums1[i] : arrays.Nums2[i];
        var best = BestSwapChoice(arrays, i, (curA, curB), cost);

        return best + (swapped ? SwapCost : 0);
    }

    private static int BestSwapChoice(SwapArrays arrays, int i, (int CurA, int CurB) current, Func<(int Index, bool Swapped), int> cost)
    {
        var best = int.MaxValue;

        if (current.CurA > arrays.Nums1[i - 1] && current.CurB > arrays.Nums2[i - 1])
        {
            best = Math.Min(best, cost((i - 1, false)));
        }

        if (current.CurA > arrays.Nums2[i - 1] && current.CurB > arrays.Nums1[i - 1])
        {
            best = Math.Min(best, cost((i - 1, true)));
        }

        return best;
    }

    private readonly record struct SwapArrays(int[] Nums1, int[] Nums2);

    private readonly record struct TabulationRows(int[] Keep, int[] Swap);
}
