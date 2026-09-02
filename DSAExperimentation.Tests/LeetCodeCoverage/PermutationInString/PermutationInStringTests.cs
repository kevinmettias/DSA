using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PermutationInString;

// LeetCode 567. Permutation in String: the same fixed-size sliding window this
// repo's Find All Anagrams in a String (438) coverage already uses - two
// HashMap<char,int> frequency maps (window vs. target) plus a running "matched
// distinct characters" counter, so each character enters/leaves the window
// exactly once - O(|s1| + |s2|). This problem only needs the first match, so it
// returns as soon as the window's character counts equal s1's.
public sealed partial class PermutationInStringTests
{
    [Fact]
    public void CheckInclusion_PermutationPresentAsSubstring_ReturnsTrue()
    {
        var actual = CheckInclusion("ab", "eidbaooo");
        Assert.True(actual);
    }

    [Fact]
    public void CheckInclusion_NoPermutationSubstringExists_ReturnsFalse()
    {
        var actual = CheckInclusion("ab", "eidboaoo");
        Assert.False(actual);
    }

    private static bool CheckInclusion(string s1, string s2)
    {
        if (s1.Length > s2.Length)
        {
            return false;
        }

        var need = BuildFrequencyMap(s1);
        var window = new HashMap<char, int>();
        var matched = 0;
        var context = new SlidingWindowContext(s1, s2, need, window);

        for (var i = 0; i < s2.Length; i++)
        {
            if (SlideWindow(context, i, ref matched))
            {
                return true;
            }
        }

        return false;
    }

    private static HashMap<char, int> BuildFrequencyMap(string s)
    {
        var frequencies = new HashMap<char, int>();

        foreach (var c in s)
        {
            frequencies.TryGetValue(c, out var count);
            frequencies.Set(c, count + 1);
        }

        return frequencies;
    }

    // One step of the fixed-size window: absorbs s2[i], then - once the window has
    // reached s1's length - checks for a full match and evicts the character
    // leaving the window. Returns true once the window matches s1 exactly.
    private static bool SlideWindow(SlidingWindowContext context, int i, ref int matched)
    {
        var (s1, s2, need, window) = context;

        AbsorbEnteringChar(need, window, s2[i], ref matched);

        if (i < s1.Length - 1)
        {
            return false;
        }

        if (matched == need.Count)
        {
            return true;
        }

        EvictLeavingChar(need, window, s2[i - s1.Length + 1], ref matched);
        return false;
    }

    private static void AbsorbEnteringChar(HashMap<char, int> need, HashMap<char, int> window, char entering, ref int matched)
    {
        if (!need.TryGetValue(entering, out var needed))
        {
            return;
        }

        window.TryGetValue(entering, out var count);
        window.Set(entering, count + 1);
        if (count + 1 == needed)
        {
            matched++;
        }
    }

    private static void EvictLeavingChar(HashMap<char, int> need, HashMap<char, int> window, char leaving, ref int matched)
    {
        if (!need.TryGetValue(leaving, out var neededLeaving))
        {
            return;
        }

        window.TryGetValue(leaving, out var leavingCount);
        if (leavingCount == neededLeaving)
        {
            matched--;
        }

        window.Set(leaving, leavingCount - 1);
    }

    private readonly record struct SlidingWindowContext(string S1, string S2, HashMap<char, int> Need, HashMap<char, int> Window);
}
