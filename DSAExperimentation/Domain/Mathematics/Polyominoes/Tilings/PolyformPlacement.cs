using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

// One row of a tiling's exact cover: a piece in one orientation, put down with its first cell on
// a region cell. The cells it covers are the orientation's cells moved by that cell, since the
// orientation's first cell is the origin.
/// <summary>A piece in one orientation, put down with its first cell on a given cell.</summary>
/// <typeparam name="TStep">The lattice's step type.</typeparam>
/// <typeparam name="TCells">How cells are joined and ordered.</typeparam>
/// <param name="Piece">The piece's index in the tiling's list of pieces.</param>
/// <param name="Orientation">The orientation the piece is put down in.</param>
/// <param name="Anchor">The cell the orientation's first cell lands on.</param>
public readonly record struct PolyformPlacement<TStep, TCells>(
    int Piece, Polyform<TStep, TCells> Orientation, TStep Anchor)
    where TStep : struct, IPlanarLatticeStep<TStep>
    where TCells : struct, IPolyformCells<TStep>;
