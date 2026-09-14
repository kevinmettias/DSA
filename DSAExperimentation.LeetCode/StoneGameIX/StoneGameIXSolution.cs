using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.StoneGameIX;

// LeetCode 2029. Stone Game IX: Alice and Bob alternate removing one stone; whoever
// makes the running sum of removed values a multiple of 3 loses immediately, and if
// every stone goes without that happening Bob wins. Return whether Alice, moving
// first, can force a win.
//
// Only a stone's value mod 3 matters, so both strategies start from the three
// remainder-bucket counts and differ in what they do with them:
//
//   - a full negamax over the remaining (count0, count1, count2, runningSumMod3)
//     state - O(n^3) states, so it needs a cache to be feasible at all and gets one
//     from this repo's own Memoizer<TState,TResult>; and
//   - the closed-form parity rule, read straight off the bucket counts in O(n).
//
// Both explore the identical rule set; the game-tree arm just re-derives by exhaustive
// search the fact the closed form states outright - a remainder-0 stone only ever flips
// whose turn effectively "counts," and only remainder-1/remainder-2 stones can push the
// running sum onto a multiple of 3.
internal static class StoneGameIXSolution
{
    private const int RemainderBucketCount = 3;
    private const int RemainderTwoIndex = 2;
    private const int EvenCountDivisor = 2;
    private const int MinimumBucketOccupancy = 1;
    private const int MinimumImbalanceThreshold = 3;

    // Negamax outcomes, always from the perspective of the player about to move.
    private const int MoverWins = 1;
    private const int MoverLoses = -1;
    private const int NoStonesLeft = 0;

    // The exhaustive answer: search every legal play order, memoized on the state that
    // actually distinguishes one position from another. This is the arm the closed form
    // has to justify itself against - and, before this migration, the benchmark baseline
    // nothing ever asserted.
    public static bool AliceWinsByGameTreeMinimax(int[] stones)
    {
        var counts = CountByRemainder(stones);
        var outcome = Memoizer.Memoize<(int C0, int C1, int C2, int TurnSum), int>(
            (counts[0], counts[1], counts[RemainderTwoIndex], 0),
            SolveState);

        return outcome == MoverWins;
    }

    private static int[] CountByRemainder(int[] stones)
    {
        var counts = new int[RemainderBucketCount];

        foreach (var stone in stones)
        {
            counts[stone % RemainderBucketCount]++;
        }

        return counts;
    }

    // Running out of stones without anyone busting is a loss for the mover, who is the
    // player that would have had to move next - which is exactly LeetCode's "Bob wins if
    // all the stones are removed" clause seen from the other side.
    private static int SolveState(
        (int C0, int C1, int C2, int TurnSum) state, Func<(int, int, int, int), int> solve)
    {
        var (c0, c1, c2, _) = state;

        if (c0 + c1 + c2 == 0)
        {
            return NoStonesLeft;
        }

        return BestOverRemainders(state, solve);
    }

    private static int BestOverRemainders(
        (int C0, int C1, int C2, int TurnSum) state, Func<(int, int, int, int), int> solve)
    {
        var (c0, c1, c2, _) = state;
        var counts = new[] { c0, c1, c2 };
        var best = int.MinValue;

        for (var r = 0; r < RemainderBucketCount; r++)
        {
            if (counts[r] == 0)
            {
                continue;
            }

            var nextState = NextState(state, r);
            var branchValue = Branch(r, nextState, solve);
            best = Math.Max(best, branchValue);
        }

        return best;
    }

    private static GameState NextState((int C0, int C1, int C2, int TurnSum) state, int r)
    {
        var (c0, c1, c2, turnSum) = state;

        return r switch
        {
            0 => new GameState(c0 - 1, c1, c2, turnSum),
            1 => new GameState(c0, c1 - 1, c2, turnSum),
            _ => new GameState(c0, c1, c2 - 1, turnSum),
        };
    }

    // Negamax step: a move of remainder r updates the running sum; landing on a multiple
    // of 3 loses immediately for the mover, otherwise the outcome is whatever the
    // opponent's own best play yields, negated back to this mover's perspective.
    private static int Branch(int r, GameState state, Func<(int, int, int, int), int> solve)
    {
        var newSum = (state.TurnSum + r) % RemainderBucketCount;

        return newSum == 0 ? MoverLoses : -solve((state.C0, state.C1, state.C2, newSum));
    }

    // Bucket the stones by value mod 3 with this repo's own HashMap<int,int>, then read
    // the answer off the known parity rule over the three counts.
    public static bool AliceWinsByClosedFormCounting(int[] stones)
    {
        var counts = new HashMap<int, int>();

        foreach (var stone in stones)
        {
            counts.TryGetValue(stone % RemainderBucketCount, out var c);
            counts.Set(stone % RemainderBucketCount, c + 1);
        }

        counts.TryGetValue(0, out var cnt0);
        counts.TryGetValue(1, out var cnt1);
        counts.TryGetValue(RemainderTwoIndex, out var cnt2);

        var remainderZeroCountIsEven = cnt0 % EvenCountDivisor == 0;

        return remainderZeroCountIsEven
            ? HasBothNonZeroRemainders(cnt1, cnt2)
            : HasWinningImbalance(cnt1, cnt2);
    }

    // With an even number of remainder-0 stones they cancel out of the turn parity
    // entirely, so Alice only needs one stone of each non-zero remainder to open with
    // whichever of them forces Bob into the busting position.
    private static bool HasBothNonZeroRemainders(int cnt1, int cnt2)
        => cnt1 >= MinimumBucketOccupancy && cnt2 >= MinimumBucketOccupancy;

    // An odd number of remainder-0 stones hands the parity to Bob, and the only way
    // back is an alternating run long enough to exhaust one bucket first.
    private static bool HasWinningImbalance(int cnt1, int cnt2)
        => Math.Abs(cnt1 - cnt2) >= MinimumImbalanceThreshold;

    private readonly record struct GameState(int C0, int C1, int C2, int TurnSum);
}
