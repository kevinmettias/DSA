using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.InsertDeleteGetRandomO1DuplicatesAllowed;

// LC 381's getRandom "returns a random element from the current multiset of elements", so LeetCode accepts
// any value the multiset holds there, while insert and remove each have one answer: insert whether the
// value was absent before it went in, remove whether one was present to take away. An answer to a script
// stands when it has one entry per call, null for the constructor, each insert and remove answers what the
// multiset - replayed here from the script as a count per value - says, and each getRandom answers a value
// held at that call. The recorded draws are one sequence of many that stand, so the recorded answer takes no
// part: the list scan and the linked-occurrences arm keep their values in different orders, and so draw
// different ones from the same seed.
internal static class InsertDeleteGetRandomO1DuplicatesAllowedAnswerCheck
{
    private const string InsertCall = "insert";
    private const string RemoveCall = "remove";
    private const string GetRandomCall = "getRandom";

    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        if (answer.Count != calls.Length || answer[0] is not null)
        {
            return false;
        }

        var counts = new Dictionary<int, int>();

        for (var call = 1; call < calls.Length; call++)
        {
            if (!IsAnsweredAsHeld(calls[call], arguments[call], answer[call], counts))
            {
                return false;
            }
        }

        return true;
    }

    // One call replayed on the counts: insert and remove change them and answer whether the value was held
    // before; getRandom reads them.
    private static bool IsAnsweredAsHeld(string call, JsonNode? callArguments, JsonNode? given, Dictionary<int, int> counts)
    {
        switch (call)
        {
            case InsertCall:
            case RemoveCall:
                var isAnsweredTrue = IsAnsweredTrueOnReplay(counts, call, callArguments);
                var expected = JsonValue.Create(isAnsweredTrue);
                return JsonNode.DeepEquals(given, expected);
            case GetRandomCall:
                return IsHeld(given, counts);
            default:
                return false;
        }
    }

    // Replays the call on the counts and says whether LeetCode's answer to it is true: an insert adds one to
    // its value's count and answers whether the value was absent; a remove takes one away when the value is
    // held, a count that falls to 0 leaving the multiset, and answers whether it was.
    private static bool IsAnsweredTrueOnReplay(Dictionary<int, int> counts, string call, JsonNode? callArguments)
    {
        var value = ValueOf(callArguments);
        var count = counts.GetValueOrDefault(value);

        if (call == InsertCall)
        {
            counts[value] = count + 1;
            return count == 0;
        }

        if (count <= 1)
        {
            counts.Remove(value);
        }
        else
        {
            counts[value] = count - 1;
        }

        return count > 0;
    }

    // The one value an insert or remove call names.
    private static int ValueOf(JsonNode? callArguments) =>
        callArguments?[0]?.GetValue<int>() ?? throw new ArgumentException("An insert or remove call names no value.", nameof(callArguments));

    // LeetCode promises the multiset holds a value whenever getRandom is called.
    private static bool IsHeld(JsonNode? given, Dictionary<int, int> counts)
    {
        if (given is not JsonValue value || !value.TryGetValue<int>(out var drawn))
        {
            return false;
        }

        return counts.ContainsKey(drawn);
    }
}
