using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Mathematics.Lattices.Fixtures;

// A step on the hexagonal lattice in axial coordinates (q, r), the third cube coordinate being
// -q - r, test-only: it proves the lattice contract, and the polyforms built on it, are not the
// square's alone. A hexagonal grid's cells sit on the triangular lattice A2, whose smallest
// rotation is a sixth of a turn: in cube coordinates (x, y, z) goes to (-z, -x, -y), and
// swapping two cube coordinates mirrors it.
internal readonly record struct HexStep(int DeltaQ, int DeltaR) : IPlanarLatticeStep<HexStep>
{
    private const int SixthTurnsPerTurn = 6;

    public static int RotationOrder => SixthTurnsPerTurn;

    public static HexStep AdditiveIdentity { get; } = new(0, 0);

    // How many cells away the step lands: half the cube coordinates' absolute sum.
    public int Distance => (Math.Abs(DeltaQ) + Math.Abs(DeltaR) + Math.Abs(DeltaQ + DeltaR)) / 2;

    public static HexStep operator +(HexStep left, HexStep right) =>
        new(left.DeltaQ + right.DeltaQ, left.DeltaR + right.DeltaR);

    public static HexStep operator -(HexStep step) => new(-step.DeltaQ, -step.DeltaR);

    public static HexStep Rotate(HexStep step) => new(-step.DeltaR, step.DeltaQ + step.DeltaR);

    public static HexStep Reflect(HexStep step) => new(step.DeltaR, step.DeltaQ);
}
