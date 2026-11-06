using System.Text.Json;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.InsertDeleteGetRandomO1;

namespace DSAExperimentation.LeetCode.Solutions.Tests.InsertDeleteGetRandomO1;

// Each row is a script in LeetCode's notation - its calls, their arguments - and an answer to it. Any member
// stands as a draw; a draw the set does not hold, an insert or remove answered against the replayed set, and
// an answer to the constructor are refused.
public sealed partial class InsertDeleteGetRandomO1AnswerCheckTests
{
    public static TheoryData<string, string, string> AnswersTheReplayAllows =>
        new()
        {
            // LeetCode's example, as it records it.
            {
                """["RandomizedSet","insert","remove","insert","getRandom","remove","insert","getRandom"]""",
                """[[],[1],[2],[2],[],[1],[2],[]]""",
                """[null,true,false,true,2,true,false,2]"""
            },
            // The other member at the first draw.
            {
                """["RandomizedSet","insert","remove","insert","getRandom","remove","insert","getRandom"]""",
                """[[],[1],[2],[2],[],[1],[2],[]]""",
                """[null,true,false,true,1,true,false,2]"""
            },
            // A value removed and inserted again is a member again.
            {
                """["RandomizedSet","insert","remove","insert","getRandom"]""",
                """[[],[-7],[-7],[-7],[]]""",
                """[null,true,true,true,-7]"""
            },
        };

    public static TheoryData<string, string, string> AnswersTheReplayRefuses =>
        new()
        {
            // 1 was removed before the second draw.
            {
                """["RandomizedSet","insert","remove","insert","getRandom","remove","insert","getRandom"]""",
                """[[],[1],[2],[2],[],[1],[2],[]]""",
                """[null,true,false,true,2,true,false,1]"""
            },
            // 2 was never inserted, so removing it answers false.
            {
                """["RandomizedSet","insert","remove"]""",
                """[[],[1],[2]]""",
                """[null,true,true]"""
            },
            // 1 is already a member, so inserting it again answers false.
            {
                """["RandomizedSet","insert","insert"]""",
                """[[],[1],[1]]""",
                """[null,true,true]"""
            },
            // The constructor answers nothing.
            {
                """["RandomizedSet","insert"]""",
                """[[],[1]]""",
                """[true,true]"""
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

        return InsertDeleteGetRandomO1AnswerCheck.IsAccepted(names, argumentArray, answerArray, answerArray);
    }

    private static JsonArray ArrayOf(string notation) =>
        JsonNode.Parse(notation) as JsonArray ?? throw new ArgumentException("The notation is no array.", nameof(notation));
}
