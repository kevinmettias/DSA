using System.Text.Json;

namespace DSAExperimentation.Tests.LeetCodeCatalog.Validation;

// Two Sum's own raw test-case shape: two lines per example (nums, target), both
// already valid JSON literals - System.Text.Json parses each line directly, no
// hand-rolled tokenizer needed. IsMatch compares the returned pair
// ORDER-INDEPENDENTLY: LeetCode's own problem statement says "you can return the
// answer in any order," so the scraped example output (one specific order) isn't
// the only correct answer - only EqualityComparer<int[]>.Default (reference
// equality on arrays) would be wrong here, exactly the footgun
// ILeetCodeTestCaseAdapter's own doc comment warns a shared default would hide.
internal readonly struct TwoSumAdapter : ILeetCodeTestCaseAdapter<(int[] Nums, int Target), int[]>
{
    // presumption: allow -- rawInput always comes from a LeetCodeQuestion cached
    // for "two-sum" specifically, whose two example lines are always valid JSON
    // (a number array, then a number) by LeetCodeQuestionMapper's own construction.
    public (int[] Nums, int Target) ParseInput(string rawInput)
    {
        var lines = rawInput.Split('\n');
        return (JsonSerializer.Deserialize<int[]>(lines[0])!, JsonSerializer.Deserialize<int>(lines[1]));
    }

    // presumption: allow -- same as ParseInput's own reasoning above, for the
    // scraped expected-output line.
    public int[] ParseExpectedOutput(string rawExpectedOutput) => JsonSerializer.Deserialize<int[]>(rawExpectedOutput)!;

    public bool IsMatch(int[] actual, int[] expected) => actual.Order().SequenceEqual(expected.Order());
}
