using DSAExperimentation.LeetCode.Solutions.SortIntegersByThePowerValue;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SortIntegersByThePowerValue;

// Harness only. Both strategies are SortIntegersByThePowerValueSolution's - this
// repo's MergeSort over ArrayIndexedSequence and the insertion sort that used to live
// untested as the benchmark's baseline - pinned to a range whose kth integer is the
// last in power order: 3's power of 7 puts it after 4, whose power is 2. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class SortIntegersByThePowerValueSolutionTests
{
    public static TheoryData<int, int, int, int> Cases =>
        new()
        {
            { 1, 4, 4, 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetKthByMergeSort_Cases_ReturnsKthIntegerByPowerValue(
        int lo, int hi, int rank, int expected)
    {
        var actual = SortIntegersByThePowerValueSolution.GetKthByMergeSort(lo, hi, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetKthByInsertionSort_Cases_ReturnsKthIntegerByPowerValue(
        int lo, int hi, int rank, int expected)
    {
        var actual = SortIntegersByThePowerValueSolution.GetKthByInsertionSort(lo, hi, rank);

        Assert.Equal(expected, actual);
    }
}
