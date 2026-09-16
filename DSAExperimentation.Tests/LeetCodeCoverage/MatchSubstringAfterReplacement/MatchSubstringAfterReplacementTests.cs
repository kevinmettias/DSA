using DSAExperimentation.LeetCode.MatchSubstringAfterReplacement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MatchSubstringAfterReplacement;

// Harness only: both strategies live in MatchSubstringAfterReplacementSolution -
// the linear scan over the raw mapping list that used to be the benchmark's
// untested baseline arm, and the HashMap<char, Set<char>> lookup the test used to
// carry inline. This file just pins them to LeetCode's published examples plus the
// cases that separate the two lookup shapes: one "old" character with several
// allowed targets, no mappings at all, and a sub longer than s.
public sealed class MatchSubstringAfterReplacementTests
{
    public static TheoryData<MatchExample> Examples =>
        new()
        {
            {
                new MatchExample(
                    Source: "fool3e7bar",
                    Sub: "leet",
                    Mappings: [('e', '3'), ('t', '7'), ('t', '8')],
                    Expected: true)
            },
            // The mapping only lets an 'o' in sub become '0', not the reverse, so
            // sub's '0' characters can never equal s's 'o' characters.
            {
                new MatchExample(
                    Source: "fooleetbar",
                    Sub: "f00l",
                    Mappings: [('o', '0')],
                    Expected: false)
            },
            {
                new MatchExample(
                    Source: "Fool33tbaR",
                    Sub: "leetd",
                    Mappings: [('e', '3'), ('t', '7'), ('t', '8'), ('d', 'b'), ('p', 'b')],
                    Expected: true)
            },
            // 'x' has two allowed targets, and only the second one matches at the
            // start position that works - so a lookup keyed by "old" alone is not
            // enough, the target set has to be consulted too.
            {
                new MatchExample(
                    Source: "a1b2",
                    Sub: "xy",
                    Mappings: [('x', '1'), ('x', 'a'), ('y', 'b')],
                    Expected: true)
            },
            // No replacements offered at all: this degenerates to plain substring
            // containment.
            { new MatchExample(Source: "hello", Sub: "ell", Mappings: [], Expected: true) },
            { new MatchExample(Source: "hello", Sub: "lox", Mappings: [], Expected: false) },
            // sub cannot fit in s, so there is no candidate start position at all.
            { new MatchExample(Source: "ab", Sub: "abc", Mappings: [('a', 'b')], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByLinearScan_LeetCodeExamples_ReportsWhetherSubMatchesSomewhere(MatchExample example)
    {
        var actual = MatchSubstringAfterReplacementSolution.IsMatchByLinearScan(
            new MatchSubstringAfterReplacementSolution.SourceText(example.Source),
            new MatchSubstringAfterReplacementSolution.SubstringPattern(example.Sub),
            example.Mappings);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByHashMapLookup_LeetCodeExamples_ReportsWhetherSubMatchesSomewhere(MatchExample example)
    {
        var actual = MatchSubstringAfterReplacementSolution.IsMatchByHashMapLookup(
            new MatchSubstringAfterReplacementSolution.SourceText(example.Source),
            new MatchSubstringAfterReplacementSolution.SubstringPattern(example.Sub),
            example.Mappings);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the text whose every window is tried, the pattern that
    // has to match one of them, the allowed one-directional replacements, and whether
    // a match exists. Source and sub are both `string` - and the two the solution
    // already gives distinct types - so the row names the roles here too rather than
    // leaving two adjacent positions a caller could hand over the wrong way round.
    public readonly record struct MatchExample(
        string Source,
        string Sub,
        (char Old, char New)[] Mappings,
        bool Expected);
}
