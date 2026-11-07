namespace DSAExperimentation.Domain.Mathematics.SetTheory;

// An ordinal number, the length of a well-ordered list, here those below ω², each written in Cantor
// normal form as ω·a + b: a copies of the naturals one after another, then b more elements. The
// finite ordinals are the naturals; ω, the first infinite one, is the length of 0, 1, 2, ...; ω + 1
// is that list with one element after all of it.
//
// Addition puts one list after another, so it does not commute: 1 + ω puts one element before
// the naturals, which is the naturals again (ω), while ω + 1 puts one after them, which is longer.
// Written as a rule, a finite part is absorbed by any ω that follows it: (ω·a + b) + (ω·c + d) is
// ω·(a + c) + d when c > 0, and ω·a + (b + d) otherwise. Sizes add as cardinals do (Cardinal), and
// ω + 1, ω and 1 + ω all have ℵ0 elements, so an ordinal's cardinality forgets its order.
//
// The ordinals below ω² are as far as this goes exactly: addition stays below ω², but ω·ω does not,
// so multiplication is not offered.
/// <summary>An ordinal below ω², in Cantor normal form ω·a + b.</summary>
public readonly record struct Ordinal : IComparable<Ordinal>
{
    /// <summary>Gets the empty list's length, 0.</summary>
    public static Ordinal Zero { get; } = new(0, 0);

    /// <summary>Gets the length of a list of one element, 1.</summary>
    public static Ordinal One { get; } = new(0, 1);

    /// <summary>
    /// Gets ω, the length of the naturals in their order, the first infinite ordinal.
    /// </summary>
    public static Ordinal Omega { get; } = new(1, 0);

    /// <summary>Gets a, the number of copies of ω in ω·a + b.</summary>
    public long OmegaCoefficient { get; }

    /// <summary>Gets b, the finite part after the copies of ω in ω·a + b.</summary>
    public long FinitePart { get; }

    /// <summary>Gets a value indicating whether the ordinal is a natural number.</summary>
    public bool IsFinite => OmegaCoefficient == 0;

    /// <summary>
    /// Gets a value indicating whether the ordinal is a limit: not zero, and no successor.
    /// </summary>
    public bool IsLimit => OmegaCoefficient > 0 && FinitePart == 0;

    /// <summary>
    /// Gets the ordinal's cardinality: its finite part when finite, otherwise ℵ0.
    /// </summary>
    public Cardinal Cardinality => IsFinite ? Cardinal.Finite(FinitePart) : Cardinal.AlephNull;

    private Ordinal(long omegaCoefficient, long finitePart)
    {
        OmegaCoefficient = omegaCoefficient;
        FinitePart = finitePart;
    }

    /// <summary>The ordinal ω·a + b.</summary>
    /// <param name="omegaCoefficient">The copies of ω, a, at least zero.</param>
    /// <param name="finitePart">The finite part, b, at least zero.</param>
    /// <returns>The ordinal ω·a + b.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Either part is negative.</exception>
    public static Ordinal Of(long omegaCoefficient, long finitePart)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(omegaCoefficient);
        ArgumentOutOfRangeException.ThrowIfNegative(finitePart);
        return new(omegaCoefficient, finitePart);
    }

    /// <summary>The finite ordinal of a natural number.</summary>
    /// <param name="value">The natural number, at least zero.</param>
    /// <returns>The ordinal ω·0 + value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is negative.
    /// </exception>
    public static Ordinal Finite(long value) => Of(0, value);

    /// <summary>The length of this list followed by another.</summary>
    /// <param name="other">The list placed after this one.</param>
    /// <returns>
    /// The ordinal sum, which drops this finite part when the other has a copy of ω.
    /// </returns>
    public Ordinal Plus(Ordinal other)
    {
        if (other.OmegaCoefficient > 0)
        {
            return new(checked(OmegaCoefficient + other.OmegaCoefficient), other.FinitePart);
        }

        return new(OmegaCoefficient, checked(FinitePart + other.FinitePart));
    }

    /// <summary>The successor, this list with one more element at its end.</summary>
    /// <returns>This ordinal plus one.</returns>
    public Ordinal Successor() => Plus(One);

    /// <summary>Compares two ordinals by which list is a starting part of the other.</summary>
    /// <param name="other">The ordinal to compare with.</param>
    /// <returns>Negative, zero or positive as this ordinal is smaller, equal or larger.</returns>
    public int CompareTo(Ordinal other)
    {
        if (OmegaCoefficient != other.OmegaCoefficient)
        {
            return OmegaCoefficient.CompareTo(other.OmegaCoefficient);
        }

        return FinitePart.CompareTo(other.FinitePart);
    }
}
