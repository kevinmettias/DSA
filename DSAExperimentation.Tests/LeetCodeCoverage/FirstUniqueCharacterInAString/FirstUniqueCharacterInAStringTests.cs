using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FirstUniqueCharacterInAString;

// LeetCode 387. First Unique Character in a String: a HashMap<char,int> frequency
// count pass followed by a second pass returning the first index whose count is 1 -
// the same character-bookkeeping shape LongestSubstringWithoutRepeatingCharacters
// already uses this repo's own HashMap<TKey,TValue> for.
public sealed class FirstUniqueCharacterInAStringTests
{
    [Theory]
    [InlineData("leetcode", 0)]
    [InlineData("loveleetcode", 2)]
    [InlineData("aabb", -1)]
    [InlineData("z", 0)]
    public void FirstUniqChar_Examples_ReturnsExpectedIndex(string s, int expected)
        => Assert.Equal(expected, FirstUniqChar(s));

    private static int FirstUniqChar(string s)
    {
        var counts = new HashMap<char, int>();

        foreach (var c in s)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        for (var i = 0; i < s.Length; i++)
        {
            counts.TryGetValue(s[i], out var count);

            if (count == 1)
            {
                return i;
            }
        }

        return -1;
    }
}
