using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.LeetCode.FindSubstringWithGivenHashValue;

namespace DSAExperimentation.LeetCode.Tests.FindSubstringWithGivenHashValue;

// Harness only: both strategies live in FindSubstringWithGivenHashValueSolution,
// including the window-rehash baseline the pre-migration benchmark kept to itself.
//
// An empty expectation means "no window hashes to that value": LeetCode guarantees
// an answer exists, but the benchmark deliberately asks for an unreachable hash so
// neither strategy can exit early, and that is the path both of its arms actually
// measure - so it is asserted here rather than left to the unasserted baseline
// ARCHITECTURE.md section 17.1 describes. The reversed prefix table the rolling-hash
// strategy is handed is asserted on its own, window by hand-computed window.
public sealed partial class FindSubstringWithGivenHashValueSolutionTests
{
    public static TheoryData<HashExample> Examples =>
        new()
        {
            { new HashExample(Text: "leetcode", Power: 7, Modulo: 20, K: 2, HashValue: 0, Expected: "ee") },
            { new HashExample(Text: "fbxzaad", Power: 31, Modulo: 100_000, K: 3, HashValue: 23_132, Expected: "fbx") },

            // k = 1 makes power irrelevant (power^0 == 1), so this is hand
            // checkable: val('a') = 1 and val('b') = 2, and hashValue 2 must
            // return the SECOND character.
            { new HashExample(Text: "ab", Power: 7, Modulo: 97, K: 1, HashValue: 2, Expected: "b") },

            // The match is the very last window, so the whole text is scanned
            // before it is found: hash("de") = 4 + 5*7 = 39, and 39 mod 20 = 19.
            { new HashExample(Text: "leetcode", Power: 7, Modulo: 20, K: 2, HashValue: 19, Expected: "de") },

            // Whole text as one window: 1 + 2*10 + 3*100 = 321.
            { new HashExample(Text: "abc", Power: 10, Modulo: 1_000, K: 3, HashValue: 321, Expected: "abc") },

            // power 1 collapses the hash to a sum of letter values, so "ba" and
            // "ab" both hash to 3 - the FIRST of the two must be returned.
            { new HashExample(Text: "baab", Power: 1, Modulo: 26, K: 2, HashValue: 3, Expected: "ba") },

            // No window of "leetcode" hashes to 13 under this lane.
            { new HashExample(Text: "leetcode", Power: 7, Modulo: 20, K: 2, HashValue: 13, Expected: "") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindSubstringByWindowRehash_LeetCodeExamples_ReturnsFirstMatchingWindow(HashExample example)
    {
        var found = FindSubstringWithGivenHashValueSolution.TryFindSubstringByWindowRehash(
            example.Text,
            new RollingHashLane(example.Power, example.Modulo),
            (WindowLength: example.K, HashValue: example.HashValue),
            out var substring);

        AssertFirstMatch(example, new WindowSearch(Found: found, Substring: substring));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindSubstringByRollingHash_LeetCodeExamples_ReturnsFirstMatchingWindow(HashExample example)
    {
        var found = FindSubstringWithGivenHashValueSolution.TryFindSubstringByRollingHash(
            example.Text,
            new RollingHashLane(example.Power, example.Modulo),
            (WindowLength: example.K, HashValue: example.HashValue),
            out var substring);

        AssertFirstMatch(example, new WindowSearch(Found: found, Substring: substring));
    }

    // "leetcode" reversed is "edocteel". LeetCode's answer "ee" sits at [1, 3), which maps
    // to [8 - 1 - 2, 8 - 1) = [5, 7) of the reversed text: e then e, each valued 5, so
    // 5 * 7 + 5 = 40, and 40 mod 20 = 0 - the published hashValue. The one lane serves as
    // both of RollingHash's lanes, so the second component is 0 too.
    [Fact]
    public void BuildReversedWindowHash_LeetCodeFirstExample_HashesTheAnswerWindowToZero()
    {
        var reversedHash = FindSubstringWithGivenHashValueSolution.BuildReversedWindowHash(
            "leetcode", new RollingHashLane(7, 20));
        var answerWindow = reversedHash.Hash(5, 2);

        Assert.Equal(8, reversedHash.Length);
        Assert.Equal(new RollingHashValue(0, 0), answerWindow);
    }

    // "abc" reversed is "cba", letters valued from 1: its first character is c (3), its
    // last a (1), and the whole text under base 10 is 3 * 100 + 2 * 10 + 1 = 321 - which is
    // LC 2156's 1 + 2 * 10 + 3 * 100 for "abc", the powers running the other way.
    [Fact]
    public void BuildReversedWindowHash_ThreeLetters_ReversesTheTextAndValuesLettersFromOne()
    {
        var reversedHash = FindSubstringWithGivenHashValueSolution.BuildReversedWindowHash(
            "abc", new RollingHashLane(10, 1_000));
        var firstLetter = reversedHash.Hash(0, 1);
        var lastLetter = reversedHash.Hash(2, 1);
        var wholeText = reversedHash.Hash(0, 3);

        Assert.Equal(new RollingHashValue(3, 3), firstLetter);
        Assert.Equal(new RollingHashValue(1, 1), lastLetter);
        Assert.Equal(new RollingHashValue(321, 321), wholeText);
    }

    private static void AssertFirstMatch(HashExample example, WindowSearch search)
    {
        Assert.Equal(example.Expected.Length > 0, search.Found);
        Assert.Equal(example.Expected, search.Substring);
    }

    // One LeetCode example: the text, the lane's power and modulus, the window length,
    // the hash to look for, and the first window that hashes to it. They travel together
    // at every row and are what both strategies are asked for, so they are one thing with
    // a name rather than six positional arguments a reader has to line up by hand.
    public readonly record struct HashExample(
        string Text, int Power, int Modulo, int K, long HashValue, string Expected);

    // What one search reports: whether any window hashed to the value, and which window it
    // was. They are the two halves of a single Try call - its bool return and its out
    // parameter - so the assertion helper takes them together rather than as a bare flag.
    private readonly record struct WindowSearch(bool Found, string Substring);
}
