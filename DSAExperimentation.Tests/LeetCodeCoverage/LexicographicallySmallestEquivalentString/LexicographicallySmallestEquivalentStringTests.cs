using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicallySmallestEquivalentString;

// LeetCode 1061. Lexicographically Smallest Equivalent String: DisjointSet over
// the 26 lowercase-letter ids (union every s1[i]/s2[i] pair - the same
// SatisfiabilityOfEqualityEquations union-over-26-letters shape), then one pass
// recording the smallest letter reached per root and one pass over baseStr
// remapping each character through it.
public sealed partial class LexicographicallySmallestEquivalentStringTests
{
    [Theory]
    [InlineData("parker", "morris", "parser", "makkek")]
    [InlineData("hello", "world", "hold", "hdld")]
    [InlineData("leetcode", "programs", "sourcecode", "aauaaaaada")]
    public void SmallestEquivalentString_LeetCodeExamples_ReturnsRemappedString(
        string s1, string s2, string baseStr, string expected)
        => Assert.Equal(expected, SmallestEquivalentString(s1, s2, baseStr));

    private static string SmallestEquivalentString(string s1, string s2, string baseStr)
    {
        var equivalences = new DisjointSet(26);

        for (var i = 0; i < s1.Length; i++)
        {
            equivalences.Union(s1[i] - 'a', s2[i] - 'a');
        }

        var smallestInGroup = new char[26];
        for (var letter = 0; letter < 26; letter++)
        {
            var root = equivalences.Find(letter);
            var candidate = (char)('a' + letter);

            if (smallestInGroup[root] == default || candidate < smallestInGroup[root])
            {
                smallestInGroup[root] = candidate;
            }
        }

        var result = new char[baseStr.Length];
        for (var i = 0; i < baseStr.Length; i++)
        {
            result[i] = smallestInGroup[equivalences.Find(baseStr[i] - 'a')];
        }

        return new string(result);
    }
}
