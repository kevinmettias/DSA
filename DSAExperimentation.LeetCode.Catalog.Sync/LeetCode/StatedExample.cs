using System.Text.Json;
using System.Text.Json.Nodes;
using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

// Whether an example's stored inputs are the ones its statement shows, so that the output the
// statement states may be recorded for them. LeetCode's exampleTestcaseList is usually the
// statement's examples in order, but for some questions it holds other inputs: Check if There is a
// Valid Path in a Grid stores three grids its statement never shows. Paired by position alone, those
// recorded the answers to other inputs.
//
// A function question states "nums = [3,3], target = 6"; a design question states its call script
// as bare values, the method names and then their arguments ("["MinStack","push"] [[],[-2]]"). A
// stated input that reads as neither, one value per stored input, cannot be confirmed, so it does
// not count as shown.
internal static class StatedExample
{
    private static readonly JsonReaderOptions ValueSequence = new() { AllowMultipleValues = true };

    public static bool HasInputs(string statedInput, string storedInputs)
    {
        var inputs = storedInputs.Split(LeetCodeNotation.InputSeparator);

        if (StatedValuesOf(statedInput) is not { } values || values.Count != inputs.Length)
        {
            return false;
        }

        return values.Zip(inputs).All(pair => IsSameValue(pair.First, pair.Second));
    }

    private static IReadOnlyList<string>? StatedValuesOf(string statedInput)
    {
        if (DatasetInput.Split(statedInput) is { } assignments)
        {
            return [.. assignments.Select(assignment => assignment.Value)];
        }

        return BareValuesOf(statedInput);
    }

    // Each top-level JSON value in turn; null when the text is not a sequence of them.
    private static List<string>? BareValuesOf(string statedInput)
    {
        var values = new List<string>();

        try
        {
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(statedInput), ValueSequence);

            while (reader.Read())
            {
                using var value = JsonDocument.ParseValue(ref reader);
                values.Add(value.RootElement.GetRawText());
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return values;
    }

    // The same value however each side spaces or escapes it; text that is not JSON compares as written.
    private static bool IsSameValue(string stated, string stored) =>
        string.Equals(CanonicalOf(stated), CanonicalOf(stored), StringComparison.Ordinal);

    private static string CanonicalOf(string value)
    {
        try
        {
            return JsonNode.Parse(value)?.ToJsonString() ?? value.Trim();
        }
        catch (JsonException)
        {
            return value.Trim();
        }
    }
}
