namespace DSAExperimentation.Domain.Language.Phonetics.Consonants;

// How a consonant shapes the air, the manner of articulation: the pulmonic chart's rows, the
// lateral flap among its other symbols, and the affricate, a stop released into a fricative at one
// place, which the chart writes as its two symbols joined by a tie bar (t͡ʃ) rather than giving it
// a row.
/// <summary>A consonant's manner of articulation.</summary>
public enum Manner
{
    /// <summary>The air stopped and released, as for p.</summary>
    Plosive,

    /// <summary>The air stopped in the mouth and let out through the nose, as for m.</summary>
    Nasal,

    /// <summary>A part of the mouth set vibrating, as for r.</summary>
    Trill,

    /// <summary>One quick tap, as for ɾ.</summary>
    TapOrFlap,

    /// <summary>The air forced through a narrow gap, as for s.</summary>
    Fricative,

    /// <summary>Friction past the sides of the tongue, as for ɬ.</summary>
    LateralFricative,

    /// <summary>A gap too wide for friction, as for j.</summary>
    Approximant,

    /// <summary>The air past the sides of the tongue, as for l.</summary>
    LateralApproximant,

    /// <summary>A lateral tap, as for ɺ.</summary>
    LateralFlap,

    /// <summary>A stop released into a fricative at one place, as for t͡ʃ.</summary>
    Affricate,
}
