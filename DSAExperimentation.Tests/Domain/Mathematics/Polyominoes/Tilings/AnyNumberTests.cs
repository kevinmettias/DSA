using DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Tilings;

// A piece used any number of times has no column.
public sealed partial class AnyNumberTests
{
    [Fact]
    public void PrimaryColumnsFor_Pieces_IsNone() =>
        Assert.Equal(0, AnyNumber.PrimaryColumnsFor(12));

    [Fact]
    public void SecondaryColumnsFor_Pieces_IsNone() =>
        Assert.Equal(0, AnyNumber.SecondaryColumnsFor(12));
}
