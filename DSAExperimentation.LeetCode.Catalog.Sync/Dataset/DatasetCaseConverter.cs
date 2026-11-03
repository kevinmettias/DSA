using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Text.Json;
using DSAExperimentation.LeetCode.Catalog.Bounds;
using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// Turns one question's LeetCodeDataset cases into catalog cases, keeping only those that pose the
// question as LeetCode does. The dataset's inputs were generated, not drawn from LeetCode's judge,
// and its outputs are what one Python reference solution printed, so a case is dropped (and the
// reason counted) when:
//
// - the reference solution failed on it, so the "output" is an error message;
// - its inputs cannot be matched to the signature's by name, or read as literals;
// - the reference returned None where the question's answer cannot be empty - it fell off the end,
//   which happens when the input breaks a promise (Two Sum's "exactly one solution") that no
//   parsed constraint states;
// - an input lies outside a bound the statement's constraints state;
// - it repeats an example or an earlier case.
//
// For a question that answers by changing its input, the reference returned None and the dataset
// recorded nothing about the changed input, so such a case keeps its inputs and a null answer.
//
// The dataset writes a string answer as the text itself, not as a Python literal - "Alice", or
// nothing at all for the empty string - while every other answer is a literal. Measured over the
// 5,370 string and character answers of the covered questions, none was quoted, so a string answer
// is taken as text and everything else is translated.
//
// Each step that can drop a case records why and returns null, so ConvertOne reads as the order
// the checks run in. One converter serves one question: the duplicates it remembers and the drops
// it counts are that question's.
internal sealed class DatasetCaseConverter
{
    private const string NullNotation = "null";
    private const string PythonNone = "None";
    private const string EmptyNodeNotation = "[]";

    // How the reference solution's failures begin: a raised error, or the run's time limit.
    private static readonly string[] FailurePrefixes = ["Error", "Execution timed out"];

    // The answer types the dataset prints as bare text.
    private static readonly string[] TextTypes = ["string", "character"];

    private static readonly JsonSerializerOptions TextEncoding = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    private readonly LeetCodeSignature _signature;
    private readonly IReadOnlyList<LeetCodeBound> _bounds;
    private readonly HashSet<string> _seenInputs;
    private readonly Dictionary<DatasetDropReason, int> _dropped = [];

    private bool IsTextAnswer => TextTypes.Contains(_signature.ReturnType);

    public DatasetCaseConverter(LeetCodeQuestion question)
    {
        _signature = question.Signature;
        _bounds = LeetCodeBounds.Of(question.Constraints);
        _seenInputs = question.Examples.Select(KeyOf).OfType<string>().ToHashSet(StringComparer.Ordinal);
    }

    public DatasetConversion Convert(IReadOnlyList<DatasetCaseDto> cases)
    {
        var kept = new List<LeetCodeCase>(cases.Count);

        foreach (var datasetCase in cases)
        {
            if (ConvertOne(datasetCase) is { } testCase)
            {
                kept.Add(testCase);
            }
        }

        return new DatasetConversion(kept, _dropped);
    }

    private LeetCodeCase? ConvertOne(DatasetCaseDto datasetCase)
    {
        if (AnswerOf(datasetCase.Output) is not { } answer || InputsOf(datasetCase.Input) is not { } inputs)
        {
            return null;
        }

        var testCase = new LeetCodeCase { Inputs = inputs, ExpectedOutput = answer.Notation, Source = LeetCodeCaseSource.Dataset };
        return IsAdmitted(testCase) ? testCase : null;
    }

    private DatasetAnswer? AnswerOf(string? output)
    {
        if (output is null || IsReferenceFailure(output))
        {
            return Dropped<DatasetAnswer?>(DatasetDropReason.ReferenceFailed);
        }

        if (!TryAnswerNotation(output, out var notation))
        {
            return Dropped<DatasetAnswer?>(DatasetDropReason.NotTranslatable);
        }

        if (!TryExpectedFrom(notation, out var expected))
        {
            return Dropped<DatasetAnswer?>(DatasetDropReason.NoAnswer);
        }

        return new DatasetAnswer(expected);
    }

