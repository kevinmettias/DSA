using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidAnagram;

// LeetCode 242. Valid Anagram: one O(n) frequency-count pass over this repo's own
// HashMap<char,int>, incrementing per character of s and decrementing per character
// of t, instead of the O(n log n) sort-both-and-compare approach.
public sealed partial class ValidAnagramTests
{
    [Fact]
    public void IsAnagram_ClassicExample_ReturnsTrue()
    {
        var result = IsAnagram("anagram", "nagaram");

        Assert.True(result);
    }

    [Fact]
    public void IsAnagram_NotAnAnagram_ReturnsFalse()
    {
        var result = IsAnagram("rat", "car");

        Assert.False(result);
    }

    [Fact]
    public void IsAnagram_DifferentLengths_ReturnsFalse()
    {
        var result = IsAnagram("aa", "a");

        Assert.False(result);
    }

    private static bool IsAnagram(string s, string t)
    {
        if (s.Length != t.Length)
        {
            return false;
        }

        var counts = new HashMap<char, int>();

        foreach (var c in s)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        foreach (var c in t)
        {
            if (!counts.TryGetValue(c, out var count) || count == 0)
            {
                return false;
            }

            counts.Set(c, count - 1);
        }

        return true;
    }
}
