using DSAExperimentation.LeetCode.SortIntegersByThePowerValue;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortIntegersByThePowerValue;

// Harness only. Both strategies are SortIntegersByThePowerValueSolution's - this
// repo's MergeSort over ArrayIndexedSequence and the insertion sort that used to live
// untested as the benchmark's baseline - pinned to LeetCode's published examples plus
// a single-value range and a case whose kth falls on a power tie.
public sealed class SortIntegersByThePowerValueTests
{
    public static TheoryData<int, int, int, int> Examples =>
        new()
        {
            { 12, 15, 2, 13 },
            { 7, 11, 4, 7 },
            { 10, 20, 5, 13 },
            { 1, 1, 1, 1 },
            { 1, 4, 4, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetKthByMergeSort_LeetCodeExamples_ReturnsKthIntegerByPowerValue(
        int lo, int hi, int rank, int expected)
    {
        var actual = SortIntegersByThePowerValueSolution.GetKthByMergeSort(lo, hi, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetKthByInsertionSort_LeetCodeExamples_ReturnsKthIntegerByPowerValue(
        int lo, int hi, int rank, int expected)
    {
        var actual = SortIntegersByThePowerValueSolution.GetKthByInsertionSort(lo, hi, rank);

        Assert.Equal(expected, actual);
    }
}
