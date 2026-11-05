using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;

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
    private const string SequenceStart = "[";
    private const string SequenceEnd = "]";
    private const string ElementSeparator = ",";
    private const string SignificantDigits = "G12";

    // One line per case answered wrongly, naming the arm, the case's inputs, the recorded output and the
    // answer; empty when every case with a recorded output was answered as recorded.
    public static IReadOnlyList<string> WrongAnswers(
        string arm,
        LeetCodeQuestionAttribute question,
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

            if (!IsAccepted(actual, expected, question.AnswerOrder, question.Tolerance))
            {
                wrong.Add($"{arm} on {string.Join(InputSeparator, cases[index].Inputs)}: the catalog records {expected.ToJsonString()}, and it answered {actual.ToJsonString()}.");
            }
        }

        return wrong;
    }

    // Exactly as LeetCode compares, or with whatever the order rule frees put in one order on both sides: the
    // answer's elements, each element's own elements, or both. Under a tolerance, each number may stand that
    // far from LeetCode's, in the order written.
    public static bool IsAccepted(JsonNode actual, JsonNode expected, LeetCodeAnswerOrder order, double tolerance = 0)
    {
        var written = JsonNode.Parse(actual.ToJsonString());

        if (tolerance > 0)
        {
            return IsWithin(written, expected, tolerance);
        }

        if (order == LeetCodeAnswerOrder.AsWritten)
        {
            return JsonNode.DeepEquals(written, expected);
        }

        // Only a sequence answer has an order to free: a number or a string is compared as written whatever
        // the question says.
        if (written is not JsonArray answered || expected is not JsonArray wanted)
        {
            return JsonNode.DeepEquals(written, expected);
        }

        return FreedTextOf(answered, order) == FreedTextOf(wanted, order);
    }

    // The text two strategies' answers share exactly when each is the other's answer by the question's
    // order rule, for holding arms to one another where no recorded output stands between them: a sequence is
    // written with what its order rule frees sorted, anything else as it is.
    // Under a tolerance, numbers are written to twelve significant digits, as AnswerGraphText writes a double:
    // two arms that order the same operations differently part only in the last few bits.
    public static string AgreementTextOf(JsonNode answer, LeetCodeAnswerOrder order, double tolerance = 0)
    {
        var compared = answer;

        if (tolerance > 0)
        {
            compared = RoundedOf(answer) ?? answer;
        }

        if (order == LeetCodeAnswerOrder.AsWritten || compared is not JsonArray items)
        {
            return compared.ToJsonString();
        }

        return FreedTextOf(items, order);
    }

    private static JsonNode? RoundedOf(JsonNode? node) => node switch
    {
        JsonArray items => new JsonArray([.. items.Select(RoundedOf)]),
        _ when NumberOf(node) is { } number => JsonValue.Create(Rounded(number)),
        _ => node?.DeepClone(),
    };

    private static double Rounded(double number)
    {
        var significant = number.ToString(SignificantDigits, CultureInfo.InvariantCulture);

        return double.Parse(significant, CultureInfo.InvariantCulture);
    }

    // Each number at most the tolerance from LeetCode's, arrays element for element, anything else as written.
    private static bool IsWithin(JsonNode? actual, JsonNode? expected, double tolerance)
    {
        if (actual is JsonArray answered && expected is JsonArray wanted)
        {
            return answered.Count == wanted.Count && answered.Zip(wanted).All(pair => IsWithin(pair.First, pair.Second, tolerance));
        }

        if (NumberOf(actual) is { } answer && NumberOf(expected) is { } recorded)
        {
            return Math.Abs(answer - recorded) <= tolerance;
        }

        return JsonNode.DeepEquals(actual, expected);
    }

    private static double? NumberOf(JsonNode? node)
    {
        if (node is not JsonValue value || value.GetValueKind() != JsonValueKind.Number)
        {
            return null;
        }

        return value.GetValue<double>();
    }

    // The sequence's text with what its order rule frees put in one order: its elements sorted under AnyOrder,
    // each element's own elements sorted under EachInAnyOrder, both under AnyOrderThroughout.
    private static string FreedTextOf(JsonArray items, LeetCodeAnswerOrder order)
    {
        var elements = items.Select(item => ElementTextOf(item, order));

        if (order is LeetCodeAnswerOrder.AnyOrder or LeetCodeAnswerOrder.AnyOrderThroughout)
        {
            elements = elements.Order(StringComparer.Ordinal);
        }

        return $"{SequenceStart}{string.Join(ElementSeparator, elements)}{SequenceEnd}";
    }

    private static string ElementTextOf(JsonNode? item, LeetCodeAnswerOrder order)
    {
        if (order is LeetCodeAnswerOrder.EachInAnyOrder or LeetCodeAnswerOrder.AnyOrderThroughout && item is JsonArray inner)
        {
            return FreedTextOf(inner, LeetCodeAnswerOrder.AnyOrder);
        }

        return item?.ToJsonString() ?? NullText;
    }
}
