using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestNiceSubstring;

// LeetCode 1763. Longest Nice Substring: the standard divide-and-conquer
// approach - build this repo's own Set<char> of every character present in
// the (sub)string, then scan for the first character whose opposite-case
// partner is missing from that Set. If none is missing, the whole string is
// already nice. Otherwise no nice substring can ever cross that character
// (it can never gain its missing partner), so the answer is the longer of
// the two halves split around it, recursed independently.
public sealed partial class LongestNiceSubstringTests
{
    [Fact]
    public void FindLongestNiceSubstring_MixedCaseWithOneBadCharacter_ReturnsInnerNiceRun()
    {
        var result = FindLongestNiceSubstring("YazaAay");

        Assert.Equal("aAa", result);
    }

    [Fact]
    public void FindLongestNiceSubstring_WholeStringAlreadyNice_ReturnsWholeString()
    {
        var result = FindLongestNiceSubstring("Bb");

        Assert.Equal("Bb", result);
    }

    [Fact]
    public void FindLongestNiceSubstring_SingleCharacterCanNeverBeNice_ReturnsEmpty()
    {
        var result = FindLongestNiceSubstring("c");

        Assert.Equal(string.Empty, result);
    }

    private static string FindLongestNiceSubstring(string s)
    {
        if (s.Length < 2)
        {
            return string.Empty;
        }

        var present = BuildCharacterSet(s);

        return SplitAtMissingPartner(s, present) ?? s;
    }

    private static Set<char> BuildCharacterSet(string s)
    {
        var present = new Set<char>();

        foreach (var c in s)
        {
            present.TryAdd(c);
        }

        return present;
    }

    // No nice substring can ever cross a character whose opposite-case partner
    // is missing from the whole string, so the answer is the longer of the two
    // halves split around the first such character, recursed independently.
    // Returns null when every character has its partner (s is already nice).
    private static string? SplitAtMissingPartner(string s, Set<char> present)
    {
        for (var i = 0; i < s.Length; i++)
        {
            if (HasMissingPartner(present, s[i]))
            {
                var left = FindLongestNiceSubstring(s[..i]);
                var right = FindLongestNiceSubstring(s[(i + 1)..]);
                return left.Length >= right.Length ? left : right;
            }
        }

        return null;
    }

    private static bool HasMissingPartner(Set<char> present, char c)
        => char.IsUpper(c) ? !present.Has(char.ToLower(c)) : !present.Has(char.ToUpper(c));
}
