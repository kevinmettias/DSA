using DSAExperimentation.LeetCode.MinimumCostToConvertStringI;

namespace DSAExperimentation.LeetCode.Tests.MinimumCostToConvertStringI;

// Harness only. The 26-letter conversion graph is
// LeetCode.MinimumCostToConvertStringI.LetterNetwork and both Floyd-Warshall
// strategies are MinimumCostToConvertStringISolution's - this file just pins
// them to LeetCode's published examples, including the unreachable-character
// case the baseline's raw matrix and the graph strategy's distance
// dictionary both have to report as -1 without ever agreeing on a magic
// in-between value. The closed distance matrix the baseline is handed is asserted on
// its own, pair by reachable pair.
public sealed partial class MinimumCostToConvertStringISolutionTests
{
    // LeetCode's first rule set costs 2 + 5 + 5 + 1 + 2 + 20 = 35 in all. A shortest
    // chain never takes the same rule twice, so a reachable pair costs at most 35, and
    // any entry above that is the matrix's mark for "unreachable", whatever its value.
    private const long FirstExampleRuleCostSum = 35;

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

    // LeetCode's first rule set: a->b 2, b->c 5, c->b 5, c->e 1, e->b 2, d->e 20. Closed
    // under chaining, row by row: a reaches b at 2, c at 7 (via b) and e at 8 (via b, c);
    // b reaches c at 5 and e at 6 (via c); c reaches b at 3 (via e, under its own rule's
    // 5) and e at 1; d reaches b at 22, c at 27 and e at 20; e reaches b at 2 and c at 7.
    // No rule ends at a or d, no other letter appears, and every letter reaches itself
    // at 0.
    [Fact]
    public void BuildDistanceMatrix_LeetCodeFirstExample_ClosesTheRulesUnderChaining()
    {
        var distances = MinimumCostToConvertStringISolution.BuildDistanceMatrix(
            ['a', 'b', 'c', 'c', 'e', 'd'], ['b', 'c', 'b', 'e', 'b', 'e'], [2, 5, 5, 1, 2, 20]);
        var letters = Enumerable.Range(0, distances.GetLength(0)).ToArray();
        var selfCosts = letters.Select(letter => distances[letter, letter]).Distinct();
        var reachable = ReachablePairs(distances, letters);

        Assert.Equal(26, letters.Length);
        Assert.Equal(26, distances.GetLength(1));
        Assert.Equal([0L], selfCosts);
        Assert.Equal(
            [
                ("ab", 2L), ("ac", 7L), ("ae", 8L), ("bc", 5L), ("be", 6L), ("cb", 3L),
                ("ce", 1L), ("db", 22L), ("dc", 27L), ("de", 20L), ("eb", 2L), ("ec", 7L),
            ],
            reachable);
    }

    // Every off-diagonal entry within the rule-cost bound, row by row, named by its two
    // letters.
    private static (string Pair, long Cost)[] ReachablePairs(long[,] distances, int[] letters) =>
    [
        .. letters.SelectMany(from => letters
            .Where(to => to != from && distances[from, to] <= FirstExampleRuleCostSum)
            .Select(to => (LetterPair(from, to), distances[from, to]))),
    ];

    private static string LetterPair(int from, int to) => $"{(char)('a' + from)}{(char)('a' + to)}";

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
