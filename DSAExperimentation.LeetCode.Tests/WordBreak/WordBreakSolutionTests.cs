using DSAExperimentation.LeetCode.WordBreak;

namespace DSAExperimentation.LeetCode.Tests.WordBreak;

// Harness only. Both strategies are WordBreakSolution's - the Trie<bool> + Memoizer
// composition and the bottom-up reachability sweep; this file just pins them to
// LeetCode's published examples.
public sealed partial class WordBreakSolutionTests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBreakByIterativeReachability_LeetCodeExamples_ReturnsWhetherSegmentable(SegmentCase example)
    {
        var canBreak = WordBreakSolution.CanBreakByIterativeReachability(example.S, example.WordDict);

        Assert.Equal(example.Expected, canBreak);
    }

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they answer the same on every example - not merely that each
    // agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBreak_AgreeOnEveryExample(SegmentCase example) =>
        Assert.Equal(
            WordBreakSolution.CanBreakByTrieMemoized(example.S, example.WordDict),
            WordBreakSolution.CanBreakByIterativeReachability(example.S, example.WordDict));

    // One LeetCode example: the string to segment, the dictionary it may be cut into,
    // and whether some concatenation of dictionary words spells the whole string. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct SegmentCase(string S, string[] WordDict, bool Expected);
}
