using DSAExperimentation.LeetCode.Solutions.GenerateRandomPointInACircle;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GenerateRandomPointInACircle;

// Three randPoint() calls on a circle, answered as LeetCode's judge would accept -
// each point inside the circle or on it, within 10^-5 - and as it would not.
public sealed partial class GenerateRandomPointInACircleAnswerCheckTests
{
    private const string Calls = """["Solution","randPoint","randPoint","randPoint"]""";
    private const string UnitCircle = "[[1.0,0.0,0.0],[],[],[]]";
    private const string Published = "[null,[-0.02493,-0.38077],[0.82314,0.38945],[0.36572,0.17248]]";

    // LeetCode's published script and answer; points on the circumference; a point past
    // it by less than 10^-5; and a circle away from the origin.
    public static TheoryData<string, string> AcceptedAnswers =>
        new()
        {
            { UnitCircle, Published },
            { UnitCircle, "[null,[1.0,0.0],[0.0,-1.0],[0.6,0.8]]" },
            { UnitCircle, "[null,[1.000005,0.0],[0.0,0.0],[0.0,0.0]]" },
            { "[[10.0,5.0,-3.0],[],[],[]]", "[null,[15.0,-3.0],[5.0,7.0],[-1.0,-11.0]]" },
        };

    // A point past the circumference by more than 10^-5; a point of three coordinates; a
    // missing point; an answer short of a call; and a point of the origin's circle
    // judged against one centered elsewhere.
    public static TheoryData<string, string> RefusedAnswers =>
        new()
        {
            { UnitCircle, "[null,[1.0001,0.0],[0.0,0.0],[0.0,0.0]]" },
            { UnitCircle, "[null,[0.5,0.5,0.5],[0.0,0.0],[0.0,0.0]]" },
            { UnitCircle, "[null,null,[0.0,0.0],[0.0,0.0]]" },
            { UnitCircle, "[null,[0.0,0.0],[0.0,0.0]]" },
            { "[[10.0,5.0,-3.0],[],[],[]]", "[null,[-6.0,0.0],[5.0,7.0],[5.0,-3.0]]" },
        };

    [Theory]
    [MemberData(nameof(AcceptedAnswers))]
    public void IsAccepted_EveryPointWithinTheCircle_IsTrue(string arguments, string answer) =>
        Assert.True(IsAcceptedFor(arguments, answer));

    [Theory]
    [MemberData(nameof(RefusedAnswers))]
    public void IsAccepted_AnAnswerTheJudgeRefuses_IsFalse(string arguments, string answer) =>
        Assert.False(IsAcceptedFor(arguments, answer));

    private static bool IsAcceptedFor(string arguments, string answer) =>
        new DesignScript(Calls, arguments, Published).IsAccepting(GenerateRandomPointInACircleAnswerCheck.IsAccepted, answer);
}
