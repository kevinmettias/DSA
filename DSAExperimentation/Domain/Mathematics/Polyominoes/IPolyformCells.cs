using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes;

// How a polyform's cells sit on a planar lattice. Each cell is a point of the lattice, written as
// its step from the origin, and two cells are joined when one of these steps takes one to the
// other: squares joined edge to edge make polyominoes, hexagons joined edge to edge polyhexes, and
// squares joined at an edge or a corner polyplets. The lattice's symmetries are the step type's own
// (IPlanarLatticeStep), and the joining steps must agree with them, a union of their orbits, so
// that a piece turned or mirrored is still connected.
//
// A polyform is listed in one order of its cells, which a translation must keep: a comes before b
// exactly when a + c comes before b + c. Translating a sorted list then leaves it sorted, so a
// polyform's first cell can be moved to the origin without sorting again. Ordering by one
// coordinate and then the next, as reading order goes row by row, is such an order.
/// <summary>How a polyform's cells are joined on a lattice, and the order they are in.</summary>
/// <typeparam name="TStep">The lattice's step type; a cell is its step from the origin.</typeparam>
public interface IPolyformCells<TStep>
    where TStep : struct, IPlanarLatticeStep<TStep>
{
    /// <summary>Gets the steps from a cell to every cell joined to it.</summary>
    static abstract ImmutableArray<TStep> JoiningSteps { get; }

    /// <summary>Orders two cells, in an order a translation of both keeps.</summary>
    /// <param name="left">The first cell.</param>
    /// <param name="right">The second cell.</param>
    /// <returns>Negative when the first comes first, zero when equal, else positive.</returns>
    static abstract int Compare(TStep left, TStep right);
}
