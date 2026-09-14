using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfWaysToReachAPositionAfterExactlyKSteps;

// LeetCode 2400. Number of Ways to Reach a Position After Exactly k Steps: count
// the step sequences of length k that move from startPos to endPos, one unit left
// or right per step, modulo 1e9+7.
//
// Only the distance between the two positions matters, so both strategies collapse
// the state to (remainingSteps, distanceToTarget) and recur:
//   ways(steps, diff) = ways(steps - 1, |diff - 1|) + ways(steps - 1, diff + 1)
// The absolute value is what makes the collapse sound: from an offset of 0 the left
// and right neighbours both land on |0 - 1| = |0 + 1| = 1, so the two branches
// double-count that state exactly the way the two physical step choices do. The
// diff > steps early exit is pruning, not a correctness check - ways(0, diff)
// already reports 0 for any non-zero diff.
//
// They differ only in whether that state is remembered. The baseline re-derives it
// on every left/right ordering that reaches it, which is 2^k work; the composed
// strategy routes the same recurrence through this repo's own Memoizer keyed on the
// (Steps, Diff) tuple - the 2-D value-tuple state shape TargetSumSolution's
// (Index, Sum) already establishes - so each state is solved once.
internal static class NumberOfWaysToReachAPositionAfterExactlyKStepsSolution
{
    // The textbook answer: plain recursion, no cache, BCL arithmetic only. This is
    // the arm the memoized strategy has to justify itself against, and k must stay
    // modest because the recursion really is 2^k.
    public static int NumberOfWaysByUnmemoizedRecursion(int startPos, int endPos, int k)
    {
        var diff = Math.Abs(endPos - startPos);

        return (int)WaysWithoutCache(k, diff);
    }

    private static long WaysWithoutCache(int steps, int diff)
    {
        if (diff > steps)
        {
            return 0;
        }

        if (steps == 0)
        {
            return diff == 0 ? 1 : 0;
        }

        var towardTarget = WaysWithoutCache(steps - 1, Math.Abs(diff - 1));
        var awayFromTarget = WaysWithoutCache(steps - 1, diff + 1);

        return (towardTarget + awayFromTarget) % ModularArithmetic.Modulo;
    }

    // Same recurrence driven top-down through Memoizer, so each (steps, diff) state
    // is solved once and shared by every step ordering that reaches it.
    public static int NumberOfWaysByMemoizedRecursion(int startPos, int endPos, int k)
    {
        var diff = Math.Abs(endPos - startPos);

        return (int)Memoizer.Memoize<(int Steps, int Diff), long>((k, diff), WaysFrom);
    }

    private static long WaysFrom((int Steps, int Diff) state, Func<(int Steps, int Diff), long> ways)
    {
        var (steps, diff) = state;

        if (diff > steps)
        {
            return 0;
        }

        if (steps == 0)
        {
            return diff == 0 ? 1 : 0;
        }

        var towardTarget = ways((steps - 1, Math.Abs(diff - 1)));
        var awayFromTarget = ways((steps - 1, diff + 1));

        return (towardTarget + awayFromTarget) % ModularArithmetic.Modulo;
    }
}
