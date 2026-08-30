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
        Assert.True(CheckInclusion("ab", "eidbaooo"));
    }

    [Fact]
    public void CheckInclusion_NoPermutationSubstringExists_ReturnsFalse()
    {
        Assert.False(CheckInclusion("ab", "eidboaoo"));
    }

    private static bool CheckInclusion(string s1, string s2)
    {
        if (s1.Length > s2.Length)
        {
            return false;
        }

        var need = new HashMap<char, int>();
        foreach (var c in s1)
        {
            need.TryGetValue(c, out var count);
            need.Set(c, count + 1);
        }

        var window = new HashMap<char, int>();
        var matched = 0;

        for (var i = 0; i < s2.Length; i++)
        {
            if (need.TryGetValue(s2[i], out var needed))
            {
                window.TryGetValue(s2[i], out var count);
                window.Set(s2[i], count + 1);
                if (count + 1 == needed)
                {
                    matched++;
                }
            }

            if (i < s1.Length - 1)
            {
                continue;
            }

            if (matched == need.Count)
            {
                return true;
            }

            var leaving = s2[i - s1.Length + 1];
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

        return false;
    }
}
