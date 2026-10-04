using DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

namespace DSAExperimentation.LeetCode.Tests.MinimumCostToConvertStringII;

// Harness only. The distinct-substring conversion graph is
// LeetCode.MinimumCostToConvertStringII.SubstringNetwork and both
// Floyd-Warshall strategies are MinimumCostToConvertStringIISolution's -
// this file just pins them to LeetCode's published examples: a
// single-character case identical to LC 2976's, a multi-character chain
// requiring two hops through one substring, and an unreachable case. The string index
// and the closed distance matrix the baseline is handed are asserted on their own.
public sealed partial class MinimumCostToConvertStringIISolutionTests
{
    // LeetCode's second rule set costs 1 + 3 + 5 = 9 in all. A shortest chain never takes
    // the same rule twice, so a reachable pair costs at most 9, and any entry above that
    // is the matrix's mark for "unreachable", whatever its value.
    private const long SecondExampleRuleCostSum = 9;

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

    // LeetCode's second rule set: originals first, then changes, each string numbered the
    // first time it is seen - bcd 0, fgh 1, thh 2 from original, then cde 3 and ghh 4 from
    // changed, where thh is already numbered. Five strings, numbered 0 to 4 with no gap.
    [Fact]
    public void BuildIndex_LeetCodeSecondExample_NumbersEachDistinctStringOnFirstSight()
    {
        var index = MinimumCostToConvertStringIISolution.BuildIndex(["bcd", "fgh", "thh"], ["cde", "thh", "ghh"]);
        var stringsInIdOrder = index.OrderBy(entry => entry.Value).Select(entry => entry.Key);
        var ids = index.Values.Order();

        Assert.Equal(["bcd", "fgh", "thh", "cde", "ghh"], stringsInIdOrder);
        Assert.Equal([0, 1, 2, 3, 4], ids);
    }

    // LeetCode's second rule set over the index written out by hand: bcd->cde 1,
    // fgh->thh 3, thh->ghh 5. Closing it under chaining adds one pair, fgh->ghh at
    // 3 + 5 = 8 - LeetCode's own two-hop conversion. Every string reaches itself at 0,
    // and no other pair connects.
    [Fact]
    public void BuildDistanceMatrix_LeetCodeSecondExample_ClosesTheRulesUnderChaining()
    {
        string[] names = ["bcd", "fgh", "thh", "cde", "ghh"];
        var index = new Dictionary<string, int> { ["bcd"] = 0, ["fgh"] = 1, ["thh"] = 2, ["cde"] = 3, ["ghh"] = 4 };

        var distances = MinimumCostToConvertStringIISolution.BuildDistanceMatrix(
            index, ["bcd", "fgh", "thh"], ["cde", "thh", "ghh"], [1, 3, 5]);
        var ids = Enumerable.Range(0, distances.GetLength(0)).ToArray();
        var selfCosts = ids.Select(id => distances[id, id]).Distinct();
        var reachable = ReachablePairs(distances, names);

        Assert.Equal(5, ids.Length);
        Assert.Equal([0L], selfCosts);
        Assert.Equal([("bcd", "cde", 1L), ("fgh", "thh", 3L), ("fgh", "ghh", 8L), ("thh", "ghh", 5L)], reachable);
    }

    // Every off-diagonal entry within the rule-cost bound, row by row, named by the two
    // strings whose ids index it.
    private static (string From, string To, long Cost)[] ReachablePairs(long[,] distances, string[] names) =>
    [
        .. names.Index().SelectMany(from => names.Index()
            .Where(to => to.Index != from.Index && distances[from.Index, to.Index] <= SecondExampleRuleCostSum)
            .Select(to => (from.Item, to.Item, distances[from.Index, to.Index]))),
    ];

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
