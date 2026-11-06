using System.Text.Json;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.InsertDeleteGetRandomO1DuplicatesAllowed;

namespace DSAExperimentation.LeetCode.Solutions.Tests.InsertDeleteGetRandomO1DuplicatesAllowed;

// Each row is a script in LeetCode's notation - its calls, their arguments - and an answer to it. Any value
// held stands as a draw, however many copies; a draw of a value no longer held, an insert or remove answered
// against the replayed counts, and an answer to the constructor are refused.
public sealed partial class InsertDeleteGetRandomO1DuplicatesAllowedAnswerCheckTests
{
    public static TheoryData<string, string, string> AnswersTheReplayAllows =>
        new()
        {
            // LeetCode's example, as it records it.
            {
                """["RandomizedCollection","insert","insert","insert","getRandom","remove","getRandom"]""",
                """[[],[1],[1],[2],[],[1],[]]""",
                """[null,true,false,true,2,true,1]"""
            },
            // The other value at each draw: one copy of 1 is still held after the remove.
            {
                """["RandomizedCollection","insert","insert","insert","getRandom","remove","getRandom"]""",
                """[[],[1],[1],[2],[],[1],[]]""",
                """[null,true,false,true,1,true,2]"""
            },
            // Both copies of 5 removed, then 5 inserted again as a value not held.
            {
                """["RandomizedCollection","insert","insert","remove","remove","remove","insert","getRandom"]""",
                """[[],[5],[5],[5],[5],[5],[5],[]]""",
                """[null,true,false,true,true,false,true,5]"""
            },
        };

    public static TheoryData<string, string, string> AnswersTheReplayRefuses =>
        new()
        {
            // 1's only copy was removed before the draw.
            {
                """["RandomizedCollection","insert","insert","remove","getRandom"]""",
                """[[],[1],[2],[1],[]]""",
                """[null,true,true,true,1]"""
            },
            // A second copy of 1 goes in, but 1 was already held.
            {
                """["RandomizedCollection","insert","insert"]""",
                """[[],[1],[1]]""",
                """[null,true,true]"""
            },
            // Both copies of 5 are gone, so a third remove answers false.
            {
                """["RandomizedCollection","insert","insert","remove","remove","remove"]""",
                """[[],[5],[5],[5],[5],[5]]""",
                """[null,true,false,true,true,true]"""
            },
            // The constructor answers nothing.
            {
                """["RandomizedCollection","insert"]""",
                """[[],[1]]""",
                """[false,true]"""
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

        return InsertDeleteGetRandomO1DuplicatesAllowedAnswerCheck.IsAccepted(names, argumentArray, answerArray, answerArray);
    }

    private static JsonArray ArrayOf(string notation) =>
        JsonNode.Parse(notation) as JsonArray ?? throw new ArgumentException("The notation is no array.", nameof(notation));
}
