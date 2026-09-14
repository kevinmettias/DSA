using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<(int Value, int Step)>;

namespace DSAExperimentation.LeetCode.StepsToMakeArrayNonDecreasing;

// LeetCode 2289. Steps to Make Array Non-decreasing: each step simultaneously
// removes every element that has a strictly greater element immediately to its
// left; the answer is how many steps run before the array is non-decreasing.
//
// The naive baseline literally replays those rounds - O(n) work per round and up
// to O(n) rounds, so O(n^2) on a sawtooth. The composed strategy sweeps once,
// keeping a monotonic non-increasing Stack<(Value, Step)> of elements still
// pending: an incoming value pops every no-greater element still on top, and the
// step at which it will itself be removed is one past the latest step it just
// swallowed (DailyTemperaturesSolution's monotonic-stack precedent over this
// repo's own Stack, generalized from "wait days" to "removal step"). The answer
// is the largest removal step recorded.
internal static class StepsToMakeArrayNonDecreasingSolution
{
    // The textbook answer: rebuild the array round by round until a round removes
    // nothing. Deliberately written with BCL types only - it is the arm the
    // composed sweep below has to justify itself against.
    public static int TotalStepsBySimulatingRounds(int[] nums)
    {
        var current = nums;
        var steps = 0;

        while (true)
        {
            var (next, anyRemoved) = RunOneRound(current);

            if (!anyRemoved)
            {
                return steps;
            }

            current = next;
            steps++;
        }
    }

    // One simultaneous removal round: every index whose left neighbor (in the
    // pre-round array) is strictly greater gets dropped, in one linear pass.
    private static (int[] Next, bool AnyRemoved) RunOneRound(int[] current)
    {
        var next = new List<int>(current.Length) { current[0] };
        var anyRemoved = false;

        for (var i = 1; i < current.Length; i++)
        {
            if (current[i - 1] > current[i])
            {
                anyRemoved = true;
            }
            else
            {
                next.Add(current[i]);
            }
        }

        return (next.ToArray(), anyRemoved);
    }

    // This repo's own Stack as the monotonic scratch structure: one pass, each
    // element pushed once and popped at most once.
    public static int TotalStepsByMonotonicStackSweep(int[] nums)
    {
        var stack = new RepoStack();
        var maxSteps = 0;

        foreach (var value in nums)
        {
            var step = 0;

            while (stack.TryPeek(out var top) && top.Value <= value)
            {
                step = Math.Max(step, top.Step);
                stack.TryPop(out _);
            }

            // Nothing left below means this value has no strictly greater element
            // to its left at all, so it is never removed.
            step = stack.Count == 0 ? 0 : step + 1;
            maxSteps = Math.Max(maxSteps, step);
            stack.Push((value, step));
        }

        return maxSteps;
    }
}
