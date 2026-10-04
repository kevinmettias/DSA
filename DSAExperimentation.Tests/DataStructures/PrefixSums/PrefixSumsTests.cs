using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.PrefixSums;

namespace DSAExperimentation.Tests.DataStructures.PrefixSums;

// Values is short enough to total by hand: the running sums are 0, 3, 4, 8, 9, 14. The xor cases use a
// sequence whose ranges cancel, which a sum could never do, so a test that passes for xor is one where
// Invert really was the identity and not negation.
public sealed partial class PrefixSumsTests
{
    private static readonly long[] Values = [3, 1, 4, 1, 5];

    public static TheoryData<int, long> EndsToTotalsBefore =>
        new() { { 0, 0L }, { 1, 3L }, { 3, 8L }, { 5, 14L } };

    public static TheoryData<int> EndsOutsideZeroToCount =>
        new() { -1, 6 };

    public static TheoryData<int, int, long> InclusiveRangesToSums =>
        new() { { 0, 0, 3L }, { 1, 3, 6L }, { 0, 4, 14L }, { 4, 4, 5L } };

    public static TheoryData<int, int> RangesOutOfBoundsOrReversed =>
        new() { { -1, 2 }, { 2, 5 }, { 3, 2 } };

    [Fact]
    public void Count_IsTheNumberOfValues() =>
        Assert.Equal(5, new PrefixSums<long, SumOperation<long>>(Values).Count);

    [Theory]
    [MemberData(nameof(EndsToTotalsBefore))]
    public void TotalBefore_End_CombinesTheValuesBeforeIt(int end, long expected) =>
        Assert.Equal(expected, new PrefixSums<long, SumOperation<long>>(Values).TotalBefore(end));

    [Theory]
    [MemberData(nameof(EndsOutsideZeroToCount))]
    public void TotalBefore_EndOutsideZeroToCount_Throws(int end) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PrefixSums<long, SumOperation<long>>(Values).TotalBefore(end));

    [Theory]
    [MemberData(nameof(InclusiveRangesToSums))]
    public void Query_InclusiveRange_SumsBothEnds(int left, int right, long expected) =>
        Assert.Equal(expected, new PrefixSums<long, SumOperation<long>>(Values).Query(left, right));

    [Theory]
    [MemberData(nameof(RangesOutOfBoundsOrReversed))]
    public void Query_RangeOutOfBoundsOrReversed_Throws(int left, int right) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PrefixSums<long, SumOperation<long>>(Values).Query(left, right));

    // 6 ^ 5 ^ 3 == 0, so the xor of [0, 2] cancels while its sum is 14.
    [Fact]
    public void Query_XorOperation_CancelsRepeatedBits() =>
        Assert.Equal(0, new PrefixSums<int, XorOperation<int>>([6, 5, 3, 7]).Query(0, 2));

    [Fact]
    public void Query_XorOperation_RecoversOneValue() =>
        Assert.Equal(7, new PrefixSums<int, XorOperation<int>>([6, 5, 3, 7]).Query(3, 3));

    [Fact]
    public void Totals_IsEveryTotalBeforeInOrder()
    {
        var totals = new PrefixSums<long, SumOperation<long>>(Values).Totals;

        Assert.Equal(6, totals.Length);
        Assert.Equal([0L, 3L, 4L, 8L, 9L, 14L], Enumerable.Range(0, totals.Length).Select(totals.Get));
    }

    // Non-negative addends keep the totals sorted, so BinarySearch can take them directly: the first
    // total of at least 8 sits after the first three values.
    [Fact]
    public void Totals_NonNegativeSums_CanBeBinarySearched() =>
        Assert.Equal(3, BinarySearch.LowerBound(new PrefixSums<long, SumOperation<long>>(Values).Totals, 8L));

    [Fact]
    public void PrefixSums_NoValues_HasOneTotalOfIdentity()
    {
        var empty = new PrefixSums<long, SumOperation<long>>([]);

        Assert.Equal(0, empty.Count);
        Assert.Equal(0L, empty.TotalBefore(0));
    }
}
