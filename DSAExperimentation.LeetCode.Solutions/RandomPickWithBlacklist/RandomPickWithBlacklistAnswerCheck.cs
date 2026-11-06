using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.RandomPickWithBlacklist;

// LC 710 accepts, for each pick, any number in [0, n) not in the blacklist, chosen at random, so no script's
// answer can match a recording. A script's answer stands when it answers every call, the constructor with
// null and each pick with a number in [0, n) the blacklist does not hold. Picks change nothing, so each is
// judged on its own; the recorded answer is one of the scripts that stand, and does not take part.
internal static class RandomPickWithBlacklistAnswerCheck
{
    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        var constructorInputs = arguments[0] as JsonArray;

        if (answer.Count != calls.Length || constructorInputs is not [JsonValue rangeSize, JsonArray blacklist])
        {
            return false;
        }

        var range = rangeSize.GetValue<int>();
        var barred = blacklist.GetValues<int>().ToHashSet();
        var picks = Enumerable.Range(1, calls.Length - 1);
        return answer[0] is null && picks.All(pick => IsWhitelisted(range, barred, answer[pick]));
    }

    // Whether a pick answered a number in [0, range) that is not barred.
    private static bool IsWhitelisted(int range, HashSet<int> barred, JsonNode? picked) =>
        NumberBelow(range, picked) is { } number && !barred.Contains(number);

    // The number an answer names in [0, bound); null when it names none - not a whole number, or out of range.
    private static int? NumberBelow(int bound, JsonNode? answer)
    {
        if (answer is not JsonValue value || !value.TryGetValue<int>(out var number))
        {
            return null;
        }

        var isInRange = number >= 0 && number < bound;
        return isInRange ? number : null;
    }
}
