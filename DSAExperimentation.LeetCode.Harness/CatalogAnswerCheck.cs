using System.Reflection;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog;

namespace DSAExperimentation.LeetCode.Harness;

// Asks a question's answer check (LeetCodeQuestionAttribute.AnswerCheck) whether it accepts an answer: the
// case's inputs, the answer and the recorded answer are each read from LeetCode's notation into the types
// its IsAccepted takes. Where a statement lets several answers stand, the catalog records one of them, so the
// recorded answer says what any accepted answer must achieve - its length, its size, whether there is one
// at all - rather than what it must be.
internal static class CatalogAnswerCheck
{
    // The method an answer check states its rule with.
    private const string CheckMethod = "IsAccepted";

    // The answer and the recorded answer, which IsAccepted takes after the case's inputs.
    private const int AnswerParameters = 2;

    private const string Accepted = "accepted";
    private const string Refused = "refused";
    private const string NotJudged = "not judged";

    public static bool IsAccepted(Type check, LeetCodeCase testCase, JsonNode answer, JsonNode recorded)
    {
        var method = check.GetMethod(CheckMethod, BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"{check.Name} has no public static {CheckMethod} method.");
        var types = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
        var texts = testCase.Inputs.Take(types.Length - AnswerParameters).Append(answer.ToJsonString()).Append(recorded.ToJsonString());
        var arguments = texts.Zip(types, NotationReader.Read).ToArray();

        return method.Invoke(null, arguments) is true;
    }

    // Two strategies may give different answers that the check accepts, so they agree when it takes the same
    // view of each: this text says whether it accepts the answer, or that the case, recording no output, is
    // not judged.
    public static string VerdictTextOf(Type check, LeetCodeCase testCase, JsonNode answer)
    {
        if (CatalogJudge.RecordedOutputOf(testCase) is not { } recorded)
        {
            return NotJudged;
        }

        var isAccepted = IsAccepted(check, testCase, answer, recorded);
        return isAccepted ? Accepted : Refused;
    }
}
