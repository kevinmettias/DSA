namespace DSAExperimentation.Domain.Language.Phonetics.Consonants;

// A consonant as the IPA writes it: its symbol and how it is made, in the order the IPA's own names
// read, so p is the voiceless bilabial plosive. An affricate is a stop released into a fricative,
// and the IPA writes it as the two symbols joined by a tie bar: t͡ʃ is t released into ʃ, the
// voiceless postalveolar affricate, made where the fricative is. Affricate builds one from its two
// consonants, so its symbol cannot disagree with theirs. The tie bar is a combining character,
// which shows on no character of its own in source, so it is written by its code point, U+0361.
/// <summary>A consonant: its IPA symbol, voicing, place and manner of articulation.</summary>
/// <param name="Symbol">The IPA symbol, such as "p".</param>
/// <param name="Voicing">Whether the vocal folds vibrate.</param>
/// <param name="Place">Where it is made.</param>
/// <param name="Manner">How it shapes the air.</param>
public readonly record struct Consonant(string Symbol, Voicing Voicing, Place Place, Manner Manner)
{
    private const char TieBar = (char)0x0361;
    private const string NotAStopMessage = "An affricate begins with a plosive.";
    private const string NotAFricativeMessage = "An affricate is released into a fricative.";
    private const string UnlikeVoicingMessage =
        "An affricate's stop and fricative are voiced alike.";

    /// <summary>Builds the affricate a stop released into a fricative makes.</summary>
    /// <param name="stop">The plosive it begins with, such as t.</param>
    /// <param name="fricative">The fricative it is released into, such as ʃ.</param>
    /// <returns>The affricate, its symbol the two joined by the tie bar, made at the fricative's place.</returns>
    /// <exception cref="ArgumentException"><paramref name="stop"/> is no plosive, <paramref name="fricative"/> no fricative, or the two are voiced differently.</exception>
    public static Consonant Affricate(Consonant stop, Consonant fricative)
    {
        if (stop.Manner != Manner.Plosive)
        {
            throw new ArgumentException(NotAStopMessage, nameof(stop));
        }

        if (fricative.Manner != Manner.Fricative)
        {
            throw new ArgumentException(NotAFricativeMessage, nameof(fricative));
        }

        if (stop.Voicing != fricative.Voicing)
        {
            throw new ArgumentException(UnlikeVoicingMessage, nameof(fricative));
        }

        var symbol = $"{stop.Symbol}{TieBar}{fricative.Symbol}";

        return new(symbol, fricative.Voicing, fricative.Place, Manner.Affricate);
    }
}
