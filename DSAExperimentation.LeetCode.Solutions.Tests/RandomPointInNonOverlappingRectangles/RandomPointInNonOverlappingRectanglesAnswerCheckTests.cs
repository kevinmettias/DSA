using DSAExperimentation.LeetCode.Solutions.RandomPointInNonOverlappingRectangles;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RandomPointInNonOverlappingRectangles;

// LeetCode's published script, answered as its judge would accept - each pick an
// integer point inside or on one of the rectangles - and as it would not.
public sealed partial class RandomPointInNonOverlappingRectanglesAnswerCheckTests
{
    private const string Calls = """["Solution","pick","pick","pick","pick","pick"]""";
    private const string Arguments = "[[[[-2,-2,1,1],[2,2,4,6]]],[],[],[],[],[]]";
    private const string Published = "[null,[1,-2],[1,-1],[-1,-2],[-2,-2],[0,0]]";

    // The published answer, and one of corners and edges of both rectangles.
    public static TheoryData<string> AcceptedAnswers =>
        new()
        {
            { Published },
            { "[null,[2,2],[4,6],[3,4],[-2,1],[1,1]]" },
        };

    // A point between the rectangles, a point past the second's top, a point that is not
    // an integer one, an answer short of a call, and a missing point.
    public static TheoryData<string> RefusedAnswers =>
        new()
        {
            { "[null,[1,2],[1,-1],[-1,-2],[-2,-2],[0,0]]" },
            { "[null,[1,-2],[3,7],[-1,-2],[-2,-2],[0,0]]" },
            { "[null,[1,-2],[0.5,0],[-1,-2],[-2,-2],[0,0]]" },
            { "[null,[1,-2],[1,-1],[-1,-2],[-2,-2]]" },
            { "[null,[1,-2],null,[-1,-2],[-2,-2],[0,0]]" },
        };

    [Theory]
    [MemberData(nameof(AcceptedAnswers))]
    public void IsAccepted_EveryPickInsideARectangle_IsTrue(string answer) =>
        Assert.True(IsAcceptedFor(answer));

    [Theory]
    [MemberData(nameof(RefusedAnswers))]
    public void IsAccepted_AnAnswerTheJudgeRefuses_IsFalse(string answer) =>
        Assert.False(IsAcceptedFor(answer));

    private static bool IsAcceptedFor(string answer) =>
        new DesignScript(Calls, Arguments, Published).IsAccepting(RandomPointInNonOverlappingRectanglesAnswerCheck.IsAccepted, answer);
}
