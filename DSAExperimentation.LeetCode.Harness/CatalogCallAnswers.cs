using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Harness;

// What a generated design arm returns (ARCHITECTURE 17.13): each call's answer stays in the buffer of the
// method it called, typed as that method returns it, and is read back here as LeetCode writes a script's
// answer. It is public because a benchmark class, and so its arms' return types, must be.

/// <summary>
/// The answers of a generated arm for a design question: every call's return value, by case, in the buffer
/// of the method the call made.
/// </summary>
public sealed class CatalogCallAnswers
{
    private readonly int[][] _calls;
    private readonly Array?[] _buffersByMethod;

    internal CatalogCallAnswers(int[][] calls, Array?[] buffersByMethod)
    {
        _calls = calls;
        _buffersByMethod = buffersByMethod;
    }

    /// <summary>How many cases the answers are for.</summary>
    public int Count => _calls.Length;

    // One case's answer as LeetCode writes it: null for the constructor and for each call that returns
    // nothing, and each other call's return value in LeetCode's notation.
    internal JsonArray NotationOf(int caseIndex)
    {
        var script = _calls[caseIndex];
        var answers = new JsonArray();

        for (var call = 0; call < script.Length; call++)
        {
            answers.Add(AnswerOf(script[call], caseIndex, call));
        }

        return answers;
    }

    private JsonNode? AnswerOf(int method, int caseIndex, int call)
    {
        if (method == CatalogScripts.Constructor || _buffersByMethod[method] is not { } buffer)
        {
            return null;
        }

        var answers = (Array)buffer.GetValue(caseIndex)!;

        return AnswerNotation.Of(answers.GetValue(call));
    }
}
