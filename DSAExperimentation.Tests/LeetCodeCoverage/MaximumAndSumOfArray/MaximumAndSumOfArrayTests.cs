using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumAndSumOfArray;

// LeetCode 2172. Maximum AND Sum of Array: bitmask DP over (which slot is being
// filled, which elements of nums are already placed), via this repo's own Memoizer -
// the same (Row, PrevMask)-shaped tuple state MaximumStudentsTakingExamTests already
// establishes, just with "row" renamed to "slot." Each slot independently gets 0, 1,
// or 2 of the still-unused elements before the walk advances to the next slot - a
// slot is never forced to take elements just because it comes first, since AND with
// a bigger slot index isn't always worse than AND with a smaller one.
public sealed class MaximumAndSumOfArrayTests
{
    [Fact]
    public void MaximumAndSum_LeetCodeExampleOne_ReturnsNine()
    {
        int[] nums = [1, 2, 3, 4, 5, 6];

        var actual = MaximumAndSum(nums, numSlots: 3);

        Assert.Equal(9, actual);
    }

    [Fact]
    public void MaximumAndSum_LeetCodeExampleTwo_ReturnsTwentyFour()
    {
        int[] nums = [1, 3, 10, 4, 7, 1];

        var actual = MaximumAndSum(nums, numSlots: 9);

        Assert.Equal(24, actual);
    }

    [Fact]
    public void MaximumAndSum_SingleElementPrefersTheBetterAndingSlot()
    {
        int[] nums = [3];

        // 3 & 1 = 1 but 3 & 2 = 2 - picking slot 1 just because it's first would be
        // wrong; the DP must consider leaving slot 1 empty in favor of slot 2.
        var actual = MaximumAndSum(nums, numSlots: 2);

        Assert.Equal(2, actual);
    }

    private static int MaximumAndSum(int[] nums, int numSlots)
    {
        var context = new SlotContext(nums, numSlots);

        return Memoizer.Memoize<(int Slot, int UsedMask), int>(
            (1, 0), (state, bestFrom) => BestFrom(state, bestFrom, context));
    }

    private readonly record struct SlotContext(int[] Nums, int NumSlots);

    private readonly record struct SlotFrame((int Slot, int UsedMask) State, SlotContext Context);

    private static int BestFrom(
        (int Slot, int UsedMask) state, Func<(int Slot, int UsedMask), int> bestFrom, SlotContext context)
    {
        var (slot, usedMask) = state;
        if (slot > context.NumSlots)
        {
            return 0;
        }

        // Leaving this slot empty is always a candidate - a slot never has to be filled.
        var best = bestFrom((slot + 1, usedMask));

        var frame = new SlotFrame(state, context);
        for (var i = 0; i < context.Nums.Length; i++)
        {
            best = BestConsideringSlot(frame, i, best, bestFrom);
        }

        return best;
    }

    private static int BestConsideringSlot(
        SlotFrame frame, int i, int best, Func<(int Slot, int UsedMask), int> bestFrom)
    {
        var (slot, usedMask) = frame.State;
        var nums = frame.Context.Nums;

        var bitI = 1 << i;
        if ((usedMask & bitI) != 0)
        {
            return best;
        }

        var withOne = (slot & nums[i]) + bestFrom((slot + 1, usedMask | bitI));
        best = Math.Max(best, withOne);

        return BestConsideringPair(frame, i, best, bestFrom);
    }

    private static int BestConsideringPair(
        SlotFrame frame, int i, int best, Func<(int Slot, int UsedMask), int> bestFrom)
    {
        var (slot, usedMask) = frame.State;
        var nums = frame.Context.Nums;
        var bitI = 1 << i;

        for (var j = i + 1; j < nums.Length; j++)
        {
            var bitJ = 1 << j;
            if ((usedMask & bitJ) != 0)
            {
                continue;
            }

            var withTwo = (slot & nums[i]) + (slot & nums[j])
                + bestFrom((slot + 1, usedMask | bitI | bitJ));
            best = Math.Max(best, withTwo);
        }

        return best;
    }
}
