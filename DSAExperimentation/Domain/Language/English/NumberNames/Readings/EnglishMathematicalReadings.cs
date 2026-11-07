using System.Numerics;
using System.Text;
using DSAExperimentation.Domain.Language.English.NumberNames.Composition;
using DSAExperimentation.Domain.Language.English.NumberNames.Scales;
using DSAExperimentation.Domain.Mathematics.ExtendedReals;
using DSAExperimentation.Domain.Mathematics.SetTheory;

namespace DSAExperimentation.Domain.Language.English.NumberNames.Readings;

// How English reads aloud the numbers Mathematics/NumberSystems, ExtendedReals and SetTheory
// define, built on the composer for the numbers inside them and on EnglishMathematicalWords'
// readings. A complex number is read real part first, "three plus four i", and a quaternion through
// its units i, j and k; the octonions' seven units have no spoken names English agrees on, so they
// are not read. The affine ends are "infinity" and "negative infinity", the projective point
// "infinity". ω·a + b is "omega times a plus b", ℵ0 "aleph-null" (also said aleph-nought or
// aleph-zero), and a higher beth "beth" and its index. The words are lowercase, as written within a
// sentence.
/// <summary>
/// English readings of complex numbers, quaternions, the extended reals, ordinals and cardinals.
/// </summary>
/// <typeparam name="TScale">The scale the numbers inside the readings are composed in.</typeparam>
public static class EnglishMathematicalReadings<TScale>
    where TScale : struct, INumberScale
{
    private const string Beth = "beth";
    private const char Space = EnglishOperatorWords.WordSeparator;
    private const string UnreadableMessage = "Only a real, complex or quaternion number is read.";

    // The quaternion units after the real part; a complex number uses the first.
    private static readonly string[] Units =
        [string.Empty, EnglishMathematicalWords.ImaginaryUnit, "j", "k"];

    /// <summary>Reads a complex number or a quaternion by its coordinates.</summary>
    /// <param name="coordinates">
    /// One, two or four integer coordinates: real part, then i, j, k.
    /// </param>
    /// <returns>Such as "three minus four i"; a unit's coefficient of one is not read.</returns>
    /// <exception cref="ArgumentException">There are not one, two or four coordinates.</exception>
    /// <exception cref="OverflowException">
    /// A coordinate is long.MinValue, which has no size.
    /// </exception>
    public static string Hypercomplex(ReadOnlySpan<long> coordinates)
    {
        // One, two or four coordinates: the powers of two up to the quaternions' four.
        var length = coordinates.Length;
        var isReadable = BitOperations.IsPow2(length) && length <= Units.Length;

        if (!isReadable)
        {
            throw new ArgumentException(UnreadableMessage, nameof(coordinates));
        }

        var text = new StringBuilder();

        for (var unit = 0; unit < coordinates.Length; unit++)
        {
            AppendTerm(text, coordinates[unit], Units[unit]);
        }

        if (text.Length == 0)
        {
            return EnglishNumberNames<TScale>.Cardinal(0);
        }

        return text.ToString();
    }

    /// <summary>Reads an ordinal below ω², such as "omega times two plus three".</summary>
    /// <param name="ordinal">The ordinal.</param>
    /// <returns>Its reading; a finite ordinal reads as its number.</returns>
    public static string Transfinite(Ordinal ordinal)
    {
        if (ordinal.IsFinite)
        {
            return EnglishNumberNames<TScale>.Cardinal(ordinal.FinitePart);
        }

        var text = new StringBuilder(EnglishMathematicalWords.Omega);

        if (ordinal.OmegaCoefficient > 1)
        {
            text.Append(Space).Append(EnglishOperatorWords.Times).Append(Space);
            text.Append(EnglishNumberNames<TScale>.Cardinal(ordinal.OmegaCoefficient));
        }

        if (ordinal.FinitePart > 0)
        {
            text.Append(Space).Append(EnglishOperatorWords.Plus).Append(Space);
            text.Append(EnglishNumberNames<TScale>.Cardinal(ordinal.FinitePart));
        }

        return text.ToString();
    }

    /// <summary>Reads a cardinal: its count, aleph-null, or a beth by its index.</summary>
    /// <param name="cardinal">The cardinal.</param>
    /// <returns>Such as "seven", "aleph-null" or "beth one".</returns>
    public static string Transfinite(Cardinal cardinal)
    {
        if (cardinal.IsFinite)
        {
            return EnglishNumberNames<TScale>.Cardinal(cardinal.Count);
        }

        if (cardinal == Cardinal.AlephNull)
        {
            return EnglishMathematicalWords.AlephNull;
        }

        var index = EnglishNumberNames<TScale>.Cardinal(cardinal.BethIndex);
        return string.Join(Space, Beth, index);
    }

    /// <summary>Reads a number of the affinely extended reals.</summary>
    /// <param name="value">The number.</param>
    /// <returns>"infinity", "negative infinity", or the number's words.</returns>
    public static string AffinelyExtended(AffinelyExtendedReal<long> value)
    {
        if (value.TryGetFinite(out var finite))
        {
            return EnglishNumberNames<TScale>.Cardinal(finite);
        }

        var isPositive = value == AffinelyExtendedReal<long>.PositiveInfinity;
        return isPositive
            ? EnglishMathematicalWords.Infinity
            : EnglishMathematicalWords.NegativeInfinity;
    }

    /// <summary>Reads a number of the projectively extended reals.</summary>
    /// <param name="value">The number.</param>
    /// <returns>"infinity", or the number's words.</returns>
    public static string ProjectivelyExtended(ProjectivelyExtendedReal<long> value)
    {
        if (value.TryGetFinite(out var finite))
        {
            return EnglishNumberNames<TScale>.Cardinal(finite);
        }

        return EnglishMathematicalWords.Infinity;
    }

    // One term: nothing for a zero coefficient, its sign word, then its size and unit.
    private static void AppendTerm(StringBuilder text, long coefficient, string unit)
    {
        if (coefficient == 0)
        {
            return;
        }

        var isNegative = coefficient < 0;

        if (text.Length > 0)
        {
            var sign = isNegative ? EnglishOperatorWords.Minus : EnglishOperatorWords.Plus;
            text.Append(Space).Append(sign).Append(Space);
        }
        else if (isNegative)
        {
            text.Append(EnglishOperatorWords.Minus).Append(Space);
        }

        var size = Math.Abs(coefficient);
        var sizeAndUnit = SizeAndUnit(size, unit);
        text.Append(sizeAndUnit);
    }

    // A real part is its size; a unit's coefficient of one is left unread ("i", "minus j").
    private static string SizeAndUnit(long size, string unit)
    {
        if (unit.Length == 0)
        {
            return EnglishNumberNames<TScale>.Cardinal(size);
        }

        if (size == 1)
        {
            return unit;
        }

        var spelled = EnglishNumberNames<TScale>.Cardinal(size);
        return string.Join(Space, spelled, unit);
    }
}
