using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistinctEchoSubstrings;

// LeetCode 1316. Distinct Echo Substrings: an echo substring is text[start..start+2L)
// whose first half text[start..start+L) equals its second half
// text[start+L..start+2L). Every (start, halfLength) pair is screened with this
// repo's own RollingHash for an O(1) equality check before paying for a real
// SequenceEqual verification - the same screen-then-verify shape
// RollingHashSearch.FindAll/LongestChunkedPalindromeDecompositionTests already use
// (a hash match is "probably equal," not "definitely equal") - and every substring
// that verifies as an echo is deduplicated through this repo's own Set<string>
// (HashMap-backed), not a hand-rolled dictionary.
public sealed partial class DistinctEchoSubstringsTests
{
    [Theory]
    [InlineData("abcabcabc", 3)]
    [InlineData("leetcodeleetcode", 2)]
    [InlineData("bbb", 1)]
    public void CountDistinctEchoSubstrings_LeetCodeExamples_ReturnsExpectedCount(string text, int expected)
        => Assert.Equal(expected, CountDistinctEchoSubstrings(text));

    private static int CountDistinctEchoSubstrings(string text)
    {
        var hash = new RollingHash(text);
        var echoes = new Set<string>();

        for (var halfLength = 1; halfLength * 2 <= text.Length; halfLength++)
        {
            for (var start = 0; start + (2 * halfLength) <= text.Length; start++)
            {
                if (IsEcho(text, hash, start, halfLength))
                {
                    echoes.TryAdd(text.Substring(start, halfLength * 2));
                }
            }
        }

        return echoes.Count;
    }

    private static bool IsEcho(string text, RollingHash hash, int start, int halfLength)
    {
        if (hash.Hash(start, halfLength) != hash.Hash(start + halfLength, halfLength))
        {
            return false;
        }

        return text.AsSpan(start, halfLength).SequenceEqual(text.AsSpan(start + halfLength, halfLength));
    }
}
