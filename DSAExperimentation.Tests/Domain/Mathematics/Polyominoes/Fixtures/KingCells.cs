using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Polyominoes;

namespace DSAExperimentation.Tests.Domain.Mathematics.Polyominoes.Fixtures;

// Squares joined at an edge or a corner, test-only: the cells of a polyplet (a polyking, joined as
// a king moves), which prove the joining steps are an axis of their own. The lattice and its
// symmetries are the polyomino's; only which squares count as touching differs, so the order is
// SquareCells' reading order.
internal readonly struct KingCells : IPolyformCells<GridStep>
{
    public static ImmutableArray<GridStep> JoiningSteps => GridDirections.Octilinear;

    public static int Compare(GridStep left, GridStep right) => SquareCells.Compare(left, right);
}
