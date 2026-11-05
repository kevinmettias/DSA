namespace DSAExperimentation.LeetCode.Solutions.SubarrayWithElementsGreaterThanVaryingThreshold;

// LC 2334 accepts the size of any subarray whose every element is greater than threshold / size, and -1
// only when there is none. The catalog records one size, so an answer stands when it is -1 exactly where
// the recorded one is, or else the size of a subarray that qualifies. The window scan answers the first
// size it finds and the union-find order the size of the first group that qualifies.
internal static class SubarrayWithElementsGreaterThanVaryingThresholdAnswerCheck
{
    private const int NoSubarray = -1;

    public static bool IsAccepted(int[] nums, int threshold, int answer, int recorded)
    {
        if (answer == NoSubarray || recorded == NoSubarray)
        {
            return answer == recorded;
        }

        return answer >= 1 && answer <= nums.Length && HasQualifyingRun(nums, threshold, answer);
    }

    // Some answer-long run of elements each greater than threshold / answer, compared as element * answer >
    // threshold so no division rounds.
    private static bool HasQualifyingRun(int[] nums, int threshold, int answer)
    {
        var run = 0;

        foreach (var number in nums)
        {
            if ((long)number * answer > threshold)
            {
                run++;
            }
            else
            {
                run = 0;
            }

            if (run >= answer)
            {
                return true;
            }
        }

        return false;
    }
}
