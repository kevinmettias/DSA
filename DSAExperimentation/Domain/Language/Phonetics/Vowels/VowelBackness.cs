namespace DSAExperimentation.Domain.Language.Phonetics.Vowels;

// How far forward the tongue's highest point sits, the IPA vowel chart's columns from left to
// right: front (i), near-front (ɪ), central (ə), near-back (ʊ) and back (u).
/// <summary>A vowel's backness, the column of the IPA vowel chart it sits in.</summary>
public enum VowelBackness
{
    /// <summary>Front, as for i.</summary>
    Front,

    /// <summary>Near-front, as for ɪ.</summary>
    NearFront,

    /// <summary>Central, as for ə.</summary>
    Central,

    /// <summary>Near-back, as for ʊ.</summary>
    NearBack,

    /// <summary>Back, as for u.</summary>
    Back,
}
