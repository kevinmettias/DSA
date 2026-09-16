using DSAExperimentation.LeetCode.MinimumCostToConvertStringI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToConvertStringI;

// Harness only. The 26-letter conversion graph is
// LeetCode.MinimumCostToConvertStringI.LetterNetwork and both Floyd-Warshall
// strategies are MinimumCostToConvertStringISolution's - this file just pins
// them to LeetCode's published examples, including the unreachable-character
// case the baseline's raw matrix and the graph strategy's distance
// dictionary both have to report as -1 without ever agreeing on a magic
// in-between value.
public sealed class MinimumCostToConvertStringITests
{
    public static TheoryData<ConversionExample> Examples =>
        new()
        {
            {
                new ConversionExample(
                    Source: "abcd",
                    Target: "acbe",
                    Original: ['a', 'b', 'c', 'c', 'e', 'd'],
                    Changed: ['b', 'c', 'b', 'e', 'b', 'e'],
                    Cost: [2, 5, 5, 1, 2, 20],
                    Expected: 28)
            },
            {
                new ConversionExample(
                    Source: "aaaa",
                    Target: "bbbb",
                    Original: ['a', 'c'],
                    Changed: ['c', 'b'],
                    Cost: [1, 2],
                    Expected: 12)
            },
            {
                new ConversionExample(
                    Source: "abcd",
                    Target: "abce",
                    Original: ['a'],
                    Changed: ['e'],
                    Cost: [10000],
                    Expected: -1)
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByBruteForceFloydWarshall_LeetCodeExamples_ReturnsMinimumConversionCost(
        ConversionExample example)
    {
        var actual = MinimumCostToConvertStringISolution.MinimumCostByBruteForceFloydWarshall(
            new MinimumCostToConvertStringISolution.SourceText(example.Source),
            new MinimumCostToConvertStringISolution.TargetText(example.Target),
            (example.Original, example.Changed, example.Cost));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByAllPairsShortestPaths_LeetCodeExamples_ReturnsMinimumConversionCost(
        ConversionExample example)
    {
        var actual = MinimumCostToConvertStringISolution.MinimumCostByAllPairsShortestPaths(
            new MinimumCostToConvertStringISolution.SourceText(example.Source),
            new MinimumCostToConvertStringISolution.TargetText(example.Target),
            (example.Original, example.Changed, example.Cost));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the conversion asked for, the reachable character edits
    // that may serve it, and the cost LeetCode publishes. Named fields rather than
    // six positional arguments, so `Source` and `Target` - the same `string` type,
    // and the two whose transposition silently asks the reverse question - state
    // which role each plays.
    public readonly record struct ConversionExample(
        string Source,
        string Target,
        char[] Original,
        char[] Changed,
        int[] Cost,
        long Expected);
}
