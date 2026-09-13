using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.DistributeRepeatingIntegers;

// LeetCode 1655. Distribute Repeating Integers: can every customer be given exactly
// quantity[i] copies of one single value from nums? Two customers may share a value
// as long as its stock covers both.
//
// Only each distinct value's *count* matters, never the value itself, so nums
// collapses to a stock-per-value array and the puzzle becomes the k-bucket
// assignment PartitionToKEqualSumSubsets runs for LeetCode 698 - with orders as the
// items and a per-value stock as each bucket's capacity, instead of one shared
// target sum. Orders are placed largest first for the same pruning reason: the
// hardest order fails fastest.
internal static class DistributeRepeatingIntegersSolution
{
    // The textbook recursion: a hand-written DFS that tries each value's remaining
    // stock for the current order and undoes the placement on backtrack.
    // Deliberately written without this repo's Backtrack primitive - it is the arm
    // the composed solution below has to justify itself against.
    public static bool CanDistributeByNaiveBacktracking(int[] nums, int[] quantity)
        => SearchNaive(StockCountsOf(nums), OrdersDescending(quantity), 0);

    // Section 17.4's hoisted overload: the stock counts are already prepared, so
    // grouping nums is not charged to the search being measured.
    public static bool CanDistributeByNaiveBacktracking(DynamicArray<int> stock, int[] quantity)
        => SearchNaive(CopyOf(stock), OrdersDescending(quantity), 0);

    private static bool SearchNaive(int[] remaining, int[] orders, int index)
    {
        if (index == orders.Length)
        {
            return true;
        }

        for (var value = 0; value < remaining.Length; value++)
        {
            if (TryAssign(remaining, orders, value, index))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryAssign(int[] remaining, int[] orders, int value, int index)
    {
        if (remaining[value] < orders[index])
        {
            return false;
        }

        remaining[value] -= orders[index];

        if (SearchNaive(remaining, orders, index + 1))
        {
            return true;
        }

        remaining[value] += orders[index];
        return false;
    }

    // This repo's own Backtrack.TrySearch (the NQueens/SudokuSolver/
    // PartitionToKEqualSumSubsets precedent), closed over the same choose/explore/
    // unchoose steps the naive arm writes out by hand.
    public static bool CanDistributeByGenericBacktrack(int[] nums, int[] quantity)
        => SearchGeneric(StockCountsOf(nums), OrdersDescending(quantity));

    // Section 17.4's hoisted overload, as above.
    public static bool CanDistributeByGenericBacktrack(DynamicArray<int> stock, int[] quantity)
        => SearchGeneric(CopyOf(stock), OrdersDescending(quantity));

    private static bool SearchGeneric(int[] stock, int[] orders)
    {
        var state = new OrderState(orders, stock);

        return Backtrack.TrySearch<OrderState, int>(state, new BacktrackingSteps<OrderState, int>(
            IsSolution: s => s.Index == orders.Length,
            Candidates: s => s.Index == orders.Length ? [] : Enumerable.Range(0, stock.Length).Where(s.CanPlace),
            Choose: (s, value) => s.Place(value),
            Unchoose: (s, value) => s.Remove(value),
            OnSolution: _ => true));
    }

    // Every distinct value is interchangeable with any other of the same count, so
    // nums is only ever needed as "how many copies of each value exist".
    private static int[] StockCountsOf(int[] nums)
        => nums
            .GroupBy(value => value)
            .Select(group => group.Count())
            .ToArray();

    private static int[] CopyOf(DynamicArray<int> stock)
    {
        var counts = new int[stock.Count];

        for (var index = 0; index < counts.Length; index++)
        {
            counts[index] = stock.Get(index);
        }

        return counts;
    }

    private static int[] OrdersDescending(int[] quantity)
    {
        var orders = (int[])quantity.Clone();
        Array.Sort(orders);
        Array.Reverse(orders);
        return orders;
    }

    // Takes ownership of remaining: every caller above hands it a freshly derived
    // stock array, so there is nothing left to defend against copying again.
    private sealed class OrderState(int[] orders, int[] remaining)
    {
        private readonly int[] _remaining = remaining;

        public int Index { get; private set; }

        public bool CanPlace(int value) => _remaining[value] >= orders[Index];

        public void Place(int value)
        {
            _remaining[value] -= orders[Index];
            Index++;
        }

        public void Remove(int value)
        {
            Index--;
            _remaining[value] += orders[Index];
        }
    }
}
