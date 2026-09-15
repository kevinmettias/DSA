using CompetitiveStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.FindTheMostCompetitiveSubsequence;

// LeetCode 1673. Find the Most Competitive Subsequence: the lexicographically
// smallest subsequence of nums of length k.
//
// Both strategies apply the identical greedy rule - drop an element whenever a
// strictly smaller one follows it and enough elements remain to still reach
// length k - and differ only in how many passes that takes: one removal per O(n)
// scan, or every removal within a single sweep over a monotonic stack.
internal static class FindTheMostCompetitiveSubsequenceSolution
{
    // The textbook baseline this composition has to justify itself against:
    // round by round, find the first element that a smaller one follows and drop
    // it, falling back to the last element when the sequence is already
    // non-decreasing. Deliberately a plain BCL List<int> - it is the arm the
    // composed solution below has to beat, and its (n - k) separate scans are
    // exactly what makes it O(n^2).
    public static int[] MostCompetitiveByRepeatedRemoval(int[] nums, int k)
    {
        var current = new List<int>(nums);

        while (current.Count > k)
        {
            current.RemoveAt(FirstDescentIndex(current));
        }

        return current.ToArray();
    }

    // The first position whose successor is smaller, or the last position when
    // there is none - removing the tail is the only way to shorten an already
    // non-decreasing sequence without making it larger.
    private static int FirstDescentIndex(List<int> current)
    {
        for (var i = 0; i < current.Count - 1; i++)
        {
            if (current[i] > current[i + 1])
            {
                return i;
            }
        }

        return current.Count - 1;
    }

    // The composed answer: one sweep over this repo's own Stack<int>, the same
    // monotonic-stack greedy RemoveKDigits applies. Every element is pushed and
    // popped at most once, so the whole removal schedule the baseline rediscovers
    // (n - k) times is settled in a single O(n) pass.
    public static int[] MostCompetitiveByMonotonicStack(int[] nums, int k)
    {
        var stack = new CompetitiveStack();

        BuildCompetitiveStack(stack, nums, k);

        return DrainStackToArray(stack);
    }

    // Pop any still-poppable, strictly greater element as long as enough elements
    // remain afterward to still reach length k, then push the current element only
    // while there is still room left for it.
    private static void BuildCompetitiveStack(CompetitiveStack stack, int[] nums, int k)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            while (CanAffordToDropTop(stack, nums[i], nums.Length - i, k))
            {
                stack.TryPop(out _);
            }

            if (stack.Count < k)
            {
                stack.Push(nums[i]);
            }
        }
    }

    // The stack is allowed to give up its top: something is there, it is strictly
    // greater than the element arriving, and enough elements remain after the drop
    // to still reach length k.
    private static bool CanAffordToDropTop(
        CompetitiveStack stack, int incoming, int remaining, int k)
        => stack.Count > 0
            && stack.TryPeek(out var top)
            && top > incoming
            && stack.Count - 1 + remaining >= k;

    // The stack holds the answer in reverse, so it is filled back to front.
    private static int[] DrainStackToArray(CompetitiveStack stack)
    {
        var result = new int[stack.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return result;
    }
}
