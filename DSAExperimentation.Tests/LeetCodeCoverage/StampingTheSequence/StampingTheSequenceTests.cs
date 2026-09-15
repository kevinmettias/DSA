using DSAExperimentation.LeetCode.StampingTheSequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StampingTheSequence;

// Harness only: both strategies live in StampingTheSequenceSolution and are asserted
// against the same examples - LeetCode's two published cases, the degenerate
// stamp-is-target case, and three impossible ones (a target the stamp can never
// produce, a target shorter than the stamp, and a target whose reverse simulation
// gets stuck partway).
//
// Many move sequences are valid for the same input, so each example pins the move
// COUNT and the observable postcondition LeetCode's own judge checks - replaying the
// returned start indices, in order, onto a blank '?' canvas lands on target.
public sealed class StampingTheSequenceTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "abc", "ababc", 2 },
            { "abca", "aabcaca", 3 },
            { "abc", "abc", 1 },
            { "a", "b", 0 },
            { "abc", "ab", 0 },
            { "abc", "abca", 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MovesToStampByListPrepend_LeetCodeExamples_ProducesTargetWhenReplayed(
        string stamp, string target, int expectedMoveCount)
    {
        var moves = StampingTheSequenceSolution.MovesToStampByListPrepend(
            new StampingTheSequenceSolution.StampPattern(stamp),
            new StampingTheSequenceSolution.TargetText(target));

        AssertMovesReplayOntoTarget(stamp, target, expectedMoveCount, moves);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MovesToStampByStackReverse_LeetCodeExamples_ProducesTargetWhenReplayed(
        string stamp, string target, int expectedMoveCount)
    {
        var moves = StampingTheSequenceSolution.MovesToStampByStackReverse(
            new StampingTheSequenceSolution.StampPattern(stamp),
            new StampingTheSequenceSolution.TargetText(target));

        AssertMovesReplayOntoTarget(stamp, target, expectedMoveCount, moves);
    }

    // An expected count of zero means the target is unreachable, so there is nothing
    // to replay; otherwise the returned indices must rebuild target exactly.
    private static void AssertMovesReplayOntoTarget(
        string stamp, string target, int expectedMoveCount, int[] moves)
    {
        Assert.Equal(expectedMoveCount, moves.Length);

        if (expectedMoveCount == 0)
        {
            return;
        }

        Assert.Equal(target, Replay(stamp, target.Length, moves));
    }

    // The exact forward operation LeetCode's judge replays: stamp each start index,
    // in order, over a canvas that starts out entirely unstamped.
    private static string Replay(string stamp, int length, int[] moves)
    {
        var chars = new char[length];
        Array.Fill(chars, '?');

        foreach (var start in moves)
        {
            for (var k = 0; k < stamp.Length; k++)
            {
                chars[start + k] = stamp[k];
            }
        }

        return new string(chars);
    }
}
