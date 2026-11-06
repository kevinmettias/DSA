using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.RandomPickWithWeight;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RandomPickWithWeight;

// LeetCode's second example script, answered in ways the check must take and ways it must refuse.
public sealed partial class RandomPickWithWeightAnswerCheckTests
{
    // LeetCode's example: its calls, their arguments, and the answer it records, which the check does not consult.
    private const string Arguments = "[[[1,3]],[],[],[],[],[]]";
    private const string Recorded = "[null,1,1,1,1,0]";

    private static readonly string[] Calls = ["Solution", "pickIndex", "pickIndex", "pickIndex", "pickIndex", "pickIndex"];

    public static TheoryData<string> AcceptedAnswers =>
        new()
        {
            { "[null,1,1,1,1,0]" },
            { "[null,0,0,0,0,0]" },
        };

    // An index past w, a negative index, a pick left unanswered, and a constructor answering.
    public static TheoryData<string> RefusedAnswers =>
        new()
        {
            { "[null,1,1,2,1,0]" },
            { "[null,1,1,-1,1,0]" },
            { "[null,1,1,1,1]" },
            { "[0,1,1,1,1,0]" },
        };

    [Theory]
    [MemberData(nameof(AcceptedAnswers))]
    public void IsAccepted_EveryPickAnIndexOfPositiveWeight_IsTrue(string answer) => Assert.True(IsAccepted(answer));

    [Theory]
    [MemberData(nameof(RefusedAnswers))]
    public void IsAccepted_SomeCallAnsweredAsLeetCodeWouldNot_IsFalse(string answer) => Assert.False(IsAccepted(answer));

    private static bool IsAccepted(string answer)
    {
        var arguments = ArrayOf(Arguments);
        var answered = ArrayOf(answer);
        var recorded = ArrayOf(Recorded);

        return RandomPickWithWeightAnswerCheck.IsAccepted(Calls, arguments, answered, recorded);
    }

    private static JsonArray ArrayOf(string text) =>
        JsonNode.Parse(text) as JsonArray ?? throw new ArgumentException($"{text} is not a JSON array.", nameof(text));
}
