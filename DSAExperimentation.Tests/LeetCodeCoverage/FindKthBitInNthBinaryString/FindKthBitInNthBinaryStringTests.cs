namespace DSAExperimentation.Tests.LeetCodeCoverage.FindKthBitInNthBinaryString;

// LeetCode 1545. Find Kth Bit in Nth Binary String: straight-line recursive
// bisection over the string's own construction rule
// (Sn = S(n-1) + "1" + invert(reverse(S(n-1)))) without ever materializing Sn - the
// middle bit of Sn is always '1', a k left of it is the same bit as that position in
// S(n-1), and a k right of it mirrors to position (length-k+1) in S(n-1) with the
// result inverted. Each call makes exactly one recursive call (never two), so there
// are no overlapping subproblems for this repo's own Memoizer to help with - no
// repo container or algorithm primitive applies, the same "lighter repo-primitive
// fit" case as PowXn/Power of Two.
public sealed partial class FindKthBitInNthBinaryStringTests
{
    [Theory]
    [InlineData(1, 1, '0')]
    [InlineData(2, 1, '0')]
    [InlineData(2, 3, '1')]
    [InlineData(3, 1, '0')]
    [InlineData(4, 11, '1')]
    public void FindKthBit_LeetCodeExamples_ReturnsExpectedBit(int n, int k, char expected)
    {
        var actual = FindKthBit(n, k);
        Assert.Equal(expected, actual);
    }

    private static char FindKthBit(int n, int k)
    {
        if (n == 1)
        {
            return '0';
        }

        var length = (1 << n) - 1;
        var mid = (length / 2) + 1;

        if (k == mid)
        {
            return '1';
        }

        if (k < mid)
        {
            return FindKthBit(n - 1, k);
        }

        var mirroredBit = FindKthBit(n - 1, length - k + 1);
        return mirroredBit == '0' ? '1' : '0';
    }
}
