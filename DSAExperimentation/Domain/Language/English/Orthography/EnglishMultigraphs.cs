using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.English.Orthography;

// English's consonant digraphs and trigraph, each a named value: ch, sh, th, ph, ng and ck, and
// tch, letters that spell one consonant sound together, as sh does in "ship". Each is spelled with
// consonant letters. They are spelling, as the vowels are: which sound each spells is English's
// phonology, and th spells two, as in "thin" and "this". Has reads MultigraphSet's table, built
// once from the named values.
/// <summary>English's consonant digraphs, ch, sh, th, ph, ng and ck, and its trigraph tch.</summary>
public readonly struct EnglishMultigraphs : IMultigraphSet
{
    /// <summary>ch, as in "church".</summary>
    public static Multigraph Ch { get; } = new("ch");

    /// <summary>sh, as in "ship".</summary>
    public static Multigraph Sh { get; } = new("sh");

    /// <summary>th, as in "thin" and "this".</summary>
    public static Multigraph Th { get; } = new("th");

    /// <summary>ph, as in "phone".</summary>
    public static Multigraph Ph { get; } = new("ph");

    /// <summary>ng, as in "sing".</summary>
    public static Multigraph Ng { get; } = new("ng");

    /// <summary>ck, as in "back".</summary>
    public static Multigraph Ck { get; } = new("ck");

    /// <summary>tch, as in "watch".</summary>
    public static Multigraph Tch { get; } = new("tch");

    /// <summary>The digraphs, ch, sh, th, ph, ng and ck.</summary>
    public static ImmutableArray<Multigraph> Digraphs { get; } = [Ch, Sh, Th, Ph, Ng, Ck];

    /// <summary>The trigraph, tch.</summary>
    public static ImmutableArray<Multigraph> Trigraphs { get; } = [Tch];

    /// <inheritdoc/>
    public static ImmutableArray<Multigraph> Multigraphs { get; } = [.. Digraphs, .. Trigraphs];

    /// <summary>Tests whether some letters are one of English's digraphs or its trigraph, in either case.</summary>
    /// <param name="letters">The letters to test, such as "sh".</param>
    /// <returns>True for ch, sh, th, ph, ng, ck or tch, in any case; otherwise false.</returns>
    public static bool Has(ReadOnlySpan<char> letters) =>
        MultigraphSet<EnglishMultigraphs>.Has(letters);
}
