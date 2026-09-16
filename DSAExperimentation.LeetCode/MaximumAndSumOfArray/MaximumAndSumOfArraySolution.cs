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

    // This repo's own Memoizer, keyed on the (Slot, UsedMask) tuple - the same
    // 2D memo state MaximumStudentsTakingExamSolution's (Row, PrevMask) uses, with
    // "row" renamed to "slot". Every reachable state is computed exactly once.
    public static int MaximumAndSumByMemoizedBitmask(int[] nums, int numSlots)
    {
        var context = new SlotContext(nums, numSlots);

        return Memoizer.Memoize<(int Slot, int UsedMask), int>(
            (FirstSlot, 0), new BestSlotPlacement(context));
    }

    private static int BestFrom(
        (int Slot, int UsedMask) state,
        IRecurrence<(int Slot, int UsedMask), int> rest,
        SlotContext context)
    {
        var (slot, usedMask) = state;

        if (slot > context.NumSlots)
        {
            return 0;
        }

        var best = rest.Replay((slot + 1, usedMask), rest);
        var frame = new SlotFrame(context, slot, usedMask);

        for (var elementIndex = 0; elementIndex < context.Nums.Length; elementIndex++)
        {
            best = BestConsideringElement(frame, elementIndex, best, rest);
        }

        return best;
    }

    private static int BestConsideringElement(
        SlotFrame frame, int elementIndex, int best, IRecurrence<(int Slot, int UsedMask), int> rest)
    {
        var elementBit = 1 << elementIndex;

        if ((frame.UsedMask & elementBit) != 0)
        {
            return best;
        }

        var withOne = (frame.Slot & frame.Context.Nums[elementIndex])
            + rest.Replay((frame.Slot + 1, frame.UsedMask | elementBit), rest);
        best = Math.Max(best, withOne);

        return BestConsideringPair(frame, elementIndex, best, rest);
    }

    private static int BestConsideringPair(
        SlotFrame frame, int elementIndex, int best, IRecurrence<(int Slot, int UsedMask), int> rest)
    {
        var nums = frame.Context.Nums;
        var elementBit = 1 << elementIndex;

        for (var otherIndex = elementIndex + 1; otherIndex < nums.Length; otherIndex++)
        {
            var otherElementBit = 1 << otherIndex;

            if ((frame.UsedMask & otherElementBit) != 0)
            {
                continue;
            }

            var withTwo = (frame.Slot & nums[elementIndex]) + (frame.Slot & nums[otherIndex])
                + rest.Replay((frame.Slot + 1, frame.UsedMask | elementBit | otherElementBit), rest);
            best = Math.Max(best, withTwo);
        }

        return best;
    }

    private static int BestFromBruteForce(SlotContext context, int slot, int usedMask)
    {
        if (slot > context.NumSlots)
        {
            return 0;
        }

        // Leaving this slot empty is always a candidate - a slot never has to be filled.
        var best = BestFromBruteForce(context, slot + 1, usedMask);
        var frame = new SlotFrame(context, slot, usedMask);

        for (var elementIndex = 0; elementIndex < context.Nums.Length; elementIndex++)
        {
            best = BestConsideringElementBruteForce(frame, elementIndex, best);
        }

        return best;
    }

    private static int BestConsideringElementBruteForce(SlotFrame frame, int elementIndex, int best)
    {
        var elementBit = 1 << elementIndex;

        if ((frame.UsedMask & elementBit) != 0)
        {
            return best;
        }

        var withOne = (frame.Slot & frame.Context.Nums[elementIndex])
            + BestFromBruteForce(frame.Context, frame.Slot + 1, frame.UsedMask | elementBit);
        best = Math.Max(best, withOne);

        return BestConsideringPairBruteForce(frame, elementIndex, best);
    }

    private static int BestConsideringPairBruteForce(SlotFrame frame, int elementIndex, int best)
    {
        var nums = frame.Context.Nums;
        var elementBit = 1 << elementIndex;

        for (var otherIndex = elementIndex + 1; otherIndex < nums.Length; otherIndex++)
        {
            var otherElementBit = 1 << otherIndex;

            if ((frame.UsedMask & otherElementBit) != 0)
            {
                continue;
            }

            var withTwo = (frame.Slot & nums[elementIndex]) + (frame.Slot & nums[otherIndex])
                + BestFromBruteForce(frame.Context, frame.Slot + 1, frame.UsedMask | elementBit | otherElementBit);
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

    // The placement rule, named: from a (slot, usedMask) state the best total is whichever
    // of leave-this-slot-empty, seat one unused element or seat two unused elements scores
    // highest once the rule's own answer for the slot that choice advances to is counted.
    // The problem's fixed inputs arrive once through the primary constructor; `rest` is the
    // memo run's own handle on this rule, so a recursion is a call on a named type rather
    // than on an anonymous call-back value.
    private sealed class BestSlotPlacement(SlotContext context)
        : IRecurrence<(int Slot, int UsedMask), int>
    {
        /// <inheritdoc/>
        public int Replay(
            (int Slot, int UsedMask) state, IRecurrence<(int Slot, int UsedMask), int> rest)
            => BestFrom(state, rest, context);
    }
}
