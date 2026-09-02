using RepoStampStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StampingTheSequence;

// LeetCode 936. Stamping The Sequence: the reverse-simulation solution - repeatedly
// find a window that could have been the LAST stamp applied (every character either
// already matches the stamp or has already been turned to '?' by a later-discovered
// stamp, with at least one character still "live"), turn it to '?', and record its
// start index - discovers moves in reverse chronological order, so this repo's own
// Stack<int> (the same "explicit repo Stack instead of System.Collections.Generic
// .Stack" move AsteroidCollisionTests/BasicCalculatorTests already make) is exactly
// what's needed to record-then-reverse them back into forward order for free.
public sealed class StampingTheSequenceTests
{
    [Fact]
    public void MovesToStamp_ClassicExample_ProducesTargetWhenReplayed()
    {
        var moves = MovesToStamp("abc", "ababc");

        Assert.Equal(2, moves.Length);
        var reconstructsTarget = ReplayReconstructsTarget("abc", "ababc", moves);
        Assert.True(reconstructsTarget);
    }

    [Fact]
    public void MovesToStamp_OverlappingStampsNeeded_ProducesTargetWhenReplayed()
    {
        var moves = MovesToStamp("abca", "aabcaca");

        Assert.Equal(3, moves.Length);
        var reconstructsTarget = ReplayReconstructsTarget("abca", "aabcaca", moves);
        Assert.True(reconstructsTarget);
    }

    [Fact]
    public void MovesToStamp_TargetCannotBeFormedFromStamp_ReturnsEmpty()
    {
        var moves = MovesToStamp("a", "b");

        Assert.Empty(moves);
    }

    // Applies the returned start indices, in order, onto a blank '?' canvas - the
    // exact forward operation LeetCode's own judge replays - and checks it lands on
    // target. Multiple move sequences are valid for the same input, so this checks
    // the observable postcondition instead of asserting one exact index sequence.
    private static bool ReplayReconstructsTarget(string stamp, string target, int[] moves)
    {
        var chars = new char[target.Length];
        Array.Fill(chars, '?');

        foreach (var start in moves)
        {
            for (var k = 0; k < stamp.Length; k++)
            {
                chars[start + k] = stamp[k];
            }
        }

        return new string(chars) == target;
    }

    private static int[] MovesToStamp(string stamp, string target)
    {
        var windowCount = target.Length - stamp.Length + 1;

        if (windowCount <= 0)
        {
            return [];
        }

        return TryReverseStamp(stamp, target, windowCount, out var order) ? ExtractMovesInOrder(order) : [];
    }

    // Runs the reverse-simulation rounds until every character of target has been
    // turned to '?' (success) or a round stamps nothing (stuck). The discovered
    // stamp order comes back via `order` regardless of outcome.
    private static bool TryReverseStamp(string stamp, string target, int windowCount, out RepoStampStack order)
    {
        var chars = target.ToCharArray();
        var done = new bool[windowCount];
        order = new RepoStampStack();
        var turnedCount = 0;
        var scanState = new StampScanState(chars, stamp, done, order);

        for (var round = 0; round < windowCount && turnedCount < target.Length; round++)
        {
            if (!StampAvailableWindows(scanState, ref turnedCount))
            {
                break;
            }
        }

        return turnedCount == target.Length;
    }

    // The stack pops in reverse discovery order, which is exactly forward
    // chronological stamping order.
    private static int[] ExtractMovesInOrder(RepoStampStack order)
    {
        var result = new int[order.Count];

        for (var k = 0; k < result.Length; k++)
        {
            order.TryPop(out result[k]);
        }

        return result;
    }

    // Bundles one round's scan inputs so StampAvailableWindows stays within the
    // parameter-count limit: the char canvas/stamp being matched, plus the
    // done-tracking array and discovered-order stack that round mutates.
    private readonly record struct StampScanState(char[] Chars, string Stamp, bool[] Done, RepoStampStack Order);

    // Tries every not-yet-done window once; returns whether any window stamped
    // this round (a false result means the reverse simulation is stuck).
    private static bool StampAvailableWindows(StampScanState state, ref int turnedCount)
    {
        var stampedThisRound = false;

        for (var i = 0; i < state.Done.Length; i++)
        {
            if (state.Done[i] || !TryStampWindow(state.Chars, state.Stamp, i, ref turnedCount))
            {
                continue;
            }

            state.Done[i] = true;
            stampedThisRound = true;
            state.Order.Push(i);
        }

        return stampedThisRound;
    }

    // A window is stampable only if every character still visible (not yet '?')
    // matches the stamp at that offset, and at least one character is still
    // visible (otherwise this window is a no-op re-stamp of already-done work).
    private static bool TryStampWindow(char[] chars, string stamp, int start, ref int turnedCount)
    {
        if (!WindowMatchesStamp(chars, stamp, start, out var hasLiveCharacter) || !hasLiveCharacter)
        {
            return false;
        }

        for (var k = 0; k < stamp.Length; k++)
        {
            if (chars[start + k] != '?')
            {
                chars[start + k] = '?';
                turnedCount++;
            }
        }

        return true;
    }

    // A window "matches" if every already-visible character agrees with the
    // stamp at that offset; hasLiveCharacter reports whether any character was
    // still visible (a window with none left is a no-op re-stamp).
    private static bool WindowMatchesStamp(char[] chars, string stamp, int start, out bool hasLiveCharacter)
    {
        hasLiveCharacter = false;

        for (var k = 0; k < stamp.Length; k++)
        {
            var current = chars[start + k];

            if (current == '?')
            {
                continue;
            }

            if (current != stamp[k])
            {
                return false;
            }

            hasLiveCharacter = true;
        }

        return true;
    }
}
