using DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Tilings;

// Each piece used at most once is a secondary column.
public sealed partial class AtMostOnceTests
{
    [Fact]
    public void PrimaryColumnsFor_Pieces_IsNone() =>
        Assert.Equal(0, AtMostOnce.PrimaryColumnsFor(12));

    [Fact]
    public void SecondaryColumnsFor_Pieces_IsOneEach() =>
        Assert.Equal(12, AtMostOnce.SecondaryColumnsFor(12));
}
