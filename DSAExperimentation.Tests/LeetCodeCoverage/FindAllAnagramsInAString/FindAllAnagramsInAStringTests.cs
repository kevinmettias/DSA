using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllAnagramsInAString;

// LeetCode 438. Find All Anagrams in a String: a fixed-size sliding window over s,
// tracking per-character counts in two of this repo's own HashMap<char,int> (window
// vs. target - the same frequency-map shape ValidAnagramTests uses) plus a running
// "matched distinct characters" counter, so each character enters and leaves the
// window exactly once - O(|s| + |p|) instead of rebuilding a fresh frequency map for
// every window start.
public sealed partial class FindAllAnagramsInAStringTests
{
    [Fact]
    public void FindAnagrams_ClassicExample_ReturnsBothStartIndices()
    {
        var result = FindAnagrams("cbaebabacd", "abc");

        Assert.Equal([0, 6], result);
    }

    [Fact]
    public void FindAnagrams_OverlappingWindows_ReturnsEveryMatchingStart()
    {
        var result = FindAnagrams("abab", "ab");

        Assert.Equal([0, 1, 2], result);
    }

    [Fact]
    public void FindAnagrams_PatternLongerThanString_ReturnsEmpty()
    {
        var result = FindAnagrams("a", "aa");

        Assert.Empty(result);
    }

    private static List<int> FindAnagrams(string s, string p)
    {
        var result = new List<int>();
        if (p.Length > s.Length)
        {
            return result;
        }

        var need = new HashMap<char, int>();
        foreach (var c in p)
        {
            need.TryGetValue(c, out var count);
            need.Set(c, count + 1);
        }

        var window = new HashMap<char, int>();
        var matched = 0;

        for (var i = 0; i < s.Length; i++)
        {
            if (need.TryGetValue(s[i], out var needed))
            {
                window.TryGetValue(s[i], out var count);
                window.Set(s[i], count + 1);
                if (count + 1 == needed)
                {
                    matched++;
                }
            }

            if (i < p.Length - 1)
            {
                continue;
            }

            if (matched == need.Count)
            {
                result.Add(i - p.Length + 1);
            }

            var leaving = s[i - p.Length + 1];
            if (need.TryGetValue(leaving, out var neededLeaving))
            {
                window.TryGetValue(leaving, out var leavingCount);
                if (leavingCount == neededLeaving)
                {
                    matched--;
                }

                window.Set(leaving, leavingCount - 1);
            }
        }

        return result;
    }
}
