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
public sealed partial class StampingTheSequenceTests
{
    public static TheoryData<StampCase> Examples =>
        new()
        {
            { new StampCase(Stamp: "abc", Target: "ababc", ExpectedMoveCount: 2) },
            { new StampCase(Stamp: "abca", Target: "aabcaca", ExpectedMoveCount: 3) },
            { new StampCase(Stamp: "abc", Target: "abc", ExpectedMoveCount: 1) },
            { new StampCase(Stamp: "a", Target: "b", ExpectedMoveCount: 0) },
            { new StampCase(Stamp: "abc", Target: "ab", ExpectedMoveCount: 0) },
            { new StampCase(Stamp: "abc", Target: "abca", ExpectedMoveCount: 0) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MovesToStampByListPrepend_LeetCodeExamples_ProducesTargetWhenReplayed(StampCase example)
    {
        var moves = StampingTheSequenceSolution.MovesToStampByListPrepend(
            new StampingTheSequenceSolution.StampPattern(example.Stamp),
            new StampingTheSequenceSolution.TargetText(example.Target));

        AssertMovesReplayOntoTarget(example, moves);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MovesToStampByStackReverse_LeetCodeExamples_ProducesTargetWhenReplayed(StampCase example)
    {
        var moves = StampingTheSequenceSolution.MovesToStampByStackReverse(
            new StampingTheSequenceSolution.StampPattern(example.Stamp),
            new StampingTheSequenceSolution.TargetText(example.Target));

        AssertMovesReplayOntoTarget(example, moves);
    }

    // An expected count of zero means the target is unreachable, so there is nothing
    // to replay; otherwise the returned indices must rebuild target exactly.
    private static void AssertMovesReplayOntoTarget(StampCase example, int[] moves)
    {
        Assert.Equal(example.ExpectedMoveCount, moves.Length);

        if (example.ExpectedMoveCount == 0)
        {
            return;
        }

        var replayed = Replay(example.Stamp, example.Target.Length, moves);

        Assert.Equal(example.Target, replayed);
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

    // One LeetCode example: the stamp, the target it must produce, and how many moves
    // that takes (zero meaning it cannot be produced at all). The three travel together
    // at every row and every call site, so they are one thing with a name rather than
    // two adjacent strings a caller can transpose.
    public readonly record struct StampCase(string Stamp, string Target, int ExpectedMoveCount);
}
