using DSAExperimentation.LeetCode.Solutions.StampingTheSequence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StampingTheSequence;

public sealed partial class StampingTheSequenceAnswerCheckTests
{
    public static TheoryData<string, string, int[], int[], bool> Answers =>
        new()
        {
            { "abc", "ababc", [0, 2], [0, 2], true },
            { "abc", "ababc", [1, 0, 2], [0, 2], true },
            { "abca", "aabcaca", [3, 0, 1], [3, 0, 1], true },
            { "abc", "ababc", [2, 0], [0, 2], false },
            { "abc", "ababc", [3], [0, 2], false },
            { "abc", "ababc", [], [0, 2], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForStampsThatSpellTheTarget(string stamp, string target, int[] answer, int[] recorded, bool expected)
        => Assert.Equal(expected, StampingTheSequenceAnswerCheck.IsAccepted(new(stamp), new(target), answer, recorded));
}
