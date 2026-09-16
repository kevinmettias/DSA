using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.LeetCode.FindSubstringWithGivenHashValue;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindSubstringWithGivenHashValue;

// Harness only: both strategies live in FindSubstringWithGivenHashValueSolution,
// including the window-rehash baseline the pre-migration benchmark kept to itself.
//
// An empty expectation means "no window hashes to that value": LeetCode guarantees
// an answer exists, but the benchmark deliberately asks for an unreachable hash so
// neither strategy can exit early, and that is the path both of its arms actually
// measure - so it is asserted here rather than left to the unasserted baseline
// ARCHITECTURE.md section 17.1 describes.
public sealed partial class FindSubstringWithGivenHashValueTests
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
