using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestSubstringWithoutRepeatingCharacters;

// LeetCode 3. Longest Substring Without Repeating Characters: a single sliding-
// window pass tracking each character's last-seen index in this repo's own
// HashMap<char,int>, jumping the window's left edge straight past a repeat
// instead of shrinking it one character at a time.
public sealed partial class LongestSubstringWithoutRepeatingCharactersTests
{
    [Fact]
    public void FindLength_ClassicExample_ReturnsLongestUniqueRun()
    {
        var length = LongestUniqueSubstringLength("abcabcbb");

        Assert.Equal(3, length);
    }

    [Fact]
    public void FindLength_AllSameCharacter_ReturnsOne()
    {
        var length = LongestUniqueSubstringLength("bbbbb");

        Assert.Equal(1, length);
    }

    [Fact]
    public void FindLength_RepeatBeforeWindowStart_DoesNotFalselyShrinkWindow()
    {
        var length = LongestUniqueSubstringLength("pwwkew");

        Assert.Equal(3, length);
    }

    [Fact]
    public void FindLength_EmptyString_ReturnsZero()
    {
        var length = LongestUniqueSubstringLength("");

        Assert.Equal(0, length);
    }

    private static int LongestUniqueSubstringLength(string text)
    {
        var lastSeenIndex = new HashMap<char, int>();
        var windowStart = 0;
        var longest = 0;

        for (var windowEnd = 0; windowEnd < text.Length; windowEnd++)
        {
            var current = text[windowEnd];

            if (lastSeenIndex.TryGetValue(current, out var previousIndex) && previousIndex >= windowStart)
            {
                windowStart = previousIndex + 1;
            }

            lastSeenIndex.Set(current, windowEnd);
            longest = Math.Max(longest, windowEnd - windowStart + 1);
        }

        return longest;
    }
}
