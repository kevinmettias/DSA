using DSAExperimentation.LeetCode.Solutions.ImplementRand10UsingRand7;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ImplementRand10UsingRand7;

// LeetCode's published cases, answered as its judge would accept - n values, each in
// 1..10 - and as it would not.
public sealed partial class ImplementRand10UsingRand7AnswerCheckTests
{
    public static TheoryData<int, int[], int[]> AcceptedAnswers =>
        new()
        {
            { 1, [2], [2] },
            { 2, [1, 10], [2, 8] },
            { 3, [3, 8, 10], [3, 8, 10] },
        };

    // A value under 1, a value over 10, one value short, and one too many.
    public static TheoryData<int, int[], int[]> RefusedAnswers =>
        new()
        {
            { 1, [0], [2] },
            { 2, [2, 11], [2, 8] },
            { 3, [3, 8], [3, 8, 10] },
            { 1, [2, 2], [2] },
        };

    [Theory]
    [MemberData(nameof(AcceptedAnswers))]
    public void IsAccepted_ValuesInRangeOnePerCall_IsTrue(int callCount, int[] answer, int[] recorded) =>
        Assert.True(ImplementRand10UsingRand7AnswerCheck.IsAccepted(callCount, answer, recorded));

    [Theory]
    [MemberData(nameof(RefusedAnswers))]
    public void IsAccepted_AnAnswerTheJudgeRefuses_IsFalse(int callCount, int[] answer, int[] recorded) =>
        Assert.False(ImplementRand10UsingRand7AnswerCheck.IsAccepted(callCount, answer, recorded));
}
