using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;
using DSAExperimentation.Tests.Domain.Mathematics.Lattices.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Equivalences;

// The one-sided equivalence allows every turn of the lattice, one side up.
public sealed partial class OneSidedTests
{
    [Fact]
    public void Sides_IsOne() => Assert.Equal(1, OneSided.Sides);

    [Fact]
    public void TurnsOf_EachLattice_IsItsRotationOrder() =>
        Assert.Equal((4, 6), (OneSided.TurnsOf<GridStep>(), OneSided.TurnsOf<HexStep>()));
}
