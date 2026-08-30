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
        Assert.True(ReplayReconstructsTarget("abc", "ababc", moves));
    }

    [Fact]
    public void MovesToStamp_OverlappingStampsNeeded_ProducesTargetWhenReplayed()
    {
        var moves = MovesToStamp("abca", "aabcaca");

        Assert.Equal(3, moves.Length);
        Assert.True(ReplayReconstructsTarget("abca", "aabcaca", moves));
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

        var chars = target.ToCharArray();
        var done = new bool[windowCount];
        var order = new RepoStampStack();
        var turnedCount = 0;

        for (var round = 0; round < windowCount && turnedCount < target.Length; round++)
        {
            var stampedThisRound = false;

            for (var i = 0; i < windowCount; i++)
            {
                if (done[i] || !TryStampWindow(chars, stamp, i, ref turnedCount))
                {
                    continue;
                }

                done[i] = true;
                stampedThisRound = true;
                order.Push(i);
            }

            if (!stampedThisRound)
            {
                break;
            }
        }

        if (turnedCount != target.Length)
        {
            return [];
        }

        var result = new int[order.Count];

        for (var k = 0; k < result.Length; k++)
        {
            order.TryPop(out result[k]);
        }

        return result;
    }

    // A window is stampable only if every character still visible (not yet '?')
    // matches the stamp at that offset, and at least one character is still
    // visible (otherwise this window is a no-op re-stamp of already-done work).
    private static bool TryStampWindow(char[] chars, string stamp, int start, ref int turnedCount)
    {
        var hasLiveCharacter = false;

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

        if (!hasLiveCharacter)
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
}
