using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortIntegersByThePowerValue;

// LeetCode 1387. Sort Integers by The Power Value: compute each integer's Collatz
// "power" (step count to reach 1), pair it with the integer itself, and sort those
// (Power, Value) pairs with this repo's own MergeSort over ArrayIndexedSequence
// (SortAnArrayTests/HeightCheckerTests precedent) - no custom comparer needed,
// since ValueTuple<int,int> already implements IComparable lexicographically
// (KClosestPointsToOriginTests precedent), which sorts by Power first and Value
// second, exactly LeetCode's tie-break rule.
public sealed partial class SortIntegersByThePowerValueTests
{
    [Theory]
    [InlineData(12, 15, 2, 13)]
    [InlineData(7, 11, 4, 7)]
    public void GetKth_LeetCodeExamples_ReturnsKthIntegerByPowerValue(int lo, int hi, int k, int expected)
    {
        var actual = GetKth(lo, hi, k);
        Assert.Equal(expected, actual);
    }

    private static int GetKth(int lo, int hi, int k)
    {
        var length = hi - lo + 1;
        var pairs = new (int Power, int Value)[length];

        for (var i = 0; i < length; i++)
        {
            var value = lo + i;
            pairs[i] = (PowerOf(value), value);
        }

        MergeSort.Sort<(int Power, int Value), ArrayIndexedSequence<(int Power, int Value)>>(
            new ArrayIndexedSequence<(int Power, int Value)>(pairs));

        return pairs[k - 1].Value;
    }

    private static int PowerOf(int x)
    {
        var power = 0;

        while (x != 1)
        {
            x = x % 2 == 0 ? x / 2 : (3 * x) + 1;
            power++;
        }

        return power;
    }
}
