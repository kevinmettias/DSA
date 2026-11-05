using System.Collections.Frozen;

namespace DSAExperimentation.Domain.Language;

// A multigraph set read as a table, built once per set, as LetterSet reads a letter set as a mask:
// the set's spellings are gathered into a frozen set that compares letters with case set aside, so
// "SH" is found as "sh" is, and a span of a longer text is looked up without being copied out.
/// <summary>Membership in a language's set of multigraphs.</summary>
/// <typeparam name="TSet">The set, such as a language's digraphs and trigraphs.</typeparam>
public static class MultigraphSet<TSet>
    where TSet : struct, IMultigraphSet
{
    private static readonly FrozenSet<string> Spellings =
        TSet.Multigraphs
            .Select(multigraph => multigraph.Letters)
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    private static readonly FrozenSet<string>.AlternateLookup<ReadOnlySpan<char>> SpellingLookup =
        Spellings.GetAlternateLookup<ReadOnlySpan<char>>();

    /// <summary>Tests whether some letters are one of the set's multigraphs, in either case.</summary>
    /// <param name="letters">The letters to test, such as "ch".</param>
    /// <returns>True when <paramref name="letters"/> spell one of the set's multigraphs, ignoring case; otherwise false.</returns>
    public static bool Has(ReadOnlySpan<char> letters) => SpellingLookup.Contains(letters);
}
