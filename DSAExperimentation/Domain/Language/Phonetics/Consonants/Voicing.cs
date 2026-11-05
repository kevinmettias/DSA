namespace DSAExperimentation.Domain.Language.Phonetics.Consonants;

// Whether the vocal folds vibrate through a consonant: the IPA chart puts the voiceless one of a
// pair on the left of its cell and the voiced one on the right, p before b.
/// <summary>Whether a consonant is voiced.</summary>
public enum Voicing
{
    /// <summary>The vocal folds apart, as for p and s.</summary>
    Voiceless,

    /// <summary>The vocal folds vibrating, as for b and z.</summary>
    Voiced,
}
