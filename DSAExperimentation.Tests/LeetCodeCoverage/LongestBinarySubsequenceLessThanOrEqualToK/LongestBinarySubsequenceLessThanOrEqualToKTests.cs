namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestBinarySubsequenceLessThanOrEqualToK;

// LeetCode 2311. Longest Binary Subsequence Less Than or Equal to K: no repo
// primitive applies - a pure O(n) right-to-left greedy scan over the string itself,
// the same "no stronger reusable primitive over a bare sequence" category already
// established for GasStation/JumpGame/MaximumProductSubarray. Every '0' is always
// worth including (it contributes 0 to the value no matter where it lands, only
// length), so it's accepted unconditionally; every '1' is a real cost (its weight
// doubles with every character already accepted to its right), so it's accepted
// greedily, cheapest (rightmost, least-significant) first, only while the running
// value stays within k. Once the running power has climbed past ~30, no further '1'
// can ever be affordable (2^30 already exceeds k's maximum possible value of 1e9),
// so those are skipped outright rather than computing 1L shifted by an unbounded
// power - shift counts on a long are masked to 6 bits in C#, so an unguarded shift
// past 63 would silently produce a small, wrong weight instead of the huge one
// intended. The same "state the complexity/overflow law, don't just assume it"
// discipline IntegerReplacementTests' int-vs-long choice already documents.
public sealed partial class LongestBinarySubsequenceLessThanOrEqualToKTests
{
    [Fact]
    public void LongestSubsequence_LeetCodeExampleOne_ReturnsFive()
    {
        var actual = LongestSubsequence("1001010", k: 5);
        Assert.Equal(5, actual);
    }

    [Fact]
    public void LongestSubsequence_LeetCodeExampleTwo_ReturnsSix()
    {
        var actual = LongestSubsequence("00101001", k: 1);
        Assert.Equal(6, actual);
    }

    [Fact]
    public void LongestSubsequence_IncludingEveryZeroWouldOverflowK_DropsSomeOnesInstead()
    {
        // All zeros are free, but greedily taking both available '1's ("101" = 5)
        // would exceed k=3, so only the cheaper, rightmost '1' can be kept.
        var actual = LongestSubsequence("1101", k: 3);
        Assert.Equal(2, actual);
    }

    [Fact]
    public void LongestSubsequence_SingleZero_IsAlwaysIncludable()
    {
        var actual = LongestSubsequence("0", k: 5);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void LongestSubsequence_SingleOneExceedsK_ReturnsEmptySubsequence()
    {
        var actual = LongestSubsequence("1", k: 0);
        Assert.Equal(0, actual);
    }

    private const int MaxAffordablePower = 30; // 2^30 > 1e9, k's maximum possible value

    private static int LongestSubsequence(string s, int k)
    {
        var length = 0;
        long value = 0;
        var power = 0;

        for (var i = s.Length - 1; i >= 0; i--)
        {
            if (s[i] == '0')
            {
                length++;
                power++;
                continue;
            }

            if (power >= MaxAffordablePower)
            {
                continue;
            }

            var weight = 1L << power;
            if (value + weight <= k)
            {
                value += weight;
                power++;
                length++;
            }
        }

        return length;
    }
}
