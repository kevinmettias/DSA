using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;

// Free polyforms: any turn, and either side up, as pieces that may be picked up and turned over,
// so a piece and its mirror image are one. There are 5 free tetrominoes and 12 free pentominoes
// (OEIS A000105).
/// <summary>The free equivalence: a piece is the same as its turns and mirror images.</summary>
public readonly struct Free : IPolyformEquivalence
{
    // A piece and its mirror image: the side it lies on and the other.
    private const int BothSides = 2;

    /// <inheritdoc/>
    public static int Sides => BothSides;

    /// <inheritdoc/>
    public static int TurnsOf<TStep>()
        where TStep : struct, IPlanarLatticeStep<TStep> => TStep.RotationOrder;
}
