using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.RandomPickWithBlacklist;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RandomPickWithBlacklist;

// LeetCode's example script, answered in ways the check must take and ways it must refuse.
public sealed partial class RandomPickWithBlacklistAnswerCheckTests
{
    // LeetCode's example: its calls, their arguments, and the answer it records, which the check does not consult.
    private const string Arguments = "[[7,[2,3,5]],[],[],[],[],[],[],[]]";
    private const string Recorded = "[null,0,4,1,6,1,0,4]";

    private static readonly string[] Calls = ["Solution", "pick", "pick", "pick", "pick", "pick", "pick", "pick"];

    public static TheoryData<string> AcceptedAnswers =>
        new()
        {
            { "[null,0,4,1,6,1,0,4]" },
            { "[null,6,6,6,6,6,6,6]" },
        };

    // A blacklisted number, a number past n, a pick left unanswered, and a constructor answering.
    public static TheoryData<string> RefusedAnswers =>
        new()
        {
            { "[null,0,4,1,6,1,0,2]" },
            { "[null,0,4,1,7,1,0,4]" },
            { "[null,0,4,1,6,1,0]" },
            { "[0,0,4,1,6,1,0,4]" },
        };

    [Theory]
    [MemberData(nameof(AcceptedAnswers))]
    public void IsAccepted_EveryPickAWhitelistedNumber_IsTrue(string answer) => Assert.True(IsAccepted(answer));

    [Theory]
    [MemberData(nameof(RefusedAnswers))]
    public void IsAccepted_SomeCallAnsweredAsLeetCodeWouldNot_IsFalse(string answer) => Assert.False(IsAccepted(answer));

    private static bool IsAccepted(string answer)
    {
        var arguments = ArrayOf(Arguments);
        var answered = ArrayOf(answer);
        var recorded = ArrayOf(Recorded);

        return RandomPickWithBlacklistAnswerCheck.IsAccepted(Calls, arguments, answered, recorded);
    }

    private static JsonArray ArrayOf(string text) =>
        JsonNode.Parse(text) as JsonArray ?? throw new ArgumentException($"{text} is not a JSON array.", nameof(text));
}
