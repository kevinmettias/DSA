using System.Text.Json;

namespace DSAExperimentation.Tests.LeetCodeCatalog.Validation;

// IsMatch does row-by-row SequenceEqual, not EqualityComparer<int[][]>.Default
// (reference equality all the way down for a jagged array) - see
// ILeetCodeTestCaseAdapter's own doc comment for why a shared default would be
// silently wrong for any collection-shaped output.
internal readonly struct MergeIntervalsAdapter : ILeetCodeTestCaseAdapter<int[][], int[][]>
{
    // presumption: allow -- rawInput/rawExpectedOutput always come from a
    // LeetCodeQuestion cached for "merge-intervals" specifically, whose example
    // lines are always a valid JSON 2D integer array by LeetCodeQuestionMapper's
    // own construction.
    public int[][] ParseInput(string rawInput) => JsonSerializer.Deserialize<int[][]>(rawInput)!;

    public int[][] ParseExpectedOutput(string rawExpectedOutput) => JsonSerializer.Deserialize<int[][]>(rawExpectedOutput)!;

    public bool IsMatch(int[][] actual, int[][] expected)
        => actual.Length == expected.Length && actual.Zip(expected).All(pair => pair.First.SequenceEqual(pair.Second));
}
