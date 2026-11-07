using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;
using DSAExperimentation.Tests.Domain.Mathematics.Lattices.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Equivalences;

// The fixed equivalence allows the identity alone, on any lattice.
public sealed partial class FixedTests
{
    [Fact]
    public void Sides_IsOne() => Assert.Equal(1, Fixed.Sides);

    [Fact]
    public void TurnsOf_AnyLattice_IsTheIdentityAlone() =>
        Assert.Equal((1, 1), (Fixed.TurnsOf<GridStep>(), Fixed.TurnsOf<HexStep>()));
}
