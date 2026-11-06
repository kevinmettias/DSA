using DSAExperimentation.LeetCode.Solutions.ShuffleAnArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShuffleAnArray;

// LeetCode's published script, answered as its judge would accept - each shuffle any
// permutation of nums, each reset nums as given - and as it would not.
public sealed partial class ShuffleAnArrayAnswerCheckTests
{
    private const string Calls = """["Solution","shuffle","reset","shuffle"]""";
    private const string Arguments = "[[[1,2,3]],[],[],[]]";
    private const string Published = "[null,[3,1,2],[1,2,3],[1,3,2]]";

    public static TheoryData<string> AcceptedAnswers =>
        new()
        {
            { Published },
            { "[null,[1,2,3],[1,2,3],[2,3,1]]" },
        };

    // A reset that is not nums, a shuffle repeating a value, a shuffle missing one, an
    // answer short of a call, and an answer to the constructor.
    public static TheoryData<string> RefusedAnswers =>
        new()
        {
            { "[null,[3,1,2],[3,1,2],[1,3,2]]" },
            { "[null,[3,1,1],[1,2,3],[1,3,2]]" },
            { "[null,[3,1],[1,2,3],[1,3,2]]" },
            { "[null,[3,1,2],[1,2,3]]" },
            { "[[],[3,1,2],[1,2,3],[1,3,2]]" },
        };

    [Theory]
    [MemberData(nameof(AcceptedAnswers))]
    public void IsAccepted_EveryShuffleAPermutationAndEveryResetNums_IsTrue(string answer) =>
        Assert.True(IsAcceptedFor(answer));

    [Theory]
    [MemberData(nameof(RefusedAnswers))]
    public void IsAccepted_AnAnswerTheJudgeRefuses_IsFalse(string answer) =>
        Assert.False(IsAcceptedFor(answer));

    private static bool IsAcceptedFor(string answer) =>
        new DesignScript(Calls, Arguments, Published).IsAccepting(ShuffleAnArrayAnswerCheck.IsAccepted, answer);
}
