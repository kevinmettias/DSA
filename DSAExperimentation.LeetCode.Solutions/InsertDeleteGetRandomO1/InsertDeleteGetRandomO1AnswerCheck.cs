using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.InsertDeleteGetRandomO1;

// LC 380's getRandom "returns a random element from the current set of elements", so LeetCode accepts any
// member there, while insert and remove each have one answer: whether the value was absent, and whether it
// was present. An answer to a script stands when it has one entry per call, null for the constructor, each
// insert and remove answers what the set - replayed here from the script - says, and each getRandom answers
// a value the set holds at that call. The recorded draws are one sequence of many that stand, so the
// recorded answer takes no part: the list scan and the swap-remove arm keep their members in different
// orders, and so draw different ones from the same seed.
internal static class InsertDeleteGetRandomO1AnswerCheck
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

        var members = new HashSet<int>();

        for (var call = 1; call < calls.Length; call++)
        {
            if (!IsAnsweredAsHeld(calls[call], arguments[call], answer[call], members))
            {
                return false;
            }
        }

        return true;
    }

    // One call replayed on the set: insert and remove change it and answer whether they did; getRandom reads it.
    private static bool IsAnsweredAsHeld(string call, JsonNode? callArguments, JsonNode? given, HashSet<int> members)
    {
        switch (call)
        {
            case InsertCall:
            case RemoveCall:
                var isChanged = IsChangedOnReplay(members, call, callArguments);
                var expected = JsonValue.Create(isChanged);
                return JsonNode.DeepEquals(given, expected);
            case GetRandomCall:
                return IsMember(given, members);
            default:
                return false;
        }
    }

    // Replays the call and says whether it changed the set: insert adds a value that was absent, remove takes
    // away one present.
    private static bool IsChangedOnReplay(HashSet<int> members, string call, JsonNode? callArguments)
    {
        var value = ValueOf(callArguments);
        return call == InsertCall ? members.Add(value) : members.Remove(value);
    }

    // The one value an insert or remove call names.
    private static int ValueOf(JsonNode? callArguments) =>
        callArguments?[0]?.GetValue<int>() ?? throw new ArgumentException("An insert or remove call names no value.", nameof(callArguments));

    // LeetCode promises the set holds a value whenever getRandom is called.
    private static bool IsMember(JsonNode? given, HashSet<int> members)
    {
        if (given is not JsonValue value || !value.TryGetValue<int>(out var drawn))
        {
            return false;
        }

        return members.Contains(drawn);
    }
}
