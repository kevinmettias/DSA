using DSAExperimentation.LeetCode.WordBreak;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WordBreak;

// Harness only. The Trie<bool> + Memoizer composition is WordBreakSolution's -
// this file just pins it to LeetCode's published examples.
public sealed class WordBreakTests
{
    public static TheoryData<SegmentCase> Examples =>
        new()
        {
            { new SegmentCase(S: "leetcode", WordDict: ["leet", "code"], Expected: true) },
            { new SegmentCase(S: "applepenapple", WordDict: ["apple", "pen"], Expected: true) },
            { new SegmentCase(S: "catsandog", WordDict: ["cats", "dog", "sand", "and", "cat"], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBreakByTrieMemoized_LeetCodeExamples_ReturnsWhetherSegmentable(SegmentCase example)
    {
        var canBreak = WordBreakSolution.CanBreakByTrieMemoized(example.S, example.WordDict);

        Assert.Equal(example.Expected, canBreak);
    }

    // One LeetCode example: the string to segment, the dictionary it may be cut into,
    // and whether some concatenation of dictionary words spells the whole string. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct SegmentCase(string S, string[] WordDict, bool Expected);
}
