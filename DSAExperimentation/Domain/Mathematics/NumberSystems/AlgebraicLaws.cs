namespace DSAExperimentation.Domain.Mathematics.NumberSystems;

// The laws a number system's arithmetic may keep, as flags, so a system's laws are one value and
// what an extension adds or a doubling gives up is the difference of two. Each law is stated for
// the system's own addition and multiplication.
//
// Completeness is a metric space's: every Cauchy sequence has a limit inside. The naturals and the
// integers have it because their points stand apart, so a Cauchy sequence of them settles on one
// value; the rationals lose it (their sequences close in on the square root of 2) and the reals
// restore it. Algebraic closure is not a law here: the complex numbers are the field in which every
// polynomial has a root, but the quaternions and octonions are not fields, and whether their
// one-sided polynomials have roots is a theorem about them (Eilenberg and Niven, 1944), not a law
// their arithmetic keeps.
/// <summary>Laws a number system's arithmetic may keep.</summary>
[Flags]
public enum AlgebraicLaws
{
    /// <summary>No law.</summary>
    None = 0,

    /// <summary>Every element has a negative, so subtracting stays inside the system.</summary>
    Subtraction = 1 << 0,

    /// <summary>
    /// Dividing by any nonzero element stays inside and has one answer: no zero divisors.
    /// </summary>
    Division = 1 << 1,

    /// <summary>Every Cauchy sequence of the system's elements has a limit in the system.</summary>
    Completeness = 1 << 2,

    /// <summary>
    /// A total order that adding preserves, and multiplying by a positive element.
    /// </summary>
    Order = 1 << 3,

    /// <summary>Multiplication commutes: xy = yx.</summary>
    Commutativity = 1 << 4,

    /// <summary>Multiplication associates: (xy)z = x(yz).</summary>
    Associativity = 1 << 5,

    /// <summary>
    /// Multiplication associates when two factors are equal: (xx)y = x(xy), (yx)x = y(xx).
    /// </summary>
    Alternativity = 1 << 6,
}
