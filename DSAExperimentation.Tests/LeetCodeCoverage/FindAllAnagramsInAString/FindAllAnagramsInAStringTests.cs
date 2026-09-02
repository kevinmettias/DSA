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

        var need = BuildFrequencyMap(p);
        var state = new AnagramWindowState(need);

        for (var i = 0; i < s.Length; i++)
        {
            if (state.Advance(i, s, p.Length))
            {
                result.Add(i - p.Length + 1);
            }
        }

        return result;
    }

    private static HashMap<char, int> BuildFrequencyMap(string p)
    {
        var need = new HashMap<char, int>();
        foreach (var c in p)
        {
            need.TryGetValue(c, out var count);
            need.Set(c, count + 1);
        }

        return need;
    }

    private sealed class AnagramWindowState(HashMap<char, int> need)
    {
        private readonly HashMap<char, int> _window = new();
        private int _matched;

        public bool IsFullMatch => _matched == need.Count;

        public bool Advance(int i, string s, int windowLength)
        {
            AbsorbEntering(s[i]);

            if (i < windowLength - 1)
            {
                return false;
            }

            var isMatch = IsFullMatch;
            ReleaseLeaving(s[i - windowLength + 1]);
            return isMatch;
        }

        public void AbsorbEntering(char c)
        {
            if (!need.TryGetValue(c, out var needed))
            {
                return;
            }

            _window.TryGetValue(c, out var count);
            _window.Set(c, count + 1);

            if (count + 1 == needed)
            {
                _matched++;
            }
        }

        public void ReleaseLeaving(char c)
        {
            if (!need.TryGetValue(c, out var needed))
            {
                return;
            }

            _window.TryGetValue(c, out var count);

            if (count == needed)
            {
                _matched--;
            }

            _window.Set(c, count - 1);
        }
    }
}
