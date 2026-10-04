using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

// Row 4 is 1 4 6 4 1, small enough to read; row 66's middle entry is the largest coefficient long can
// hold, 7,219,428,434,016,265,740. The modulo cases use 10 and 4, composite moduli where most
// factorials have no inverse - the reason this type exists beside FactorialTable.
public sealed partial class PascalTriangleTests
{
    public static TheoryData<int, int, long> CoefficientsOfRowFive =>
        new() { { 5, 2, 10L }, { 5, 0, 1L }, { 5, 5, 1L }, { 0, 0, 1L } };

    public static TheoryData<int, int> ImpossibleChoices =>
        new() { { 5, 6 }, { 5, -1 }, { -1, 0 } };

    // C(5, 2) = 10 and C(6, 3) = 20, both 0 mod 10; C(4, 1) = 4.
    public static TheoryData<int, int, long> CoefficientsModTen =>
        new() { { 5, 2, 0L }, { 6, 3, 0L }, { 4, 1, 4L } };

    public static TheoryData<long> ModuliOutOfRange =>
        new() { 0L, (1L << 62) + 1 };

    [Fact]
    public void Exact_SmallTriangle_HasTheRowsUpToMaxRow() =>
        Assert.Equal(4, PascalTriangle.Exact(4).MaxRow);

    [Fact]
    public void Exact_PastRowSixtySix_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PascalTriangle.Exact(PascalTriangle.MaxExactRow + 1));

    [Fact]
    public void Exact_RowSixtySix_HoldsTheMiddleCoefficientExactly() =>
        Assert.Equal(7_219_428_434_016_265_740L, PascalTriangle.Exact(PascalTriangle.MaxExactRow).Choose(66, 33));

    [Fact]
    public void Row_Four_IsOneFourSixFourOne() =>
        Assert.Equal([1L, 4L, 6L, 4L, 1L], PascalTriangle.Exact(6).Row(4).ToArray());

    [Fact]
    public void Row_PastMaxRow_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PascalTriangle.Exact(3).Row(4).ToArray());

    [Theory]
    [MemberData(nameof(CoefficientsOfRowFive))]
    public void Choose_InsideTheRow_ReturnsTheCoefficient(int totalCount, int chosenCount, long expected) =>
        Assert.Equal(expected, PascalTriangle.Exact(5).Choose(totalCount, chosenCount));

    [Theory]
    [MemberData(nameof(ImpossibleChoices))]
    public void Choose_ImpossibleChoice_ReturnsZero(int totalCount, int chosenCount) =>
        Assert.Equal(0L, PascalTriangle.Exact(5).Choose(totalCount, chosenCount));

    [Fact]
    public void Choose_RowPastMaxRow_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PascalTriangle.Exact(5).Choose(6, 1));

    [Theory]
    [MemberData(nameof(CoefficientsModTen))]
    public void Modulo_CompositeModulus_ReducesEveryCoefficient(int totalCount, int chosenCount, long expected) =>
        Assert.Equal(expected, PascalTriangle.Modulo(10, 10).Choose(totalCount, chosenCount));

    // Every entry of rows 0..100 reduced mod 4 agrees with the exact coefficient reduced mod 4 where
    // the exact one fits, so the modular recurrence is the exact one, reduced.
    [Fact]
    public void Modulo_AgreesWithExactReducedByTheModulus()
    {
        var exact = PascalTriangle.Exact(PascalTriangle.MaxExactRow);
        var modular = PascalTriangle.Modulo(PascalTriangle.MaxExactRow, 4);

        for (var row = 0; row <= PascalTriangle.MaxExactRow; row++)
        {
            for (var column = 0; column <= row; column++)
            {
                Assert.Equal(exact.Choose(row, column) % 4, modular.Choose(row, column));
            }
        }
    }

    [Fact]
    public void Modulo_ModulusOne_ReducesEverythingToZero() =>
        Assert.Equal(0L, PascalTriangle.Modulo(3, 1).Choose(0, 0));

    [Theory]
    [MemberData(nameof(ModuliOutOfRange))]
    public void Modulo_ModulusOutOfRange_Throws(long modulus) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => PascalTriangle.Modulo(3, modulus));

    [Fact]
    public void MaxRow_IsTheRowTheTriangleWasBuiltTo() =>
        Assert.Equal(12, PascalTriangle.Modulo(12, 7).MaxRow);
}
