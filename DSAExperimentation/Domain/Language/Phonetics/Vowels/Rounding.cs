namespace DSAExperimentation.Domain.Language.Phonetics.Vowels;

// Whether the lips round for a vowel. The chart pairs most cells, an unrounded vowel left of its
// rounded one (i and y), but two of its symbols say nothing of the lips: ə, the mid central vowel,
// and ɐ, the near-open central one, so a third value states that rather than guessing.
/// <summary>Whether a vowel's symbol says the lips are rounded.</summary>
public enum Rounding
{
    /// <summary>Lips spread or neutral, as for i.</summary>
    Unrounded,

    /// <summary>Lips rounded, as for y and u.</summary>
    Rounded,

    /// <summary>The symbol does not say, as for ə and ɐ.</summary>
    Unspecified,
}
