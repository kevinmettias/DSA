using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Harness;

// Judges a strategy's answers, written by AnswerNotation, against the outputs the catalog records for its
// cases, by the question's order rule. A case with no recorded output is not judged: it is still a
// LeetCode input to time, but nothing vouches for an answer to it. Both harnesses judge this way - the
// solutions' tests a strategy called case by case, the benchmarks' tests a generated arm's buffer.
internal static class CatalogJudge
{
    private const string InputSeparator = " | ";
    private const string BareNullOutput = "A recorded output is never a bare null.";
    private const string NullText = "null";

    // One line per case answered wrongly, naming the arm, the case's inputs, the recorded output and the
    // answer; empty when every case with a recorded output was answered as recorded.
    public static IReadOnlyList<string> WrongAnswers(
        string arm,
        LeetCodeAnswerOrder order,
        IReadOnlyList<LeetCodeCase> cases,
        Func<int, JsonNode> answerTo)
    {
        var wrong = new List<string>();

        for (var index = 0; index < cases.Count; index++)
        {
            if (cases[index].ExpectedOutput is not { } recorded)
            {
                continue;
            }

            var expected = JsonNode.Parse(recorded) ?? throw new InvalidOperationException(BareNullOutput);
            var actual = answerTo(index);

            if (!IsAccepted(actual, expected, order))
            {
                wrong.Add($"{arm} on {string.Join(InputSeparator, cases[index].Inputs)}: the catalog records {expected.ToJsonString()}, and it answered {actual.ToJsonString()}.");
            }
        }

        return wrong;
    }

    // Exactly as LeetCode compares, or with the answer's top-level elements in any order; each element is
    // compared as written either way.
    public static bool IsAccepted(JsonNode actual, JsonNode expected, LeetCodeAnswerOrder order)
    {
        var written = JsonNode.Parse(actual.ToJsonString());

        if (order != LeetCodeAnswerOrder.AnyOrder)
        {
            return JsonNode.DeepEquals(written, expected);
        }

        // Only a sequence answer has an order to free: a number or a string is compared as written whatever
        // the question says.
        if (written is not JsonArray answered || expected is not JsonArray wanted)
        {
            return JsonNode.DeepEquals(written, expected);
        }

        return SortedTextOf(answered).SequenceEqual(SortedTextOf(wanted), StringComparer.Ordinal);
    }

    private static IEnumerable<string> SortedTextOf(JsonArray items) =>
        items.Select(item => item?.ToJsonString() ?? NullText).Order(StringComparer.Ordinal);
}
