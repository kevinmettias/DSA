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
public sealed class FindSubstringWithGivenHashValueTests
{
    // (text, power, modulo, k, hashValue, expected first matching window)
    public static TheoryData<string, int, int, int, long, string> Examples =>
        new()
        {
            { "leetcode", 7, 20, 2, 0, "ee" },
            { "fbxzaad", 31, 100_000, 3, 23_132, "fbx" },

            // k = 1 makes power irrelevant (power^0 == 1), so this is hand
            // checkable: val('a') = 1 and val('b') = 2, and hashValue 2 must
            // return the SECOND character.
            { "ab", 7, 97, 1, 2, "b" },

            // The match is the very last window, so the whole text is scanned
            // before it is found: hash("de") = 4 + 5*7 = 39, and 39 mod 20 = 19.
            { "leetcode", 7, 20, 2, 19, "de" },

            // Whole text as one window: 1 + 2*10 + 3*100 = 321.
            { "abc", 10, 1_000, 3, 321, "abc" },

            // power 1 collapses the hash to a sum of letter values, so "ba" and
            // "ab" both hash to 3 - the FIRST of the two must be returned.
            { "baab", 1, 26, 2, 3, "ba" },

            // No window of "leetcode" hashes to 13 under this lane.
            { "leetcode", 7, 20, 2, 13, "" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindSubstringByWindowRehash_LeetCodeExamples_ReturnsFirstMatchingWindow(
        string s, int power, int modulo, int k, long hashValue, string expected)
    {
        var found = FindSubstringWithGivenHashValueSolution.TryFindSubstringByWindowRehash(
            s, new RollingHashLane(power, modulo), k, hashValue, out var substring);

        AssertFirstMatch(expected, found, substring);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindSubstringByRollingHash_LeetCodeExamples_ReturnsFirstMatchingWindow(
        string s, int power, int modulo, int k, long hashValue, string expected)
    {
        var found = FindSubstringWithGivenHashValueSolution.TryFindSubstringByRollingHash(
            s, new RollingHashLane(power, modulo), k, hashValue, out var substring);

        AssertFirstMatch(expected, found, substring);
    }

    private static void AssertFirstMatch(string expected, bool found, string substring)
    {
        Assert.Equal(expected.Length > 0, found);
        Assert.Equal(expected, substring);
    }
}
