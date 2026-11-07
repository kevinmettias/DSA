using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;
using DSAExperimentation.Tests.Domain.Mathematics.Lattices.Fixtures;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Fixtures;

// Hexagons joined edge to edge, test-only: the cells of a polyhex, which prove a polyform is not
// the square lattice's alone. A hexagon shares an edge with the six cells one step away, the orbit
// of one unit step under the lattice's twelve symmetries, and cells are ordered by q and then r.
internal readonly struct HexCells : IPolyformCells<HexStep>
{
    public static ImmutableArray<HexStep> JoiningSteps { get; } =
        LatticeSymmetry.Orbit(new HexStep(1, 0));

    public static int Compare(HexStep left, HexStep right)
    {
        var byQ = left.DeltaQ.CompareTo(right.DeltaQ);

        if (byQ != 0)
        {
            return byQ;
        }

        return left.DeltaR.CompareTo(right.DeltaR);
    }
}
