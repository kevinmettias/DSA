using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FirstBadVersion;

// LeetCode 278. First Bad Version: BinarySearch.LowerBound over a monotone virtual
// sequence - isBadVersion computed on demand via Get, the same "no stored array"
// shape SqrtXTests uses for Math.Sqrt - finds the first bad version in O(log n)
// probes instead of an O(n) linear scan.
public sealed partial class FirstBadVersionTests
{
    [Theory]
    [InlineData(5, 4, 4)]
    [InlineData(1, 1, 1)]
    [InlineData(2126753390, 1702766719, 1702766719)]
    public void FirstBadVersion_LeetCodeExamples_ReturnsFirstBadVersion(int n, int firstBad, int expected)
    {
        var actual = FirstBadVersion(n, firstBad);
        Assert.Equal(expected, actual);
    }

    private static int FirstBadVersion(int n, int firstBad)
    {
        var sequence = new IsBadVersionSequence(firstBad, n);
        return BinarySearch.LowerBound<int, IsBadVersionSequence>(sequence, 1) + 1;
    }

    // index is the 0-based version-1: every version from firstBad onward is bad, and
    // the sequence never actually allocates the n versions it represents.
    private readonly struct IsBadVersionSequence(int firstBad, int length) : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => index + 1 >= firstBad ? 1 : 0;
    }
}
