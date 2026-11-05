namespace DSAExperimentation.Domain.Language.Phonetics.Consonants;

// Where in the mouth or throat a consonant is made, the place of articulation: the pulmonic chart's
// columns from the lips back to the glottis, and the places its other symbols name, made at two
// places at once (w at the lips and the soft palate) or between two columns (ɕ).
/// <summary>A consonant's place of articulation.</summary>
public enum Place
{
    /// <summary>Both lips, as for p and m.</summary>
    Bilabial,

    /// <summary>The lower lip against the upper teeth, as for f.</summary>
    Labiodental,

    /// <summary>The tongue against the teeth, as for θ.</summary>
    Dental,

    /// <summary>The tongue against the ridge behind the teeth, as for t and s.</summary>
    Alveolar,

    /// <summary>Just behind the ridge, as for ʃ.</summary>
    Postalveolar,

    /// <summary>The tongue's tip curled back, as for ʈ.</summary>
    Retroflex,

    /// <summary>The tongue's blade at the ridge and its body raised toward the hard palate, as for ɕ.</summary>
    AlveoloPalatal,

    /// <summary>The hard palate, as for j.</summary>
    Palatal,

    /// <summary>Rounded lips with the tongue at the hard palate, as for ɥ.</summary>
    LabialPalatal,

    /// <summary>The soft palate, as for k.</summary>
    Velar,

    /// <summary>Rounded lips with the tongue at the soft palate, as for w.</summary>
    LabialVelar,

    /// <summary>The uvula, as for q.</summary>
    Uvular,

    /// <summary>The pharynx, as for ħ.</summary>
    Pharyngeal,

    /// <summary>The epiglottis, as for ʜ.</summary>
    Epiglottal,

    /// <summary>The glottis, as for h.</summary>
    Glottal,
}
