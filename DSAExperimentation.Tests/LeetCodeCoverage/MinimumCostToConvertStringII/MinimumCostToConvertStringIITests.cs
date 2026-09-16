using DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumCostToConvertStringII;

// Harness only. The distinct-substring conversion graph is
// LeetCode.MinimumCostToConvertStringII.SubstringNetwork and both
// Floyd-Warshall strategies are MinimumCostToConvertStringIISolution's -
// this file just pins them to LeetCode's published examples: a
// single-character case identical to LC 2976's, a multi-character chain
// requiring two hops through one substring, and an unreachable case.
public sealed class MinimumCostToConvertStringIITests
{
    public static TheoryData<ConversionExample> Examples =>
        new()
        {
            {
                new ConversionExample(
                    Source: "abcd",
                    Target: "acbe",
                    Original: ["a", "b", "c", "c", "e", "d"],
                    Changed: ["b", "c", "b", "e", "b", "e"],
                    Cost: [2, 5, 5, 1, 2, 20],
                    Expected: 28)
            },
            {
                new ConversionExample(
                    Source: "abcdefgh",
                    Target: "acdeeghh",
                    Original: ["bcd", "fgh", "thh"],
                    Changed: ["cde", "thh", "ghh"],
                    Cost: [1, 3, 5],
                    Expected: 9)
            },
            {
                new ConversionExample(
                    Source: "abcdefgh",
                    Target: "addddddd",
                    Original: ["bcd", "defgh"],
                    Changed: ["ddd", "ddddd"],
                    Cost: [100, 1578],
                    Expected: -1)
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByBruteForceFloydWarshall_LeetCodeExamples_ReturnsMinimumConversionCost(
        ConversionExample example)
    {
        var actual = MinimumCostToConvertStringIISolution.MinimumCostByBruteForceFloydWarshall(
            new SourceText(example.Source), new TargetText(example.Target),
            (example.Original, example.Changed, example.Cost));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumCostByAllPairsShortestPaths_LeetCodeExamples_ReturnsMinimumConversionCost(
        ConversionExample example)
    {
        var actual = MinimumCostToConvertStringIISolution.MinimumCostByAllPairsShortestPaths(
            new SourceText(example.Source), new TargetText(example.Target),
            (example.Original, example.Changed, example.Cost));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the conversion asked for, the reachable substring edits
    // that may serve it, and the cost LeetCode publishes. Named fields rather than
    // six positional arguments, so `Source` and `Target` - the same `string` type,
    // and the two whose transposition silently asks the reverse question - state
    // which role each plays.
    public readonly record struct ConversionExample(
        string Source,
        string Target,
        string[] Original,
        string[] Changed,
        int[] Cost,
        long Expected);
}
