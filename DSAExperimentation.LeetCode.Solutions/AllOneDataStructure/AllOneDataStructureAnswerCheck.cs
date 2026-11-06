using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.AllOneDataStructure;

// LC 432's getMaxKey returns "one of the keys with the maximal count" and getMinKey one with the minimum,
// or "" when no key is held, so where keys tie LeetCode accepts any of them. An answer to a script stands
// when it has one entry per call, null for the constructor and for every inc and dec, and each getMaxKey or
// getMinKey names a key whose count - replayed here from the script's inc and dec calls - is the largest
// or the smallest any key holds. The recorded answer is one of those that stand, so it takes no part: the
// bucketed list answers with the key its bucket gained last, the dictionary scan with the first it meets.
internal static class AllOneDataStructureAnswerCheck
{
    private const string IncCall = "inc";
    private const string DecCall = "dec";
    private const string GetMaxKeyCall = "getMaxKey";
    private const string GetMinKeyCall = "getMinKey";

    // What getMaxKey and getMinKey answer when no key is held.
    private const string NoKey = "";

    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        if (answer.Count != calls.Length || answer[0] is not null)
        {
            return false;
        }

        var counts = new Dictionary<string, int>(StringComparer.Ordinal);

        for (var call = 1; call < calls.Length; call++)
        {
            if (!IsAnsweredAsCounted(calls[call], arguments[call], answer[call], counts))
            {
                return false;
            }
        }

        return true;
    }

    // One call replayed on the counts: inc and dec change them and answer nothing; getMaxKey and getMinKey
    // read them.
    private static bool IsAnsweredAsCounted(string call, JsonNode? callArguments, JsonNode? given, Dictionary<string, int> counts)
    {
        switch (call)
        {
            case IncCall:
            case DecCall:
                Recount(counts, call, callArguments);
                return given is null;
            case GetMaxKeyCall:
            case GetMinKeyCall:
                return IsKeyAtTheCountRead(given, counts, call);
            default:
                return false;
        }
    }

    // An inc adds one to its key's count, a dec takes one away; LeetCode promises a dec's key is held, and a
    // count that falls to 0 leaves the structure.
    private static void Recount(Dictionary<string, int> counts, string call, JsonNode? callArguments)
    {
        var key = KeyOf(callArguments);
        var count = counts.GetValueOrDefault(key);
        var step = call == IncCall ? 1 : -1;
        var recounted = count + step;

        if (recounted == 0)
        {
            counts.Remove(key);
        }
        else
        {
            counts[key] = recounted;
        }
    }

    // The one key an inc or dec call names.
    private static string KeyOf(JsonNode? callArguments) =>
        callArguments?[0]?.GetValue<string>() ?? throw new ArgumentException("An inc or dec call names no key.", nameof(callArguments));

    // A key held at the largest count for getMaxKey, at the smallest for getMinKey, or "" when no key is held.
    private static bool IsKeyAtTheCountRead(JsonNode? given, Dictionary<string, int> counts, string call)
    {
        if (given is not JsonValue value || !value.TryGetValue<string>(out var key))
        {
            return false;
        }

        if (counts.Count == 0)
        {
            return key == NoKey;
        }

        var countRead = call == GetMaxKeyCall ? counts.Values.Max() : counts.Values.Min();
        return counts.TryGetValue(key, out var held) && held == countRead;
    }
}
