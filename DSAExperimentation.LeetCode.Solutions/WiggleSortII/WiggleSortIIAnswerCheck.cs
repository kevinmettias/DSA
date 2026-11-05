namespace DSAExperimentation.LeetCode.Solutions.WiggleSortII;

// LC 324 accepts any reordering of the array in which each element is smaller than the next at even
// positions and larger at odd ones (nums[0] < nums[1] > nums[2] < nums[3] ...). So an answer stands when
// it holds the array's values and alternates so; the recorded answer is one of these, so it does not take
// part. The selection sort and the merge sort can place equal values differently.
internal static class WiggleSortIIAnswerCheck
{
    public static bool IsAccepted(int[] nums, int[] answer, int[] recorded)
        => answer.Order().SequenceEqual(nums.Order()) && answer.Skip(1).Select((_, step) => step).All(step => IsWiggleStep(answer, step + 1));

    // From an even position the next value is larger; from an odd one, smaller.
    private static bool IsWiggleStep(int[] values, int index)
    {
        var fromAnEvenPosition = index % 2 == 1;
        var rises = values[index - 1] < values[index];
        var falls = values[index - 1] > values[index];
        return fromAnEvenPosition ? rises : falls;
    }
}
