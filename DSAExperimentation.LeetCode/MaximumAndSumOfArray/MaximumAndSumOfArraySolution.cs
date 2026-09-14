using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximumAndSumOfArray;

// LeetCode 2172. Maximum AND Sum of Array: place every element of nums into one of
// numSlots numbered slots, at most two elements per slot, maximizing the sum of
// (slot number AND element).
//
// The state is (which slot is being filled, which elements of nums are already
// placed), so the second component is a bitmask over nums. Each slot independently
// takes 0, 1 or 2 of the still-unused elements before the walk advances - a slot is
// never forced to take elements just because it comes first, since ANDing with a
// larger slot number is not always worse than ANDing with a smaller one.
//
// Both strategies run that identical recursion; they differ only in whether a
// (slot, usedMask) state reached by several different placement orders is
// recomputed each time or once - the same split MaximumStudentsTakingExamSolution
// makes over its own (row, prevMask) bitmask state.
internal static class MaximumAndSumOfArraySolution
{
    // Slots are numbered from 1, and it is the slot NUMBER that is ANDed with the
    // element, so the walk starts at slot 1 with nothing placed.
    private const int FirstSlot = 1;

    // The textbook baseline: the same recursion with no memoization at all, so the
    // identical (slot, usedMask) subtree is re-explored once per distinct order the
    // earlier slots happened to consume elements in. Deliberately plain recursion
    // over a BCL array - the arm the memoized strategy below has to justify itself
    // against.
    public static int MaximumAndSumByBruteForceRecursion(int[] nums, int numSlots) =>
        BestFromBruteForce(new SlotContext(nums, numSlots), FirstSlot, usedMask: 0);

    private static int BestFromBruteForce(SlotContext context, int slot, int usedMask)
    {
        if (slot > context.NumSlots)
        {
            return 0;
        }

        // Leaving this slot empty is always a candidate - a slot never has to be filled.
        var best = BestFromBruteForce(context, slot + 1, usedMask);
        var frame = new SlotFrame(context, slot, usedMask);

        for (var i = 0; i < context.Nums.Length; i++)
        {
            best = BestConsideringElementBruteForce(frame, i, best);
        }

        return best;
    }

    private static int BestConsideringElementBruteForce(SlotFrame frame, int i, int best)
    {
        var bitI = 1 << i;

        if ((frame.UsedMask & bitI) != 0)
        {
            return best;
        }

        var withOne = (frame.Slot & frame.Context.Nums[i])
            + BestFromBruteForce(frame.Context, frame.Slot + 1, frame.UsedMask | bitI);
        best = Math.Max(best, withOne);

        return BestConsideringPairBruteForce(frame, i, best);
    }

    private static int BestConsideringPairBruteForce(SlotFrame frame, int i, int best)
    {
        var nums = frame.Context.Nums;
        var bitI = 1 << i;

        for (var j = i + 1; j < nums.Length; j++)
        {
            var bitJ = 1 << j;

            if ((frame.UsedMask & bitJ) != 0)
            {
                continue;
            }

            var withTwo = (frame.Slot & nums[i]) + (frame.Slot & nums[j])
                + BestFromBruteForce(frame.Context, frame.Slot + 1, frame.UsedMask | bitI | bitJ);
            best = Math.Max(best, withTwo);
        }

        return best;
    }

    // This repo's own Memoizer, keyed on the (Slot, UsedMask) tuple - the same
    // 2D memo state MaximumStudentsTakingExamSolution's (Row, PrevMask) uses, with
    // "row" renamed to "slot". Every reachable state is computed exactly once.
    public static int MaximumAndSumByMemoizedBitmask(int[] nums, int numSlots)
    {
        var context = new SlotContext(nums, numSlots);

        return Memoizer.Memoize<(int Slot, int UsedMask), int>(
            (FirstSlot, 0), (state, bestFrom) => BestFrom(state, bestFrom, context));
    }

    private static int BestFrom(
        (int Slot, int UsedMask) state, Func<(int Slot, int UsedMask), int> bestFrom, SlotContext context)
    {
        var (slot, usedMask) = state;

        if (slot > context.NumSlots)
        {
            return 0;
        }

        var best = bestFrom((slot + 1, usedMask));
        var frame = new SlotFrame(context, slot, usedMask);

        for (var i = 0; i < context.Nums.Length; i++)
        {
            best = BestConsideringElement(frame, i, best, bestFrom);
        }

        return best;
    }

    private static int BestConsideringElement(
        SlotFrame frame, int i, int best, Func<(int Slot, int UsedMask), int> bestFrom)
    {
        var bitI = 1 << i;

        if ((frame.UsedMask & bitI) != 0)
        {
            return best;
        }

        var withOne = (frame.Slot & frame.Context.Nums[i]) + bestFrom((frame.Slot + 1, frame.UsedMask | bitI));
        best = Math.Max(best, withOne);

        return BestConsideringPair(frame, i, best, bestFrom);
    }

    private static int BestConsideringPair(
        SlotFrame frame, int i, int best, Func<(int Slot, int UsedMask), int> bestFrom)
    {
        var nums = frame.Context.Nums;
        var bitI = 1 << i;

        for (var j = i + 1; j < nums.Length; j++)
        {
            var bitJ = 1 << j;

            if ((frame.UsedMask & bitJ) != 0)
            {
                continue;
            }

            var withTwo = (frame.Slot & nums[i]) + (frame.Slot & nums[j])
                + bestFrom((frame.Slot + 1, frame.UsedMask | bitI | bitJ));
            best = Math.Max(best, withTwo);
        }

        return best;
    }

    // The problem's fixed inputs, carried through the recursion as one value so the
    // helpers stay within this repo's parameter budget.
    private readonly record struct SlotContext(int[] Nums, int NumSlots);

    // One recursion frame: the fixed inputs plus the (slot, usedMask) state the
    // per-element and per-pair helpers branch from.
    private readonly record struct SlotFrame(SlotContext Context, int Slot, int UsedMask);
}
