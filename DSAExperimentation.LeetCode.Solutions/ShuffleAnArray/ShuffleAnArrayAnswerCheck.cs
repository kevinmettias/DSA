using System.Text.Json;
using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.ShuffleAnArray;

// LC 384's shuffle() returns "a random shuffling of the array", so any permutation of
// nums stands for it, while reset() returns nums as the constructor was handed it, its
// one answer. The check replays the script: the constructor's nums, then each call's
// answer against them, null for the constructor itself. The recorded answer is one
// random draw among many, so it does not take part.
internal static class ShuffleAnArrayAnswerCheck
{
    private const string Reset = "reset";
    private const string Shuffle = "shuffle";

    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        var isOneAnswerPerCall = answer.Count == calls.Length && answer[0] is null;

        if (!isOneAnswerPerCall || NumsOf(arguments) is not { } nums)
        {
            return false;
        }

        int[] ordered = [.. nums.Order()];

        return Enumerable.Range(1, calls.Length - 1).All(call => IsAcceptedCall(calls[call], nums, ordered, answer[call]));
    }

    // The constructor's one argument, nums.
    private static int[]? NumsOf(JsonArray arguments)
    {
        if (arguments[0] is not (JsonArray and [JsonArray nums]))
        {
            return null;
        }

        return nums.Deserialize<int[]>();
    }

    private static bool IsAcceptedCall(string call, int[] nums, int[] ordered, JsonNode? answered)
    {
        if (answered is not JsonArray values || values.Deserialize<int[]>() is not { } returned)
        {
            return false;
        }

        return call switch
        {
            Reset => returned.SequenceEqual(nums),
            Shuffle => returned.Order().SequenceEqual(ordered),
            _ => false,
        };
    }
}
