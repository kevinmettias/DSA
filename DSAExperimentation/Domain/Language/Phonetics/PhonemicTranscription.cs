using System.Collections.Frozen;
using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.Phonetics;

// A phonemic transcription: a word's phonemes in a language's inventory, written in the IPA's
// symbols, as a dictionary writes "ship" between slashes, /ʃɪp/. Reading one back splits it into
// the inventory's phonemes, and a run of symbols that is one phoneme reads as that phoneme: /maɪ/
// is m and the diphthong aɪ, not m, a and ɪ, and iː is one long vowel. So at each place the longest
// symbol the inventory has is taken, as a reader of the transcription takes it. The symbols are
// gathered from the inventory once, and one the language has no phoneme for is an error rather than
// a guess. Stress marks and syllable breaks belong to no phoneme, so a transcription read here
// holds none, and the slashes are the caller's to strip.
/// <summary>Reads a phonemic transcription as the phonemes of a language's inventory.</summary>
/// <typeparam name="TInventory">The language's phonemes, such as an accent of English.</typeparam>
public static class PhonemicTranscription<TInventory>
    where TInventory : struct, IPhonemeInventory
{
    private const string NotAPhonemeMessage =
        "The transcription holds a symbol that is no phoneme of the inventory.";

    private static readonly FrozenSet<string> Symbols = SymbolsOfInventory();
    private static readonly FrozenSet<string>.AlternateLookup<ReadOnlySpan<char>> SymbolLookup =
        Symbols.GetAlternateLookup<ReadOnlySpan<char>>();

    private static readonly int LongestSymbol = Symbols.Max(symbol => symbol.Length);

    /// <summary>Splits a transcription into its phonemes, taking the longest phoneme at each place.</summary>
    /// <param name="transcription">The phonemes' symbols, without the slashes, such as "ʃɪp".</param>
    /// <returns>Each phoneme's symbol, in order, such as "ʃ", "ɪ" and "p".</returns>
    /// <exception cref="ArgumentException">Some part of <paramref name="transcription"/> is no phoneme of the inventory.</exception>
    public static ImmutableArray<string> Segment(string transcription)
    {
        var phonemes = ImmutableArray.CreateBuilder<string>();

        for (var start = 0; start < transcription.Length;)
        {
            var length = LongestPhonemeAt(transcription.AsSpan(start));

            if (length == 0)
            {
                throw new ArgumentException(NotAPhonemeMessage, nameof(transcription));
            }

            var phoneme = transcription.Substring(start, length);

            phonemes.Add(phoneme);
            start += length;
        }

        return phonemes.ToImmutable();
    }

    // The length of the longest phoneme the rest of a transcription begins with, or 0 for none.
    private static int LongestPhonemeAt(ReadOnlySpan<char> rest)
    {
        for (var length = Math.Min(LongestSymbol, rest.Length); length > 0; length--)
        {
            if (SymbolLookup.Contains(rest[..length]))
            {
                return length;
            }
        }

        return 0;
    }

    private static FrozenSet<string> SymbolsOfInventory() =>
        TInventory.Monophthongs.Select(vowel => vowel.Symbol)
            .Concat(TInventory.Diphthongs.Select(diphthong => diphthong.Symbol))
            .Concat(TInventory.Consonants.Select(consonant => consonant.Symbol))
            .ToFrozenSet(StringComparer.Ordinal);
}
