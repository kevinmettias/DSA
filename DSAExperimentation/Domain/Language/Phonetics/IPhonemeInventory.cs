using System.Collections.Immutable;
using DSAExperimentation.Domain.Language.Phonetics.Consonants;
using DSAExperimentation.Domain.Language.Phonetics.Vowels;

namespace DSAExperimentation.Domain.Language.Phonetics;

// The sounds one language, in one accent, tells words apart by: its phonemes, each written in the
// IPA. The IPA's charts are every sound; an inventory picks the ones a language uses, how long it
// holds each vowel, and which vowels glide into which, so English's phonology is a configuration of
// the IPA rather than a second list of sounds. An accent is part of the configuration, since
// accents of one language differ in their vowels.
/// <summary>A language's phonemes, as a selection from the IPA's sounds.</summary>
public interface IPhonemeInventory
{
    /// <summary>The single vowels, each with its length.</summary>
    static abstract ImmutableArray<Monophthong> Monophthongs { get; }

    /// <summary>The diphthongs.</summary>
    static abstract ImmutableArray<Diphthong> Diphthongs { get; }

    /// <summary>The consonants.</summary>
    static abstract ImmutableArray<Consonant> Consonants { get; }
}
