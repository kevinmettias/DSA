using NextGreaterStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.NextGreaterElementIV;

// LeetCode 2454. Next Greater Element IV: for each index i, the SECOND value after
// it that is strictly greater than nums[i], or -1 when fewer than two such values
// exist.
//
// The baseline walks forward from each index counting greater values; the composed
// strategy is the two-monotonic-stack sweep over this repo's own Stack<int> - the
// NextGreaterElementII precedent generalized from "first greater" to "second
// greater".
internal static class NextGreaterElementIVSolution
{
    // "Second greater" is the second strictly-greater value seen walking forward.
    private const int RequiredGreaterCount = 2;

    // The textbook answer: from each index, scan forward counting values greater
    // than it and stop at the second one. O(n^2), written with nothing but the BCL
    // (section 17.5) - it is the arm the stack sweep below has to justify itself
    // against.
    public static int[] SecondGreaterElementByBruteForce(int[] nums)
    {
        var result = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            result[i] = SecondGreaterAfter(nums, i);
        }

        return result;
    }

    private static int SecondGreaterAfter(int[] nums, int index)
    {
        var greaterCount = 0;

        for (var j = index + 1; j < nums.Length; j++)
        {
            if (nums[j] <= nums[index])
            {
                continue;
            }

            greaterCount++;

            if (greaterCount == RequiredGreaterCount)
            {
                return nums[j];
            }
        }

        return LeetCodeAnswer.None;
    }

    // This repo's own O(n) sweep over two monotonic Stack<int>s. WaitingForFirst
    // holds indices still waiting for their FIRST greater value; WaitingForSecond
    // holds indices that already found their first and are waiting for their SECOND.
    // A value resolves WaitingForSecond's entries outright, but only PROMOTES
    // WaitingForFirst's - through the small Promoted stack, so that popping them off
    // one decreasing stack and pushing them onto the other preserves their relative
    // order and keeps WaitingForSecond decreasing too. The promotion has to happen
    // after this index's own resolutions, since a value can never be both an index's
    // first and its second greater element on the same pass.
    public static int[] SecondGreaterElementByTwoMonotonicStacks(int[] nums)
    {
        var result = new int[nums.Length];
        Array.Fill(result, LeetCodeAnswer.None);
        var waiting = new WaitingIndices(new NextGreaterStack(), new NextGreaterStack(), new NextGreaterStack());

        for (var i = 0; i < nums.Length; i++)
        {
            ResolveWaitingForSecond(nums, result, waiting, i);
            PromoteWaitingForFirst(nums, waiting, i);
            waiting.WaitingForFirst.Push(i);
        }

        return result;
    }

    private static void ResolveWaitingForSecond(int[] nums, int[] result, WaitingIndices waiting, int index)
    {
        while (waiting.WaitingForSecond.TryPeek(out var second) && nums[second] < nums[index])
        {
            waiting.WaitingForSecond.TryPop(out _);
            result[second] = nums[index];
        }
    }

    private static void PromoteWaitingForFirst(int[] nums, WaitingIndices waiting, int index)
    {
        while (waiting.WaitingForFirst.TryPeek(out var first) && nums[first] < nums[index])
        {
            waiting.WaitingForFirst.TryPop(out _);
            waiting.Promoted.Push(first);
        }

        while (waiting.Promoted.TryPop(out var promoted))
        {
            waiting.WaitingForSecond.Push(promoted);
        }
    }

    private readonly record struct WaitingIndices(
        NextGreaterStack WaitingForFirst,
        NextGreaterStack WaitingForSecond,
        NextGreaterStack Promoted);
}
