using System.Text.Json;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.LinkedListRandomNode;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LinkedListRandomNode;

// Each row is a script in LeetCode's notation - its calls, their arguments - and an answer to it. Any value
// of the constructor's list stands as a draw, however often; a value not in the list, a missing draw and an
// answer to the constructor are refused.
public sealed partial class LinkedListRandomNodeAnswerCheckTests
{
    public static TheoryData<string, string, string> AnswersTheReplayAllows =>
        new()
        {
            // LeetCode's example, as it records it.
            {
                """["Solution","getRandom","getRandom","getRandom","getRandom","getRandom"]""",
                """[[[1,2,3]],[],[],[],[],[]]""",
                """[null,1,3,2,2,3]"""
            },
            // The same value at every draw.
            {
                """["Solution","getRandom","getRandom","getRandom","getRandom","getRandom"]""",
                """[[[1,2,3]],[],[],[],[],[]]""",
                """[null,2,2,2,2,2]"""
            },
            // A one-node list draws its node.
            {
                """["Solution","getRandom"]""",
                """[[[-10000]],[]]""",
                """[null,-10000]"""
            },
        };

    public static TheoryData<string, string, string> AnswersTheReplayRefuses =>
        new()
        {
            // 4 is not in the list.
            {
                """["Solution","getRandom","getRandom"]""",
                """[[[1,2,3]],[],[]]""",
                """[null,1,4]"""
            },
            // A draw answered nothing.
            {
                """["Solution","getRandom","getRandom"]""",
                """[[[1,2,3]],[],[]]""",
                """[null,1,null]"""
            },
            // The constructor answers nothing.
            {
                """["Solution","getRandom"]""",
                """[[[1,2,3]],[]]""",
                """[1,1]"""
            },
        };

    [Theory]
    [MemberData(nameof(AnswersTheReplayAllows))]
    public void IsAccepted_AnswerTheReplayAllows_ReturnsTrue(string calls, string arguments, string answer)
        => Assert.True(IsAccepted(calls, arguments, answer));

    [Theory]
    [MemberData(nameof(AnswersTheReplayRefuses))]
    public void IsAccepted_AnswerTheReplayRefuses_ReturnsFalse(string calls, string arguments, string answer)
        => Assert.False(IsAccepted(calls, arguments, answer));

    // The check takes no part from the recorded answer, so the answer stands in for it.
    private static bool IsAccepted(string calls, string arguments, string answer)
    {
        var names = JsonSerializer.Deserialize<string[]>(calls) ?? [];
        var argumentArray = ArrayOf(arguments);
        var answerArray = ArrayOf(answer);

        return LinkedListRandomNodeAnswerCheck.IsAccepted(names, argumentArray, answerArray, answerArray);
    }

    private static JsonArray ArrayOf(string notation) =>
        JsonNode.Parse(notation) as JsonArray ?? throw new ArgumentException("The notation is no array.", nameof(notation));
}
