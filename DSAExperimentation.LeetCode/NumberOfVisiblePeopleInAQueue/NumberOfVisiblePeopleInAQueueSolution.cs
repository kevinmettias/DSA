using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.NumberOfVisiblePeopleInAQueue;

// LeetCode 1944. Number of Visible People in a Queue: person i can see person j to
// their right exactly when min(heights[i], heights[j]) is strictly greater than
// every height standing between them. Report, for each person, how many people they
// can see.
//
// The naive baseline reads that definition literally and rescans rightwards from
// every person - O(n^2) worst case. The composed strategy sweeps once from the right
// through a monotonic non-increasing Stack<int> of heights
// (DailyTemperatures/NextGreaterElementI precedent for this repo's own Stack): every
// shorter person still on top is popped and counted, one more is counted for the
// taller-or-equal blocker left on top afterwards, and heights tied with the current
// person are popped before it is pushed so a later, taller person can never see past
// more than one of a run of equal heights.
internal static class NumberOfVisiblePeopleInAQueueSolution
{
    // The textbook answer: for each person, walk right tracking the tallest person
    // seen so far between them and the candidate, per LC's own visibility rule.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int[] CountVisibleByBruteForceScan(int[] heights)
    {
        var result = new int[heights.Length];

        for (var i = 0; i < heights.Length; i++)
        {
            result[i] = CountVisibleFrom(heights, i);
        }

        return result;
    }

    private static int CountVisibleFrom(int[] heights, int person)
    {
        var maxBetween = 0;
        var visible = 0;

        for (var j = person + 1; j < heights.Length; j++)
        {
            if (maxBetween < heights[person] && maxBetween < heights[j])
            {
                visible++;
            }

            if (heights[j] >= heights[person])
            {
                break;
            }

            maxBetween = Math.Max(maxBetween, heights[j]);
        }

        return visible;
    }

    // One right-to-left sweep through a monotonic non-increasing Stack<int>: every
    // person is pushed once and popped at most once, so the whole sweep is O(n).
    public static int[] CountVisibleByMonotonicStackSweep(int[] heights)
    {
        var result = new int[heights.Length];
        var taller = new RepoIntStack();

        for (var i = heights.Length - 1; i >= 0; i--)
        {
            result[i] = CountVisibleAndAdvance(taller, heights[i]);
        }

        return result;
    }

    private static int CountVisibleAndAdvance(RepoIntStack taller, int height)
    {
        var count = 0;

        while (taller.TryPeek(out var top) && top < height)
        {
            taller.TryPop(out _);
            count++;
        }

        if (taller.Count > 0)
        {
            count++;
        }

        while (taller.TryPeek(out var top) && top == height)
        {
            taller.TryPop(out _);
        }

        taller.Push(height);
        return count;
    }
}