    private static bool IsReferenceFailure(string output)
        => FailurePrefixes.Any(prefix => output.StartsWith(prefix, StringComparison.Ordinal));

    // A text answer is encoded as the string it is, except None, which is a reference that returned
    // nothing and is judged by TryExpectedFrom like any other None.
    private bool TryAnswerNotation(string output, out string notation)
    {
        if (!IsTextAnswer || output == PythonNone)
        {
            return PythonLiteral.TryToNotation(output, out notation);
        }

        notation = JsonSerializer.Serialize(output, TextEncoding);
        return true;
    }

    // None is an answer only where the answer can be empty (a tree or list, which LeetCode writes
    // as "[]") or where the question answers through an input, whose changed value the dataset
    // never recorded.
    private bool TryExpectedFrom(string answer, out string? expected)
    {
        expected = answer;

        if (answer != NullNotation)
        {
            return true;
        }

        var returnType = _signature.ReturnType ?? LeetCodeTypeNames.Void;
        expected = LeetCodeTypeNames.NodeTypes.Contains(returnType) ? EmptyNodeNotation : null;
        return expected is not null || returnType == LeetCodeTypeNames.Void;
    }

    private List<string>? InputsOf(string input)
    {
        if (DatasetInput.Split(input) is not { } assignments)
        {
            return Dropped<List<string>>(DatasetDropReason.InputNotSplit);
        }

        if (InSignatureOrder(assignments) is not { } values)
        {
            return Dropped<List<string>>(DatasetDropReason.NamesDiffer);
        }

        return Translated(values) ?? Dropped<List<string>>(DatasetDropReason.NotTranslatable);
    }

    private List<string>? InSignatureOrder(IReadOnlyList<(string Name, string Value)> assignments)
    {
        var names = _signature.Parameters.Select(parameter => parameter.Name).ToList();
        var byName = assignments
            .GroupBy(assignment => assignment.Name, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First().Value, StringComparer.Ordinal);

        if (byName.Count != assignments.Count || !IsExactlyNamed(byName, names))
        {
            return null;
        }

        return names.Select(name => byName[name]).ToList();
    }

    private static bool IsExactlyNamed(Dictionary<string, string> byName, List<string> names)
    {
        var assigned = byName.Keys.ToHashSet(StringComparer.Ordinal);
        return assigned.SetEquals(names);
    }

    private static List<string>? Translated(List<string> values)
    {
        var inputs = new List<string>(values.Count);

        foreach (var value in values)
        {
            if (!PythonLiteral.TryToNotation(value, out var notation))
            {
                return null;
            }

            inputs.Add(notation);
        }

        return inputs;
    }

    private bool IsAdmitted(LeetCodeCase testCase)
    {
        if (IsOutOfBounds(testCase))
        {
            return Dropped<bool>(DatasetDropReason.OutOfBounds);
        }

        if (KeyOf(testCase) is not { } key || !_seenInputs.Add(key))
        {
            return Dropped<bool>(DatasetDropReason.Duplicate);
        }

        return true;
    }

    private bool IsOutOfBounds(LeetCodeCase testCase)
        => _bounds.Any(bound => bound.Check(testCase, _signature.Parameters) == LeetCodeBoundCheck.Violated);

    // Two cases are the same case when their inputs are the same values, however each source
    // spaced or escaped them; null when an input is not JSON at all, which only an example can be.
    private static string? KeyOf(LeetCodeCase testCase)
    {
        try
        {
            var canonical = testCase.Inputs.Select(input => JsonNode.Parse(input)?.ToJsonString() ?? NullNotation);
            return string.Join(LeetCodeNotation.InputSeparator, canonical);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private TValue? Dropped<TValue>(DatasetDropReason reason)
    {
        _dropped[reason] = _dropped.GetValueOrDefault(reason) + 1;
        return default;
    }
}
