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
        var maskToMaxLength = BuildLengthsByMask(words);
        var pairs = ToMaskLengthPairs(maskToMaxLength);
        return BestDisjointProduct(pairs);
    }

    private static HashMap<int, int> BuildLengthsByMask(string[] words)
    {
        var maskToMaxLength = new HashMap<int, int>();

        foreach (var word in words)
        {
            RecordWordMask(word, maskToMaxLength);
        }

        return maskToMaxLength;
    }

    private static (int Mask, int Length)[] ToMaskLengthPairs(HashMap<int, int> maskToMaxLength)
    {
        var masks = maskToMaxLength.Keys.ToArray();
        var pairs = new (int Mask, int Length)[masks.Length];

        for (var i = 0; i < masks.Length; i++)
        {
            maskToMaxLength.TryGetValue(masks[i], out var length);
            pairs[i] = (masks[i], length);
        }

        return pairs;
    }

    private static int BestDisjointProduct((int Mask, int Length)[] pairs)
    {
        var best = 0;

        for (var i = 0; i < pairs.Length; i++)
        {
            for (var j = i + 1; j < pairs.Length; j++)
            {
                if ((pairs[i].Mask & pairs[j].Mask) == 0)
                {
                    best = Math.Max(best, pairs[i].Length * pairs[j].Length);
                }
            }
        }

        return best;
    }

    private static void RecordWordMask(string word, HashMap<int, int> maskToMaxLength)
    {
        var mask = 0;
        foreach (var c in word)
        {
            mask |= 1 << (c - 'a');
        }

        if (maskToMaxLength.TryGetValue(mask, out var existingLength) && existingLength >= word.Length)
        {
            return;
        }

        maskToMaxLength.Set(mask, word.Length);
    }
}
