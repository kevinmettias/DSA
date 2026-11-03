using System.Numerics;

namespace DSAExperimentation.Domain.Mathematics.Lattices;

// A step on a planar lattice, with the algebra that makes the lattice what it is: steps add, each
// has an opposite, and the lattice's symmetries map steps to steps. A lattice's rotations can only
// be of order 2, 3, 4 or 6 (the crystallographic restriction); the square lattice turns in
// quarters and the hexagonal one in sixths. With one reflection, the smallest rotation generates
// the lattice's point group, the dihedral group of twice RotationOrder symmetries. A set of steps
// agrees with the lattice when every symmetry maps it onto itself, which makes it a union of the
// orbits LatticeSymmetry lists.
//
// The algebra sits on the step type rather than on a separate witness, so a generic method infers
// the lattice from the step it is handed, and operators can be declared at all.
/// <summary>A step on a planar lattice: steps add and negate, and the lattice's rotation and reflection map steps to steps.</summary>
/// <typeparam name="TSelf">The step type itself.</typeparam>
public interface IPlanarLatticeStep<TSelf> :
    IAdditionOperators<TSelf, TSelf, TSelf>,
    IUnaryNegationOperators<TSelf, TSelf>,
    IAdditiveIdentity<TSelf, TSelf>,
    IEquatable<TSelf>
    where TSelf : IPlanarLatticeStep<TSelf>
{
    /// <summary>Gets how many of the lattice's smallest rotations make a whole turn.</summary>
    static abstract int RotationOrder { get; }

    /// <summary>Turns a step by the lattice's smallest rotation, clockwise as the lattice is drawn.</summary>
    /// <param name="step">The step to turn.</param>
    /// <returns>The step turned.</returns>
    static abstract TSelf Rotate(TSelf step);

    /// <summary>Mirrors a step in one of the lattice's lines of symmetry.</summary>
    /// <param name="step">The step to mirror.</param>
    /// <returns>The step's mirror image.</returns>
    static abstract TSelf Reflect(TSelf step);
}
