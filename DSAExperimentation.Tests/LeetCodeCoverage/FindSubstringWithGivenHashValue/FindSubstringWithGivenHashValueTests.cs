using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindSubstringWithGivenHashValue;

// LeetCode 2156. Find Substring With Given Hash Value: this repo's own RollingHash,
// queried over the REVERSED input text. RollingHash.Hash(start,length) combines a
// window as sum(val(text[start+j]) * base^(length-1-j)) - the LEFTMOST character of
// the queried window gets the HIGHEST power (confirmed by RollingHashTests' own
// hand-computed "ab"/base-10/mod-97 example: Hash(0,2) = val('a')*10 + val('b')*1).
// LeetCode's hash formula wants the opposite power assignment - val(s[0]) at
// power^0 (lowest), val(s[k-1]) at power^(k-1) (highest) - which is exactly what a
// window of the REVERSED text produces: its leftmost character is the original
// window's LAST character (so it lands on the highest power), and its rightmost
// character is the original window's FIRST character (so it lands on power^0). A
// window [i, i+k) of the original text of length n therefore maps to
// [n-i-k, n-i) of the reversed text. val(c) = its alphabet index + 1 is supplied via
// a custom IEqualityComparer<char>, the same injection point RollingHashTests' own
// case-insensitive-comparer test already exercises. The same (power, modulo) pair is
// passed as both lanes of the full-control constructor - RollingHash always
// double-hashes for collision resistance (RollingHash.cs's own doc comment), but
// this problem's hashValue is an exact modular value to match, not a probabilistic
// screen, so only the First lane component is ever compared.
public sealed partial class FindSubstringWithGivenHashValueTests
{
    [Fact]
    public void FindSubstring_LeetCodeExampleOne_ReturnsFirstMatchingWindow()
    {
        var result = FindSubstring("leetcode", new RollingHashLaneConfig(Power: 7, Modulo: 20), k: 2, hashValue: 0);

        Assert.Equal("ee", result);
    }

    [Fact]
    public void FindSubstring_LeetCodeExampleTwo_ReturnsFirstMatchingWindow()
    {
        var result = FindSubstring("fbxzaad", new RollingHashLaneConfig(Power: 31, Modulo: 100_000), k: 3, hashValue: 23_132);

        Assert.Equal("fbx", result);
    }

    // Hand-verifiable with k=1: power never actually contributes (power^0 == 1
    // regardless of power's value). val('a')=1, val('b')=2, so "ab" hashed one
    // character at a time gives Hash("a")=1, Hash("b")=2 - hashValue=2 must return
    // "b", the second (not first) character.
    [Fact]
    public void FindSubstring_SingleCharacterWindow_MatchesHandComputedValue()
    {
        var result = FindSubstring("ab", new RollingHashLaneConfig(Power: 7, Modulo: 97), k: 1, hashValue: 2);

        Assert.Equal("b", result);
    }

    private readonly record struct RollingHashLaneConfig(int Power, int Modulo);

    private static string FindSubstring(string s, RollingHashLaneConfig laneConfig, int k, long hashValue)
    {
        var n = s.Length;
        var reversedChars = s.ToCharArray();
        Array.Reverse(reversedChars);
        var reversed = new string(reversedChars);

        var comparer = EqualityComparer<char>.Create((left, right) => left == right, value => value - 'a' + 1);
        var lane = new RollingHashLane(laneConfig.Power, laneConfig.Modulo);
        var hash = new RollingHash(reversed, comparer, lane, lane);

        for (var i = 0; i <= n - k; i++)
        {
            if (hash.Hash(n - i - k, k).First == hashValue)
            {
                return s.Substring(i, k);
            }
        }

        throw new InvalidOperationException("No substring with the given hash value exists.");
    }
}
