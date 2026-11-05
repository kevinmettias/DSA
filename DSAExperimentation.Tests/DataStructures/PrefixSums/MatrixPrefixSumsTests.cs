using DSAExperimentation.Domain.Mathematics.Algebra;
using DSAExperimentation.DataStructures.PrefixSums;

namespace DSAExperimentation.Tests.DataStructures.PrefixSums;

// Values is small enough to total by hand: its top-left rectangles sum to 1, 3, 6 along the first row
// and 5, 12, 21 once the second row is in. The xor cases use a matrix whose values cancel, which a
// sum could never do, so a test that passes for xor is one where Invert really was the identity and
// not negation.
public sealed partial class MatrixPrefixSumsTests
{
    private static readonly long[][] Values = [[1, 2, 3], [4, 5, 6]];

    private static readonly int[][] CancellingValues = [[6, 5], [3, 0]];

    public static TheoryData<int, int, long> EndsToTotalsBefore =>
        new() { { 0, 0, 0L }, { 0, 3, 0L }, { 1, 1, 1L }, { 1, 3, 6L }, { 2, 1, 5L }, { 2, 2, 12L }, { 2, 3, 21L } };

    public static TheoryData<int, int> EndsOutsideZeroToCount =>
        new() { { -1, 0 }, { 3, 0 }, { 0, -1 }, { 0, 4 } };

    public static TheoryData<RectangleSum> InclusiveRectanglesToSums =>
        new()
        {
            new RectangleSum(Top: 0, Left: 0, Bottom: 0, Right: 0, Expected: 1L),
            new RectangleSum(Top: 0, Left: 0, Bottom: 1, Right: 2, Expected: 21L),
            new RectangleSum(Top: 1, Left: 1, Bottom: 1, Right: 2, Expected: 11L),
            new RectangleSum(Top: 0, Left: 1, Bottom: 1, Right: 1, Expected: 7L),
            new RectangleSum(Top: 0, Left: 2, Bottom: 1, Right: 2, Expected: 9L),
        };

    public static TheoryData<int, int, int, int> RectanglesOutOfBoundsOrReversed =>
        new() { { -1, 0, 0, 0 }, { 0, 0, 2, 0 }, { 0, -1, 0, 0 }, { 0, 0, 0, 3 }, { 1, 0, 0, 0 }, { 0, 2, 0, 1 } };

    [Fact]
    public void RowCount_IsTheNumberOfRows() =>
        Assert.Equal(2, new MatrixPrefixSums<long, Addition<long>>(Values).RowCount);

    [Fact]
    public void ColumnCount_IsTheNumberOfValuesInARow() =>
        Assert.Equal(3, new MatrixPrefixSums<long, Addition<long>>(Values).ColumnCount);

    [Theory]
    [MemberData(nameof(EndsToTotalsBefore))]
    public void TotalBefore_Ends_CombinesTheTopLeftRectangle(int rowEnd, int columnEnd, long expected) =>
        Assert.Equal(expected, new MatrixPrefixSums<long, Addition<long>>(Values).TotalBefore(rowEnd, columnEnd));

    [Theory]
    [MemberData(nameof(EndsOutsideZeroToCount))]
    public void TotalBefore_EndOutsideZeroToCount_Throws(int rowEnd, int columnEnd) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new MatrixPrefixSums<long, Addition<long>>(Values).TotalBefore(rowEnd, columnEnd));

    [Theory]
    [MemberData(nameof(InclusiveRectanglesToSums))]
    public void Query_InclusiveRectangle_SumsEveryEdge(RectangleSum rectangle) =>
        Assert.Equal(
            rectangle.Expected,
            new MatrixPrefixSums<long, Addition<long>>(Values).Query(rectangle.Top, rectangle.Left, rectangle.Bottom, rectangle.Right));

    [Theory]
    [MemberData(nameof(RectanglesOutOfBoundsOrReversed))]
    public void Query_RectangleOutOfBoundsOrReversed_Throws(int top, int left, int bottom, int right) =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new MatrixPrefixSums<long, Addition<long>>(Values).Query(top, left, bottom, right));

    // 6 ^ 5 ^ 3 ^ 0 == 0, so the whole matrix xors to 0 while its sum is 14.
    [Fact]
    public void Query_XorOperation_CancelsRepeatedBits() =>
        Assert.Equal(0, new MatrixPrefixSums<int, BitwiseXor<int>>(CancellingValues).Query(0, 0, 1, 1));

    [Fact]
    public void Query_XorOperation_RecoversOneValue() =>
        Assert.Equal(5, new MatrixPrefixSums<int, BitwiseXor<int>>(CancellingValues).Query(0, 1, 0, 1));

    [Fact]
    public void MatrixPrefixSums_RaggedRows_Throws() =>
        Assert.Throws<ArgumentException>(() => new MatrixPrefixSums<long, Addition<long>>([[1, 2], [3]]));

    [Fact]
    public void MatrixPrefixSums_NoRows_HasOneTotalOfIdentity()
    {
        var empty = new MatrixPrefixSums<long, Addition<long>>([]);

        Assert.Equal(0, empty.RowCount);
        Assert.Equal(0, empty.ColumnCount);
        Assert.Equal(0L, empty.TotalBefore(0, 0));
    }

    // One inclusive rectangle of Values, by its four sides, and the sum of the values inside it.
    public readonly record struct RectangleSum(int Top, int Left, int Bottom, int Right, long Expected);
}
