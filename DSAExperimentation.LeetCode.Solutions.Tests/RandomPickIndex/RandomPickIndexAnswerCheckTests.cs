using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.RandomPickIndex;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RandomPickIndex;

// LeetCode's example script, answered in ways the check must take and ways it must refuse.
public sealed partial class RandomPickIndexAnswerCheckTests
{
    // LeetCode's example: its calls, their arguments, and the answer it records, which the check does not consult.
    private const string Arguments = "[[[1,2,3,3,3]],[3],[1],[3]]";
    private const string Recorded = "[null,4,0,2]";

    private static readonly string[] Calls = ["Solution", "pick", "pick", "pick"];

    public static TheoryData<string> AcceptedAnswers =>
        new()
        {
            { "[null,4,0,2]" },
            { "[null,2,0,3]" },
        };

    // An index holding another value, an index past nums, a pick left unanswered, and a constructor answering.
    public static TheoryData<string> RefusedAnswers =>
        new()
        {
            { "[null,1,0,2]" },
            { "[null,5,0,2]" },
            { "[null,4,0]" },
            { "[4,4,0,2]" },
        };

    [Theory]
    [MemberData(nameof(AcceptedAnswers))]
    public void IsAccepted_EveryPickAnIndexOfItsTarget_IsTrue(string answer) => Assert.True(IsAccepted(answer));

    [Theory]
    [MemberData(nameof(RefusedAnswers))]
    public void IsAccepted_SomeCallAnsweredAsLeetCodeWouldNot_IsFalse(string answer) => Assert.False(IsAccepted(answer));

    private static bool IsAccepted(string answer)
    {
        var arguments = ArrayOf(Arguments);
        var answered = ArrayOf(answer);
        var recorded = ArrayOf(Recorded);

        return RandomPickIndexAnswerCheck.IsAccepted(Calls, arguments, answered, recorded);
    }

    private static JsonArray ArrayOf(string text) =>
        JsonNode.Parse(text) as JsonArray ?? throw new ArgumentException($"{text} is not a JSON array.", nameof(text));
}
