using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.SetTheory;

namespace DSAExperimentation.Domain.Mathematics.NumberSystems;

// A number system as mathematics sees it: the laws its arithmetic keeps, and how many elements it
// has. Its name is a language's (Language/English/NumberNames) and its symbol, such as the
// double-struck N, a notation's, so neither is carried here.
//
// The systems are named values, each built from the one before it by what it adds or gives up, so
// no system's laws are written twice.
//
// The hierarchy: the naturals sit inside the integers, which add subtraction; the rationals add
// division, and lose completeness; the reals restore it, and have the continuum's size where the
// three before have ℵ0; the complex numbers add a square root of -1, and with it a root for every
// polynomial, and give up order, since in an ordered system no square is negative.
//
// From the reals, the Cayley-Dickson construction doubles a system: a pair (a, b) of its elements,
// multiplied as (a, b)(c, d) = (ac - d*b, da + bc*), where * is conjugation. Each doubling doubles
// the dimension over the reals, 1, 2, 4, 8, 16, and gives up a law: the complex numbers order, the
// quaternions commutativity, the octonions associativity, keeping alternativity and division, and
// the sedenions those two, having zero divisors. The reals, complex numbers, quaternions and
// octonions are the only division algebras over the reals whose norm multiplies (Hurwitz, 1898).
/// <summary>
/// A number system, the laws its arithmetic keeps and its size; the systems from the naturals to
/// the sedenions.
/// </summary>
/// <param name="Laws">The laws its addition and multiplication keep.</param>
/// <param name="Cardinality">The number of its elements: ℵ0 or the continuum.</param>
public readonly record struct NumberSystem(AlgebraicLaws Laws, Cardinal Cardinality)
{
    /// <summary>
    /// Gets the naturals 0, 1, 2, ...: ordered, commutative and associative, and complete.
    /// </summary>
    public static NumberSystem Naturals { get; } =
        new(
            AlgebraicLaws.Completeness | AlgebraicLaws.Order | AlgebraicLaws.Commutativity
                | AlgebraicLaws.Associativity | AlgebraicLaws.Alternativity,
            Cardinal.AlephNull);

    /// <summary>
    /// Gets the integers: the naturals with their negatives, so subtraction stays inside.
    /// </summary>
    public static NumberSystem Integers { get; } =
        Naturals with { Laws = Naturals.Laws | AlgebraicLaws.Subtraction };

    /// <summary>
    /// Gets the rationals: the integers' quotients, which add division and lose completeness.
    /// </summary>
    public static NumberSystem Rationals { get; } =
        Integers with
        {
            Laws = (Integers.Laws | AlgebraicLaws.Division) & ~AlgebraicLaws.Completeness,
        };

    /// <summary>Gets the reals: the rationals completed, of the continuum's size.</summary>
    public static NumberSystem Reals { get; } =
        new(Rationals.Laws | AlgebraicLaws.Completeness, Cardinal.Continuum);

    /// <summary>Gets the complex numbers: the reals doubled, which gives up order.</summary>
    public static NumberSystem Complex { get; } = Doubled(Reals, AlgebraicLaws.Order);

    /// <summary>
    /// Gets the quaternions: the complex numbers doubled, which gives up commutativity.
    /// </summary>
    public static NumberSystem Quaternions { get; } = Doubled(Complex, AlgebraicLaws.Commutativity);

    /// <summary>
    /// Gets the octonions: the quaternions doubled, which gives up associativity.
    /// </summary>
    public static NumberSystem Octonions { get; } =
        Doubled(Quaternions, AlgebraicLaws.Associativity);

    /// <summary>
    /// Gets the sedenions: the octonions doubled, which gives up alternativity and division.
    /// </summary>
    public static NumberSystem Sedenions { get; } =
        Doubled(Octonions, AlgebraicLaws.Alternativity | AlgebraicLaws.Division);

    /// <summary>
    /// Gets the systems each contained in the next: naturals, integers, rationals, reals, complex.
    /// </summary>
    public static ImmutableArray<NumberSystem> Hierarchy { get; } =
        [Naturals, Integers, Rationals, Reals, Complex];

    /// <summary>
    /// Gets the Cayley-Dickson doublings from the reals, the k-th of dimension 2^k over them.
    /// </summary>
    public static ImmutableArray<NumberSystem> CayleyDickson { get; } =
        [Reals, Complex, Quaternions, Octonions, Sedenions];

    /// <summary>Whether the system's arithmetic keeps every one of some laws.</summary>
    /// <param name="laws">The laws to check.</param>
    /// <returns>True when the system keeps all of them.</returns>
    public bool HasLaws(AlgebraicLaws laws) => (Laws & laws) == laws;

    // A doubling keeps the system's size, since a pair of continuum-sized sets is continuum-sized.
    private static NumberSystem Doubled(NumberSystem system, AlgebraicLaws givenUp) =>
        system with { Laws = system.Laws & ~givenUp };
}
