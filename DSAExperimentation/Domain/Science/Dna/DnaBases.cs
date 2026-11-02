using System.Collections.Immutable;
using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Domain.Science.Dna;

// DNA's four bases as LC 187 and LC 433 spell a strand: adenine, cytosine, guanine and thymine,
// written A, C, G and T. All is the one source: the letters, the names, the spelled-out strand
// alphabet and the slots are all read from it once, so none can disagree with another. As an
// alphabet the bases are A to T with gaps, not one run of character codes, so their slots come from
// a LetterTable. Which number a solution gives each base is its own encoding, not DNA's: LC 187's
// 2-bit code is its strategy, and stays in its folder.
/// <summary>The four DNA bases, as named values and as an alphabet a strand is spelled in.</summary>
public readonly struct DnaBases : IAlphabet
{
    // The bases in order, the one source everything below reads, declared first because static
    // initializers run in the order they are written.
    private static readonly ImmutableArray<DnaBase> Bases = [DnaBase.Adenine, DnaBase.Cytosine, DnaBase.Guanine, DnaBase.Thymine];
    private static readonly LetterTable Table = new([.. Bases.Select(dnaBase => dnaBase.Letter)]);

    /// <summary>The four bases, in alphabetical order of their letters.</summary>
    public static ImmutableArray<DnaBase> All => Bases;

    /// <summary>The bases' letters, in the order of <see cref="All"/>.</summary>
    public static ImmutableArray<char> Letters => Table.Letters;

    /// <summary>The bases' names, in the order of <see cref="All"/>.</summary>
    public static ImmutableArray<string> Names { get; } = [.. Bases.Select(dnaBase => dnaBase.Name)];

    /// <summary>The number of bases, four.</summary>
    public static int Size => Table.Size;

    /// <summary>Spells the bases' letters out as one string.</summary>
    /// <returns>The letters in order, "ACGT".</returns>
    public static string LettersAsString() => new(Letters.AsSpan());

    /// <summary>Finds the slot a base's letter takes.</summary>
    /// <param name="letter">A base's letter: A, C, G or T.</param>
    /// <returns>The base's slot, 0 for A through 3 for T.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not a base's letter.</exception>
    public static int IndexOf(char letter) => Table.IndexOf(letter);

    /// <summary>Finds the slot a character takes, if it is a base's letter.</summary>
    /// <param name="letter">The character to look up.</param>
    /// <param name="index">The base's slot; meaningful only when this returns true.</param>
    /// <returns>True for A, C, G or T; otherwise false.</returns>
    public static bool TryIndexOf(char letter, out int index) => Table.TryIndexOf(letter, out index);

    /// <summary>Finds the letter a slot holds.</summary>
    /// <param name="index">A slot, 0 through 3.</param>
    /// <returns>The base's letter, A for 0 through T for 3.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside [0, 4).</exception>
    public static char LetterAt(int index) => Table.LetterAt(index);
}
