using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfFour;

// LeetCode 342. Power of Four: BinarySearch.Find over a virtual, already-sorted
// sequence of every power of 4 that fits in a 32-bit int (4^0 .. 4^15) - n is a
// power of four exactly when Find locates it. The same "search a monotone virtual
// sequence" idiom SqrtXTests uses for LeetCode 69, applied to a finite membership
// check instead of a floor-root search.
public sealed partial class PowerOfFourTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(16, true)]
    [InlineData(5, false)]
    [InlineData(0, false)]
    [InlineData(-4, false)]
    [InlineData(1073741824, true)] // 4^15, the largest power of four an int holds.
    public void IsPowerOfFour_Examples_ReturnsExpected(int n, bool expected)
        => Assert.Equal(expected, IsPowerOfFour(n));

    private static bool IsPowerOfFour(int n)
    {
        if (n <= 0)
        {
            return false;
        }

        var sequence = new PowersOfFourSequence();
        return BinarySearch.Find<int, PowersOfFourSequence>(sequence, n) is not null;
    }

    private readonly struct PowersOfFourSequence : IRandomAccessSequence<int>
    {
        public int Length => 16;

        public int Get(int index) => 1 << (2 * index);
    }
}
