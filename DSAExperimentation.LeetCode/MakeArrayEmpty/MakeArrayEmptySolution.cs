using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.MakeArrayEmpty;

// LeetCode 2659. Make Array Empty: repeatedly look at the front element - remove it
// if it is the smallest value still present, otherwise move it to the back - and
// count the operations until the array is empty.
//
// Simulating that directly is quadratic in the number of rotations, so both
// strategies reframe it as a circular sweep over the ORIGINAL indices. Values are
// distinct, so the removal order is exactly ascending value order; for each removal
// the operations spent since the previous one are "how many still-present slots lie
// strictly after the previously removed index, up to and including this one,
// wrapping past the end" - one "move to back" per still-present slot passed over,
// plus the removal itself.
//
// The strategies differ only in how that present-count is answered: an O(n) scan of
// a bool array, or an O(log n) range query on this repo's own
// FenwickTree<int, SumOperation<int>> (Binary Indexed Tree) holding 1 for present
// and 0 for removed - the same coordinate-free range-count contrast
// CountGoodTripletsInAnArraySolution draws for LC 2179.
internal static class MakeArrayEmptySolution
{
    // The textbook answer: re-scan the present flags between the previous removal
    // and this one on every step. Deliberately written with nothing but BCL arrays
    // - it is the O(n^2) arm the Fenwick sweep below has to justify itself against,
    // and running the identical circular sweep makes it a correctness cross-check
    // rather than a second algorithm.
    public static long CountOperationsByLinearScan(int[] nums)
    {
        var n = nums.Length;
        var order = RemovalOrder(nums);
        var present = new bool[n];
        Array.Fill(present, true);

        long total = 0;
        var previous = -1;

        foreach (var index in order)
        {
            total += index > previous
                ? CountPresent(present, previous + 1, index)
                : CountPresentWrapping(present, previous + 1, index);

            present[index] = false;
            previous = index;
        }

        return total;
    }

    // The removal sat at or before the previous one, so the slots passed over run
    // off the end of the array and resume at 0.
    private static int CountPresentWrapping(bool[] present, int left, int right)
        => CountPresent(present, left, present.Length - 1) + CountPresent(present, 0, right);

    // The same circular sweep, with each range's present-count answered by a Binary
    // Indexed Tree over a 0/1 "still present" array: removing an index is a single
    // point update of -1, so the whole sweep is O(n log n).
    public static long CountOperationsByFenwickTree(int[] nums)
    {
        var n = nums.Length;
        var order = RemovalOrder(nums);
        var present = new int[n];
        Array.Fill(present, 1);
        var tree = new FenwickTree<int, SumOperation<int>>(present);

        long total = 0;
        var previous = -1;

        foreach (var index in order)
        {
            total += index > previous
                ? PresentCount(tree, previous + 1, index)
                : PresentCountWrapping(tree, previous + 1, index);

            tree.Add(index, -1);
            previous = index;
        }

        return total;
    }

    // The wrap-around counterpart of CountPresentWrapping, over the tree instead of
    // the flag array.
    private static int PresentCountWrapping(FenwickTree<int, SumOperation<int>> tree, int left, int right)
        => PresentCount(tree, left, tree.Count - 1) + PresentCount(tree, 0, right);

    private static int CountPresent(bool[] present, int left, int right)
    {
        var count = 0;

        for (var i = left; i <= right; i++)
        {
            if (present[i])
            {
                count++;
            }
        }

        return count;
    }

    private static int PresentCount(FenwickTree<int, SumOperation<int>> tree, int left, int right)
        => left > right ? 0 : tree.Query(left, right);

    // The original indices in the order their values actually get removed: ascending
    // by value, which the problem's distinctness guarantee makes unambiguous.
    private static int[] RemovalOrder(int[] nums)
        => [.. Enumerable.Range(0, nums.Length).OrderBy(index => nums[index])];
}
