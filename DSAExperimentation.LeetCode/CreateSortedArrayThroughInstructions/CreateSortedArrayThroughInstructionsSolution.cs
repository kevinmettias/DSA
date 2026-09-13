using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CreateSortedArrayThroughInstructions;

// LeetCode 1649. Create Sorted Array through Instructions: insert each instruction
// into a container that stays sorted, left to right. An insertion costs the smaller
// of "how many already-inserted elements are strictly less than it" and "how many
// are strictly greater"; report the total cost modulo 1e9+7.
//
// Both strategies answer the same two counting questions at every step and differ
// only in what they ask - a rescan of everything inserted so far, or one
// value-indexed Fenwick tree carried across the whole sweep.
internal static class CreateSortedArrayThroughInstructionsSolution
{
    // The textbook answer: keep every inserted value in a list and re-count the
    // strictly-smaller and strictly-greater ones from scratch for each instruction.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int CreateSortedArrayByPairwiseScan(int[] instructions)
    {
        var inserted = new List<int>(instructions.Length);
        long cost = 0;

        foreach (var value in instructions)
        {
            cost += CheaperSideOf(inserted, value);
            inserted.Add(value);
        }

        return (int)(cost % ModularArithmetic.Modulo);
    }

    private static int CheaperSideOf(List<int> inserted, int value)
    {
        var less = 0;
        var greater = 0;

        foreach (var existing in inserted)
        {
            if (existing < value)
            {
                less++;
            }
            else if (existing > value)
            {
                greater++;
            }
        }

        return Math.Min(less, greater);
    }

    // This repo's own FenwickTree<int, SumOperation<int>> - a Binary Indexed Tree of
    // counts indexed by value rather than by position, the same sweep
    // CountOfSmallerNumbersAfterSelf runs for LeetCode 315, here left to right
    // instead of right to left. PrefixQuery(value - 1) is every strictly-smaller
    // value inserted so far; the running insertion count minus PrefixQuery(value) is
    // every strictly-greater one. O(n log maxValue) instead of O(n^2).
    public static int CreateSortedArrayByFenwickTreeSweep(int[] instructions)
    {
        var maxValue = instructions.Max();
        var tree = new FenwickTree<int, SumOperation<int>>(maxValue + 1);
        long cost = 0;

        for (var i = 0; i < instructions.Length; i++)
        {
            var value = instructions[i];
            var lessCount = value == 0 ? 0 : tree.PrefixQuery(value - 1);
            var greaterCount = i - tree.PrefixQuery(value);
            cost += Math.Min(lessCount, greaterCount);
            tree.Add(value, 1);
        }

        return (int)(cost % ModularArithmetic.Modulo);
    }
}
