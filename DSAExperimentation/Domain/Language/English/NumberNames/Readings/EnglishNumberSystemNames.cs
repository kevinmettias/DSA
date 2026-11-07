using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.NumberSystems;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Readings;

// English's names for the number systems, each a named value pairing a system of
// Mathematics/NumberSystems with its name, lowercase and plural as written within a sentence.
/// <summary>English's names for the number systems, the naturals to the sedenions.</summary>
public static class EnglishNumberSystemNames
{
    /// <summary>Gets "natural numbers".</summary>
    public static NumberSystemName Naturals { get; } =
        new(NumberSystem.Naturals, "natural numbers");

    /// <summary>Gets "integers".</summary>
    public static NumberSystemName Integers { get; } = new(NumberSystem.Integers, "integers");

    /// <summary>Gets "rational numbers".</summary>
    public static NumberSystemName Rationals { get; } =
        new(NumberSystem.Rationals, "rational numbers");

    /// <summary>Gets "real numbers".</summary>
    public static NumberSystemName Reals { get; } = new(NumberSystem.Reals, "real numbers");

    /// <summary>Gets "complex numbers".</summary>
    public static NumberSystemName Complex { get; } = new(NumberSystem.Complex, "complex numbers");

    /// <summary>Gets "quaternions".</summary>
    public static NumberSystemName Quaternions { get; } =
        new(NumberSystem.Quaternions, "quaternions");

    /// <summary>Gets "octonions".</summary>
    public static NumberSystemName Octonions { get; } = new(NumberSystem.Octonions, "octonions");

    /// <summary>Gets "sedenions".</summary>
    public static NumberSystemName Sedenions { get; } = new(NumberSystem.Sedenions, "sedenions");

    /// <summary>Gets every name, the naturals first and the sedenions last.</summary>
    public static ImmutableArray<NumberSystemName> All { get; } =
        [Naturals, Integers, Rationals, Reals, Complex, Quaternions, Octonions, Sedenions];
}
