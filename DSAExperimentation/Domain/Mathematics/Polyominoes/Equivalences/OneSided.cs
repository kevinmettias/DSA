using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;

// One-sided polyforms: any turn, but never turned over, as pieces slid about a table. A chiral
// piece and its mirror image are then two pieces, as Tetris's L and J are. There are 7 one-sided
// tetrominoes and 18 one-sided pentominoes (OEIS A000988).
/// <summary>The one-sided equivalence: a piece is the same as its turns alone.</summary>
public readonly struct OneSided : IPolyformEquivalence
{
    // A piece slid about a table keeps one side up.
    private const int OneSide = 1;

    /// <inheritdoc/>
    public static int Sides => OneSide;

    /// <inheritdoc/>
    public static int TurnsOf<TStep>()
        where TStep : struct, IPlanarLatticeStep<TStep> => TStep.RotationOrder;
}
