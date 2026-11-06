using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.RandomPickWithWeight;

// LC 528 accepts, for each pickIndex, any index of w drawn with probability w[i] / sum(w), so no script's
// answer can match a recording, and every index whose weight is positive may come up. A script's answer
// stands when it answers every call, the constructor with null and each pick with an index of w whose weight
// is positive. Picks change nothing, so each is judged on its own; the recorded answer is one of the scripts
// that stand, and does not take part.
internal static class RandomPickWithWeightAnswerCheck
{
    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        var constructorInputs = arguments[0] as JsonArray;

        if (answer.Count != calls.Length || constructorInputs is not [JsonArray weights])
        {
            return false;
        }

        var picks = Enumerable.Range(1, calls.Length - 1);
        return answer[0] is null && picks.All(pick => IsWeightedIndex(weights, answer[pick]));
    }

    // Whether a pick answered an index of the weights whose weight is positive.
    private static bool IsWeightedIndex(JsonArray weights, JsonNode? picked)
    {
        if (IndexAmong(weights.Count, picked) is not { } index || weights[index] is not JsonValue weight)
        {
            return false;
        }

        return weight.GetValue<int>() > 0;
    }

    // The index an answer names among count items; null when it names none - not a whole number, or out of range.
    private static int? IndexAmong(int count, JsonNode? answer)
    {
        if (answer is not JsonValue value || !value.TryGetValue<int>(out var index))
        {
            return null;
        }

        var isInRange = index >= 0 && index < count;
        return isInRange ? index : null;
    }
}
