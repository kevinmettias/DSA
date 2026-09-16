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
        var rows = BuildTabulationRows(nums1, nums2);
        var last = nums1.Length - 1;

        return Math.Min(rows.Keep[last], rows.Swap[last]);
    }

    // The keep/swap table itself: both arrays paired up, index 0 already carrying the
    // swap cost, and every later index filled in from the one before it.
    private static TabulationRows BuildTabulationRows(int[] nums1, int[] nums2)
    {
        var arrays = new SwapArrays(nums1, nums2);
        var rows = new TabulationRows(new int[nums1.Length], new int[nums1.Length]);
        rows.Swap[0] = SwapCost;

        for (var i = 1; i < nums1.Length; i++)
        {
            UpdateTabulationStep(arrays, rows, i);
        }

        return rows;
    }

    private static void UpdateTabulationStep(SwapArrays arrays, TabulationRows rows, int index)
    {
        rows.Keep[index] = int.MaxValue;
        rows.Swap[index] = int.MaxValue;

        if (arrays.Nums1[index] > arrays.Nums1[index - 1]
            && arrays.Nums2[index] > arrays.Nums2[index - 1])
        {
            rows.Keep[index] = Math.Min(rows.Keep[index], rows.Keep[index - 1]);
            rows.Swap[index] = Math.Min(rows.Swap[index], rows.Swap[index - 1] + SwapCost);
        }

        if (arrays.Nums1[index] > arrays.Nums2[index - 1]
            && arrays.Nums2[index] > arrays.Nums1[index - 1])
        {
            rows.Keep[index] = Math.Min(rows.Keep[index], rows.Swap[index - 1]);
            rows.Swap[index] = Math.Min(rows.Swap[index], rows.Keep[index - 1] + SwapCost);
        }
    }

    // This repo's own Memoizer closing over the same (index, wasSwapped) recurrence:
    // the answer is the cheaper of the two states at the last index, and every prefix
    // state it depends on is computed once and cached.
    public static int MinSwapByMemoizedTwoState(int[] nums1, int[] nums2)
    {
        var arrays = new SwapArrays(nums1, nums2);
        var last = nums1.Length - 1;
        var costs = MemoizedCostsAt(arrays, last);

        return Math.Min(costs.Keep, costs.Swap);
    }

    // Both arms of the same last index, memoized side by side.
    private static (int Keep, int Swap) MemoizedCostsAt(SwapArrays arrays, int last)
    {
        var keepCost = MemoizedCostAt(arrays, last, IndexState.Kept);
        var swapCost = MemoizedCostAt(arrays, last, IndexState.Swapped);

        return (keepCost, swapCost);
    }

    // One state's own memoized cost: the same (index, state) recurrence, seeded at
    // the state asked for and discovered backwards from there by the Memoizer.
    private static int MemoizedCostAt(SwapArrays arrays, int index, IndexState state)
        => Memoizer.Memoize((index, state), new CostFromIndexState(arrays));

    // The recurrence, as a named type: what it costs to reach one index in one of its two
    // states, read backwards from the state asked for - index 0 pays the swap cost if it
    // was swapped, and every later index pays it only when this transition swaps it.
    private sealed class CostFromIndexState(SwapArrays arrays) : IRecurrence<(int Index, IndexState State), int>
    {
        public int Replay((int Index, IndexState State) state, IRecurrence<(int Index, IndexState State), int> rest)
        {
            var (i, current) = state;

            if (i == 0)
            {
                return current == IndexState.Swapped ? SwapCost : 0;
            }

            var curA = current == IndexState.Swapped ? ElementAt(arrays.Nums2, i) : ElementAt(arrays.Nums1, i);
            var curB = current == IndexState.Swapped ? ElementAt(arrays.Nums1, i) : ElementAt(arrays.Nums2, i);
            var best = BestSwapChoice(i, (curA, curB), rest);

            return best + (current == IndexState.Swapped ? SwapCost : 0);
        }

        // Either predecessor is offered as a candidate - the previous index kept, or the
        // previous index swapped - and only the ones this pair actually accepts count.
        private int BestSwapChoice(
            int index, (int CurA, int CurB) current, IRecurrence<(int Index, IndexState State), int> rest)
        {
            var best = int.MaxValue;

            if (current.CurA > arrays.Nums1[index - 1] && current.CurB > arrays.Nums2[index - 1])
            {
                var precededByKept = rest.Replay((index - 1, IndexState.Kept), rest);
                best = Math.Min(best, precededByKept);
            }

            if (current.CurA > arrays.Nums2[index - 1] && current.CurB > arrays.Nums1[index - 1])
            {
                var precededBySwapped = rest.Replay((index - 1, IndexState.Swapped), rest);
                best = Math.Min(best, precededBySwapped);
            }

            return best;
        }
    }

    private static int ElementAt(int[] values, int index) => values[index];

    private readonly record struct SwapArrays(int[] Nums1, int[] Nums2);

    private readonly record struct TabulationRows(int[] Keep, int[] Swap);

    // Which of the two states at an index is being costed: the pair kept as it is,
    // or the pair exchanged. The tabulation arm's Keep/Swap rows are the same two,
    // and the memoized arm's state tuple carries one of them rather than a bare bool.
    private enum IndexState
    {
        Kept,
        Swapped,
    }
}
