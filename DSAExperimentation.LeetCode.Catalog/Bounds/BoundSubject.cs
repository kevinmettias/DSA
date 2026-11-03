using System.Text.Json;
using System.Text.RegularExpressions;
using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Bounds;

// Reads the values a bound's subject names out of a case's inputs. A subject is an input's name
// followed by accessors, each applied to every value the previous step produced:
//
//   [i], [j]   every element (a letter index ranges over the whole array)
//   [0], [1]   the element at that position
//   .length    the array's or string's length, and only as the last step
//
// so "grid[i][j]" is every cell, "words[i].length" every word's length and "intervals[i][0]" every
// interval's start. Anything else - a member other than length, an input the signature does not
// name, a value that is not a number at the end - means the subject cannot be read, and ValuesOf
// returns null so the bound reports NotJudged rather than a verdict it has no grounds for.
internal static partial class BoundSubject
{
    private const string LengthStep = ".length";

    public static IReadOnlyList<double>? ValuesOf(
        string subject, LeetCodeCase testCase, IReadOnlyList<LeetCodeParameter> parameters)
    {
        var name = Root().Match(subject).Value;
        var position = PositionOf(name, parameters);

        if (position is not { } index || index >= testCase.Inputs.Count)
        {
            return null;
        }

        var steps = Steps().Matches(subject).Select(step => step.Value).ToList();
        return ValuesIn(testCase.Inputs[index], steps);
    }

    [GeneratedRegex(@"^[A-Za-z_]\w*")]
    private static partial Regex Root();

    private static int? PositionOf(string name, IReadOnlyList<LeetCodeParameter> parameters)
    {
        for (var index = 0; index < parameters.Count; index++)
        {
            if (parameters[index].Name == name)
            {
                return index;
            }
        }

        return null;
    }

    [GeneratedRegex(@"\.[A-Za-z_]\w*|\[\w+\]")]
    private static partial Regex Steps();

    private static List<double>? ValuesIn(string input, List<string> steps)
    {
        try
        {
            using var document = JsonDocument.Parse(input);
            return NumbersAfter(document.RootElement, steps);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    // Every step but a trailing .length selects elements; the trailing .length, if any, measures
    // what they selected.
    private static List<double>? NumbersAfter(JsonElement root, List<string> steps)
    {
        if (steps.Count == 0 || steps[^1] != LengthStep)
        {
            return NumbersSelectedBy(root, steps);
        }

        var selections = steps[..^1];
        return LengthsSelectedBy(root, selections);
    }

    private static List<double>? NumbersSelectedBy(JsonElement root, List<string> selections)
    {
        if (Selected(root, selections) is not { } selected)
        {
            return null;
        }

        return NumbersOf(selected);
    }

    private static List<double>? NumbersOf(List<JsonElement> elements)
    {
        if (elements.Exists(element => element.ValueKind != JsonValueKind.Number))
        {
            return null;
        }

        return elements.Select(element => element.GetDouble()).ToList();
    }

    private static List<double>? LengthsSelectedBy(JsonElement root, List<string> selections)
    {
        if (Selected(root, selections) is not { } selected)
        {
            return null;
        }

        return LengthsOf(selected);
    }

    private static List<double>? LengthsOf(List<JsonElement> elements)
    {
        var lengths = new List<double>(elements.Count);

        foreach (var element in elements)
        {
            if (LengthOf(element) is not { } length)
            {
                return null;
            }

            lengths.Add(length);
        }

        return lengths;
    }

    // An array's element count or a string's character count; null for anything that has neither.
    private static double? LengthOf(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Array => element.GetArrayLength(),
        JsonValueKind.String => StringLengthOf(element),
        _ => null,
    };

    // presumption: allow -- LengthOf calls this only for an element whose ValueKind is String,
    // whose GetString is never null.
    private static double StringLengthOf(JsonElement element) => element.GetString()!.Length;

    private static List<JsonElement>? Selected(JsonElement root, List<string> selections)
    {
        List<JsonElement>? current = [root];

        foreach (var step in selections)
        {
            current = ElementsAt(current, step);

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    private static List<JsonElement>? ElementsAt(List<JsonElement> elements, string step)
    {
        if (!step.StartsWith('[') || elements.Exists(element => element.ValueKind != JsonValueKind.Array))
        {
            return null;
        }

        if (int.TryParse(step[1..^1], out var fixedIndex))
        {
            return ElementsAtIndex(elements, fixedIndex);
        }

        return elements.SelectMany(element => element.EnumerateArray()).ToList();
    }

    private static List<JsonElement> ElementsAtIndex(List<JsonElement> elements, int index)
        => elements.Where(element => index < element.GetArrayLength()).Select(element => element[index]).ToList();
}
