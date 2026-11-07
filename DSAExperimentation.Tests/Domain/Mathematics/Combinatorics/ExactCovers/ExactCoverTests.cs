using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;
using DSAExperimentation.Tests.Domain.Mathematics.Combinatorics.ExactCovers.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Combinatorics.ExactCovers;

// The rule against Knuth's example, whose only cover is rows 0, 3 and 4, and a matrix with a
// secondary column, which a cover may meet once or leave alone but never meet twice.
public sealed partial class ExactCoverTests
{
    // Sets of Knuth's rows that are not covers: a column left over, a column met twice, a row
    // listed twice.
    public static TheoryData<int[]> KnuthNonCovers =>
        new() { { [] }, { [3, 0] }, { [3, 0, 4, 2] }, { [3, 0, 4, 4] } };

    // Covers of the matrix with a secondary column: one leaving it alone, one meeting it once.
    public static TheoryData<int[]> SecondaryColumnCovers => new() { { [2, 3] }, { [0, 3] } };

    // Rows of that matrix that are no cover: the secondary column met twice, a primary missed.
    public static TheoryData<int[]> SecondaryColumnNonCovers => new() { { [0, 1] }, { [0] } };

    [Fact]
    public void IsCover_KnuthsCoverInAnyOrder_IsTrue()
    {
        Assert.True(ExactCover<MatrixProblem>.IsCover(CoverMatrices.Knuth, [0, 3, 4]));
        Assert.True(ExactCover<MatrixProblem>.IsCover(CoverMatrices.Knuth, [4, 0, 3]));
    }

    [Theory]
    [MemberData(nameof(KnuthNonCovers))]
    public void IsCover_ColumnMissedOrMetTwice_IsFalse(int[] rows) =>
        Assert.False(ExactCover<MatrixProblem>.IsCover(CoverMatrices.Knuth, rows));

    [Theory]
    [MemberData(nameof(SecondaryColumnCovers))]
    public void IsCover_SecondaryColumnMetOnceOrNot_IsTrue(int[] rows) =>
        Assert.True(ExactCover<MatrixProblem>.IsCover(CoverMatrices.WithSecondary, rows));

    [Theory]
    [MemberData(nameof(SecondaryColumnNonCovers))]
    public void IsCover_SecondaryColumnMetTwiceOrPrimaryMissed_IsFalse(int[] rows) =>
        Assert.False(ExactCover<MatrixProblem>.IsCover(CoverMatrices.WithSecondary, rows));

    [Fact]
    public void IsCover_RowOutOfRange_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ExactCover<MatrixProblem>.IsCover(CoverMatrices.Knuth, [6]));
}
