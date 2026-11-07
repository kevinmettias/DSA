using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Equivalences;

// Fixed polyforms: no turn and no turning over, so two pieces are the same only when one is a
// translate of the other. There are 19 fixed tetrominoes and 63 fixed pentominoes (OEIS A001168).
/// <summary>The fixed equivalence: a piece is the same only as its translates.</summary>
public readonly struct Fixed : IPolyformEquivalence
{
    // The identity, alone of the turns, and the one side.
    private const int IdentityOnly = 1;

    /// <inheritdoc/>
    public static int Sides => IdentityOnly;

    /// <inheritdoc/>
    public static int TurnsOf<TStep>()
        where TStep : struct, IPlanarLatticeStep<TStep> => IdentityOnly;
}
