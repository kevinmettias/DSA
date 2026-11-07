using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;

// Which images of a piece count as the same piece. The lattice's symmetries that keep the origin
// cell where it is are its point group: turns by its smallest rotation, and each of those after a
// mirror. A piece's images under them, moved back so their first cell is at the origin, are the
// piece turned and turned over. An equivalence is a subgroup of that group, written as how many of
// the turns a piece may take and how many of its sides may face up:
// - fixed: one turn, the identity, and one side, so only a translate is the same piece;
// - one-sided: every turn but one side, as a piece slid about a table without being lifted;
// - free: every turn and both sides, as a piece that may be picked up and turned over.
// The subgroup's order is the product, so 1, n and 2n on a lattice that turns in n steps: 1, 4 and
// 8 on the square lattice, 1, 6 and 12 on the hexagonal one. Each is a constant of the witness and
// the lattice, which the loops over a piece's images fold.
/// <summary>Which turns and sides of a piece count as the same piece.</summary>
public interface IPolyformEquivalence
{
    /// <summary>Gets how many sides of a piece may face up: 1, or 2 when it turns over.</summary>
    static abstract int Sides { get; }

    /// <summary>Counts the turns a piece may take, the identity among them.</summary>
    /// <typeparam name="TStep">The lattice's step type.</typeparam>
    /// <returns>1 when only the identity is allowed, or the lattice's rotation order.</returns>
    static abstract int TurnsOf<TStep>()
        where TStep : struct, IPlanarLatticeStep<TStep>;
}
