using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PowerOfThree;

// LeetCode 326. Power of Three: precompute the closed set of powers of three that
// fit in a 32-bit int (3^0..3^19 - 3^20 overflows), then binary-search this repo's
// own BinarySearch.Find over that sorted ArraySequence<int> - reframing "is n a
// power of three" as "does n occur in a known, already-sorted sequence" instead of a
// hand-rolled repeated-division loop.
public sealed partial class PowerOfThreeTests
{
    private static readonly int[] PowersOfThree = BuildPowersOfThree();

    [Theory]
    [InlineData(1, true)]
    [InlineData(3, true)]
    [InlineData(9, true)]
    [InlineData(27, true)]
    [InlineData(45, false)]
    [InlineData(0, false)]
    [InlineData(-3, false)]
    [InlineData(1162261467, true)] // 3^19, the largest power of three that fits in an int
    [InlineData(1162261466, false)]
    [InlineData(int.MaxValue, false)]
    public void IsPowerOfThree_Examples_ReturnsExpected(int n, bool expected)
    {
        var found = IsPowerOfThree(n);

        Assert.Equal(expected, found);
    }

    private static bool IsPowerOfThree(int n)
    {
        var sequence = new ArraySequence<int>(PowersOfThree);

        return BinarySearch.Find<int, ArraySequence<int>>(sequence, n) is not null;
    }

    private static int[] BuildPowersOfThree()
    {
        var powers = new List<int>();
        long power = 1;

        while (power <= int.MaxValue)
        {
            powers.Add((int)power);
            power *= 3;
        }

        return [.. powers];
    }
}
