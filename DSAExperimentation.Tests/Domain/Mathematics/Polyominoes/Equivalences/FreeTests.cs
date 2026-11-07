using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;
using DSAExperimentation.Tests.Domain.Mathematics.Lattices.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Equivalences;

// The free equivalence allows every turn of the lattice, either side up.
public sealed partial class FreeTests
{
    [Fact]
    public void Sides_IsTwo() => Assert.Equal(2, Free.Sides);

    [Fact]
    public void TurnsOf_EachLattice_IsItsRotationOrder() =>
        Assert.Equal((4, 6), (Free.TurnsOf<GridStep>(), Free.TurnsOf<HexStep>()));
}
