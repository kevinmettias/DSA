using DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Tilings;

// Each piece used exactly once is a primary column.
public sealed partial class ExactlyOnceTests
{
    [Fact]
    public void PrimaryColumnsFor_Pieces_IsOneEach() =>
        Assert.Equal(12, ExactlyOnce.PrimaryColumnsFor(12));

    [Fact]
    public void SecondaryColumnsFor_Pieces_IsNone() =>
        Assert.Equal(0, ExactlyOnce.SecondaryColumnsFor(12));
}
