using System.Text.Json;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Solutions.AllOneDataStructure;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AllOneDataStructure;

// Each row is a script in LeetCode's notation - its calls, their arguments - and an answer to it. Keys tied
// at the count a read reports each stand; a key at another count, a key never held, "" while a key is held
// and an answer to an inc or dec are refused.
public sealed partial class AllOneDataStructureAnswerCheckTests
{
    public static TheoryData<string, string, string> AnswersTheReplayAllows =>
        new()
        {
            // LeetCode's example, as it records it.
            {
                """["AllOne","inc","inc","getMaxKey","getMinKey","inc","getMaxKey","getMinKey"]""",
                """[[],["hello"],["hello"],[],[],["leet"],[],[]]""",
                """[null,null,null,"hello","hello",null,"hello","leet"]"""
            },
            // a and b tie at 1, so either answers either read.
            {
                """["AllOne","inc","inc","getMaxKey","getMinKey","getMaxKey","getMinKey"]""",
                """[[],["a"],["b"],[],[],[],[]]""",
                """[null,null,null,"b","b","a","a"]"""
            },
            // a's dec takes its count to 0, so nothing is held.
            {
                """["AllOne","inc","dec","getMaxKey","getMinKey"]""",
                """[[],["a"],["a"],[],[]]""",
                """[null,null,null,"",""]"""
            },
        };

    public static TheoryData<string, string, string> AnswersTheReplayRefuses =>
        new()
        {
            // leet is held once, hello twice.
            {
                """["AllOne","inc","inc","getMaxKey","getMinKey","inc","getMaxKey","getMinKey"]""",
                """[[],["hello"],["hello"],[],[],["leet"],[],[]]""",
                """[null,null,null,"hello","hello",null,"leet","leet"]"""
            },
            // c was never held.
            {
                """["AllOne","inc","getMaxKey"]""",
                """[[],["a"],[]]""",
                """[null,null,"c"]"""
            },
            // a is held, so "" does not stand.
            {
                """["AllOne","inc","getMinKey"]""",
                """[[],["a"],[]]""",
                """[null,null,""]"""
            },
            // inc answers nothing.
            {
                """["AllOne","inc","getMaxKey"]""",
                """[[],["a"],[]]""",
                """[null,"a","a"]"""
            },
            // One answer short of the calls.
            {
                """["AllOne","inc","getMaxKey"]""",
                """[[],["a"],[]]""",
                """[null,null]"""
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

        return AllOneDataStructureAnswerCheck.IsAccepted(names, argumentArray, answerArray, answerArray);
    }

    private static JsonArray ArrayOf(string notation) =>
        JsonNode.Parse(notation) as JsonArray ?? throw new ArgumentException("The notation is no array.", nameof(notation));
}
