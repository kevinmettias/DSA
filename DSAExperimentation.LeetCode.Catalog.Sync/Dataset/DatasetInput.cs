using System.Text.RegularExpressions;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// LeetCodeDataset writes a case's inputs the way LeetCode's statement does - "nums = [3,3],
// target = 6" - so splitting them means finding the commas that separate assignments rather than
// those inside a value. Such a comma is at nesting depth zero, outside every string, and followed
// by "name =" (and not "=="). The names are returned with the values so the importer can match
// them against the question's signature instead of trusting their order.
internal static partial class DatasetInput
{
    private const char Comma = ',';
    private const string NameGroup = "name";
    private const string ValueGroup = "value";

    public static IReadOnlyList<(string Name, string Value)>? Split(string input)
    {
        var assignments = new List<(string Name, string Value)>();

        foreach (var segment in TopLevelSegments(input))
        {
            var match = Assignment().Match(segment);

            if (!match.Success)
            {
                return null;
            }

            assignments.Add((match.Groups[NameGroup].Value, match.Groups[ValueGroup].Value.Trim()));
        }

        return assignments;
    }

    private static List<string> TopLevelSegments(string input)
    {
        var segments = new List<string>();
        var scanner = new NestingScanner();
        var start = 0;

        for (var index = 0; index < input.Length; index++)
        {
            scanner.Advance(input[index]);

            if (IsAssignmentBoundary(input, index, scanner))
            {
                segments.Add(input[start..index]);
                start = index + 1;
            }
        }

        segments.Add(input[start..]);
        return segments;
    }

    // A comma outside every value, with the next assignment's "name =" right after it.
    private static bool IsAssignmentBoundary(string input, int index, NestingScanner scanner)
    {
        if (!scanner.IsAtTopLevel || input[index] != Comma)
        {
            return false;
        }

        var rest = input[(index + 1)..];
        return Assignment().IsMatch(rest);
    }

    [GeneratedRegex(@"^\s*(?<name>[A-Za-z_]\w*)\s*=(?!=)(?<value>.*)$", RegexOptions.Singleline)]
    private static partial Regex Assignment();
}
