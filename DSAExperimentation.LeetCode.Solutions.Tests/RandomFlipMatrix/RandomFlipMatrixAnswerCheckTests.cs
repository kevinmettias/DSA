using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.RandomFlipMatrix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RandomFlipMatrix;

// LeetCode's example script, answered in ways the check must take and ways it must refuse.
public sealed partial class RandomFlipMatrixAnswerCheckTests
{
    // LeetCode's example: its calls, their arguments, and the answer it records, which the check does not consult.
    private const string Arguments = "[[3,1],[],[],[],[],[]]";
    private const string Recorded = "[null,[1,0],[2,0],[0,0],null,[2,0]]";

    private static readonly string[] Calls = ["Solution", "flip", "flip", "flip", "reset", "flip"];

    // The recorded flips, and others that name a cell flipped before the reset again after it.
    public static TheoryData<string> AcceptedAnswers =>
        new()
        {
            { "[null,[1,0],[2,0],[0,0],null,[2,0]]" },
            { "[null,[0,0],[1,0],[2,0],null,[0,0]]" },
        };

    // A cell flipped twice before a reset, a column past n, a reset answering, a flip left unanswered, and a
    // flip answering a number rather than a cell.
    public static TheoryData<string> RefusedAnswers =>
        new()
        {
            { "[null,[0,0],[0,0],[2,0],null,[1,0]]" },
            { "[null,[0,1],[1,0],[2,0],null,[0,0]]" },
            { "[null,[1,0],[2,0],[0,0],[0,0],[2,0]]" },
            { "[null,[1,0],[2,0],[0,0],null]" },
            { "[null,[1,0],[2,0],[0,0],null,1]" },
        };

    [Theory]
    [MemberData(nameof(AcceptedAnswers))]
    public void IsAccepted_EveryFlipACellStillZero_IsTrue(string answer) => Assert.True(IsAccepted(answer));

    [Theory]
    [MemberData(nameof(RefusedAnswers))]
    public void IsAccepted_SomeCallAnsweredAsLeetCodeWouldNot_IsFalse(string answer) => Assert.False(IsAccepted(answer));

    private static bool IsAccepted(string answer)
    {
        var arguments = ArrayOf(Arguments);
        var answered = ArrayOf(answer);
        var recorded = ArrayOf(Recorded);

        return RandomFlipMatrixAnswerCheck.IsAccepted(Calls, arguments, answered, recorded);
    }

    private static JsonArray ArrayOf(string text) =>
        JsonNode.Parse(text) as JsonArray ?? throw new ArgumentException($"{text} is not a JSON array.", nameof(text));
}
