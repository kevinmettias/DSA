using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.LinkedListRandomNode;

// LC 382's getRandom "chooses a node randomly from the list and returns its value", so LeetCode accepts any
// value the list holds at every call. An answer to a script stands when it has one entry per call, null for
// the constructor, and each getRandom answers a value of the list the constructor was given - the first
// argument of the script's first call. The recorded draws are one sequence of many that stand, so the
// recorded answer takes no part: reservoir sampling and the cached array spend the same seed differently,
// and so draw different values.
internal static class LinkedListRandomNodeAnswerCheck
{
    private const string GetRandomCall = "getRandom";

    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        if (answer.Count != calls.Length || answer[0] is not null)
        {
            return false;
        }

        var values = ListValuesOf(arguments[0]);
        var draws = answer.Skip(1);

        return calls.Skip(1).Zip(draws).All(pair => IsDrawnFrom(pair.First, pair.Second, values));
    }

    // The values of the list the constructor takes, written as an array of node values.
    private static HashSet<int> ListValuesOf(JsonNode? constructorArguments)
    {
        var list = constructorArguments?[0]?.AsArray() ?? throw new ArgumentException("The constructor names no list.", nameof(constructorArguments));

        return [.. list.Select(value => value?.GetValue<int>() ?? throw new ArgumentException("A node of the list holds no value.", nameof(constructorArguments)))];
    }

    // A getRandom call answered with a value of the list.
    private static bool IsDrawnFrom(string call, JsonNode? given, HashSet<int> values)
    {
        if (call != GetRandomCall || given is not JsonValue value)
        {
            return false;
        }

        return value.TryGetValue<int>(out var drawn) && values.Contains(drawn);
    }
}
