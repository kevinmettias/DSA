namespace DSAExperimentation.Domain.Mathematics.NumberTheory;

// What every greatest-common-divisor procedure agrees on. Euclid's algorithm and the gcd folded
// over a segment tree's ranges are procedures, Algorithms/NumberTheory's GreatestCommonDivisor and
// GcdOperation; what holds whichever of them runs is stated here, and their tests hold them to it.
//
// Every integer divides 0, so the divisors 0 and x have in common are x's own, and gcd(0, x) = |x|.
// So 0 is the gcd's identity: the gcd of no numbers at all, where a running gcd starts before it
// has folded in a value. It is the only one, since gcd(e, x) = |x| for every x needs every x to
// divide e.
/// <summary>Facts about common divisors that every gcd procedure agrees on.</summary>
public static class CommonDivisors
{
    /// <summary>The gcd's identity, 0, the gcd of no numbers: gcd(0, x) = |x|.</summary>
    public const int GcdIdentity = 0;
}
