namespace DSAExperimentation.Domain.Language.Phonetics.Vowels;

// How close the tongue rises toward the roof of the mouth, the IPA vowel chart's rows from top to
// bottom: close (i), near-close (ɪ), close-mid (e), mid (ə), open-mid (ɛ), near-open (æ), open (a).
/// <summary>A vowel's height, the row of the IPA vowel chart it sits in.</summary>
public enum VowelHeight
{
    /// <summary>Close, the tongue nearest the roof of the mouth, as for i.</summary>
    Close,

    /// <summary>Near-close, as for ɪ.</summary>
    NearClose,

    /// <summary>Close-mid, as for e.</summary>
    CloseMid,

    /// <summary>Mid, as for ə.</summary>
    Mid,

    /// <summary>Open-mid, as for ɛ.</summary>
    OpenMid,

    /// <summary>Near-open, as for æ.</summary>
    NearOpen,

    /// <summary>Open, the tongue lowest, as for a.</summary>
    Open,
}
