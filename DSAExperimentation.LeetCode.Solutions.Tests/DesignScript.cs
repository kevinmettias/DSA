using System.Text.Json;
using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.Tests;

// A design question's script as LeetCode writes it: the calls by name, each call's arguments, and the answer
// LeetCode recorded. Its answer check judges another answer against the script, so a test states the script and
// each answer in LeetCode's notation, and this reads them into the shapes the check takes before asking it.
internal sealed record DesignScript(string Calls, string Arguments, string Recorded)
{
    public bool IsAccepting(Func<string[], JsonArray, JsonArray, JsonArray, bool> answerCheck, string answer)
    {
        var callNames = JsonSerializer.Deserialize<string[]>(Calls) ?? throw new InvalidOperationException($"{Calls} names no calls.");
        var callArguments = ArrayOf(Arguments);
        var answered = ArrayOf(answer);
        var recorded = ArrayOf(Recorded);

        return answerCheck(callNames, callArguments, answered, recorded);
    }

    private static JsonArray ArrayOf(string notation) =>
        JsonNode.Parse(notation) as JsonArray ?? throw new ArgumentException($"{notation} is not a JSON array.", nameof(notation));
}
