using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.RandomPickIndex;

// LC 398 accepts, for each pick, any index whose value in nums is the pick's target, chosen at random, so no
// script's answer can match a recording. A script's answer stands when it answers every call, the constructor
// with null and each pick with an index of nums holding its target. Picks change nothing, so each is judged on
// its own; the recorded answer is one of the scripts that stand, and does not take part.
internal static class RandomPickIndexAnswerCheck
{
    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        var constructorInputs = arguments[0] as JsonArray;

        if (answer.Count != calls.Length || constructorInputs is not [JsonArray nums])
        {
            return false;
        }

        var picks = Enumerable.Range(1, calls.Length - 1);
        return answer[0] is null && picks.All(pick => IsIndexOfTarget(nums, arguments[pick], answer[pick]));
    }

    // Whether a pick answered an index of nums whose value is the target it was asked for.
    private static bool IsIndexOfTarget(JsonArray nums, JsonNode? pickArguments, JsonNode? picked)
    {
        var pickInputs = pickArguments as JsonArray;

        if (pickInputs is not [JsonValue target] || IndexAmong(nums.Count, picked) is not { } index)
        {
            return false;
        }

        return JsonNode.DeepEquals(nums[index], target);
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
