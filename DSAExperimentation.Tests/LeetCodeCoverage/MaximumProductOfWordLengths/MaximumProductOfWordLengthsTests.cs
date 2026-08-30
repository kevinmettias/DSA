using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumProductOfWordLengths;

// LeetCode 318. Maximum Product of Word Lengths: each word collapses to a 26-bit
// letter-presence mask, and this repo's own HashMap<int,int> dedupes words sharing
// a mask down to the longest one before the pairwise scan - two words can be
// combined exactly when their masks share no set bit (mask1 & mask2 == 0).
public sealed partial class MaximumProductOfWordLengthsTests
{
    [Fact]
    public void MaxProduct_ClassicExample_ReturnsLargestDisjointProduct()
        => Assert.Equal(16, MaxProduct(["abcw", "baz", "foo", "bar", "xtfn", "abcdef"]));

    [Fact]
    public void MaxProduct_SecondExample_ReturnsLargestDisjointProduct()
        => Assert.Equal(4, MaxProduct(["a", "ab", "abc", "d", "cd", "bcd", "abcd"]));

    [Fact]
    public void MaxProduct_EveryWordSharesLetterA_ReturnsZero()
        => Assert.Equal(0, MaxProduct(["a", "aa", "aaa", "aaaa"]));

    private static int MaxProduct(string[] words)
    {
        var maskToMaxLength = new HashMap<int, int>();

        foreach (var word in words)
        {
            var mask = 0;
            foreach (var c in word)
            {
                mask |= 1 << (c - 'a');
            }

            if (maskToMaxLength.TryGetValue(mask, out var existingLength) && existingLength >= word.Length)
            {
                continue;
            }

            maskToMaxLength.Set(mask, word.Length);
        }

        var masks = maskToMaxLength.Keys.ToArray();
        var lengths = new int[masks.Length];
        for (var i = 0; i < masks.Length; i++)
        {
            maskToMaxLength.TryGetValue(masks[i], out lengths[i]);
        }

        var best = 0;

        for (var i = 0; i < masks.Length; i++)
        {
            for (var j = i + 1; j < masks.Length; j++)
            {
                if ((masks[i] & masks[j]) == 0)
                {
                    best = Math.Max(best, lengths[i] * lengths[j]);
                }
            }
        }

        return best;
    }
}
