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
    {
        var actual = SmallestEquivalentString(s1, s2, baseStr);
        Assert.Equal(expected, actual);
    }

    private static string SmallestEquivalentString(string s1, string s2, string baseStr)
    {
        var equivalences = BuildEquivalences(s1, s2);
        var smallestInGroup = ComputeSmallestPerGroup(equivalences);

        return RemapThroughGroups(baseStr, equivalences, smallestInGroup);
    }

    private static DisjointSet BuildEquivalences(string s1, string s2)
    {
        var equivalences = new DisjointSet(26);

        for (var i = 0; i < s1.Length; i++)
        {
            equivalences.Union(s1[i] - 'a', s2[i] - 'a');
        }

        return equivalences;
    }

    private static char[] ComputeSmallestPerGroup(DisjointSet equivalences)
    {
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

        return smallestInGroup;
    }

    private static string RemapThroughGroups(string baseStr, DisjointSet equivalences, char[] smallestInGroup)
    {
        var result = new char[baseStr.Length];

        for (var i = 0; i < baseStr.Length; i++)
        {
            result[i] = smallestInGroup[equivalences.Find(baseStr[i] - 'a')];
        }

        return new string(result);
    }
}
