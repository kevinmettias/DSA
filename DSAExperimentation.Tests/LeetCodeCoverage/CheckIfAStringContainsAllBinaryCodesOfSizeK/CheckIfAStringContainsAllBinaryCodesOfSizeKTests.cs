using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfAStringContainsAllBinaryCodesOfSizeK;

// LeetCode 1461. Check If a String Contains All Binary Codes of Size K: slide a
// length-k window across s, folding each window into an int via bit-shifting
// (drop the bit that falls out of range, append the new one, mask to k bits) -
// the classic rolling-bitmask trick for a *binary* alphabet - and record every
// distinct code seen in this repo's own Set<int> (HashMap<Element,bool>-backed,
// DataStructures/Set/Set.cs). s contains every code of length k exactly when
// the set ends up holding all 2^k of them.
public sealed partial class CheckIfAStringContainsAllBinaryCodesOfSizeKTests
{
    [Fact]
    public void HasAllCodes_ClassicExampleWithEveryTwoBitCode_ReturnsTrue()
    {
        Assert.True(HasAllCodes("00110110", k: 2));
    }

    [Fact]
    public void HasAllCodes_EveryOneBitCodePresent_ReturnsTrue()
    {
        Assert.True(HasAllCodes("0110", k: 1));
    }

    [Fact]
    public void HasAllCodes_MissingOneOfTheFourTwoBitCodes_ReturnsFalse()
    {
        Assert.False(HasAllCodes("0110", k: 2));
    }

    [Fact]
    public void HasAllCodes_TooShortToPossiblyContainEveryCode_ReturnsFalse()
    {
        // Fewer than 2^k + k - 1 characters can never contain all 2^k
        // length-k substrings, regardless of content - short-circuited before
        // ever touching the sliding window/Set.
        Assert.False(HasAllCodes("111", k: 3));
    }

    private static bool HasAllCodes(string s, int k)
    {
        var total = 1 << k;

        if (s.Length < total + k - 1)
        {
            return false;
        }

        var seen = new Set<int>();
        var mask = total - 1;
        var code = 0;

        for (var i = 0; i < s.Length; i++)
        {
            code = ((code << 1) | (s[i] - '0')) & mask;

            if (i >= k - 1)
            {
                seen.TryAdd(code);
            }
        }

        return seen.Count == total;
    }
}
