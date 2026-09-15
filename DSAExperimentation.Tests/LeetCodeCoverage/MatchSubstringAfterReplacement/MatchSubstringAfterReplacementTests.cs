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
    public static TheoryData<string, string, (char Old, char New)[], bool> Examples =>
        new()
        {
            { "fool3e7bar", "leet", [('e', '3'), ('t', '7'), ('t', '8')], true },
            // The mapping only lets an 'o' in sub become '0', not the reverse, so
            // sub's '0' characters can never equal s's 'o' characters.
            { "fooleetbar", "f00l", [('o', '0')], false },
            { "Fool33tbaR", "leetd", [('e', '3'), ('t', '7'), ('t', '8'), ('d', 'b'), ('p', 'b')], true },
            // 'x' has two allowed targets, and only the second one matches at the
            // start position that works - so a lookup keyed by "old" alone is not
            // enough, the target set has to be consulted too.
            { "a1b2", "xy", [('x', '1'), ('x', 'a'), ('y', 'b')], true },
            // No replacements offered at all: this degenerates to plain substring
            // containment.
            { "hello", "ell", [], true },
            { "hello", "lox", [], false },
            // sub cannot fit in s, so there is no candidate start position at all.
            { "ab", "abc", [('a', 'b')], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByLinearScan_LeetCodeExamples_ReportsWhetherSubMatchesSomewhere(
        string s, string sub, (char Old, char New)[] mappings, bool expected) =>
        Assert.Equal(
            expected,
            MatchSubstringAfterReplacementSolution.IsMatchByLinearScan(
                new MatchSubstringAfterReplacementSolution.SourceText(s),
                new MatchSubstringAfterReplacementSolution.SubstringPattern(sub),
                mappings));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsMatchByHashMapLookup_LeetCodeExamples_ReportsWhetherSubMatchesSomewhere(
        string s, string sub, (char Old, char New)[] mappings, bool expected) =>
        Assert.Equal(
            expected,
            MatchSubstringAfterReplacementSolution.IsMatchByHashMapLookup(
                new MatchSubstringAfterReplacementSolution.SourceText(s),
                new MatchSubstringAfterReplacementSolution.SubstringPattern(sub),
                mappings));
}
