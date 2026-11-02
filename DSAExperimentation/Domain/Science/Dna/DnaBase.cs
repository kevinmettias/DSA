namespace DSAExperimentation.Domain.Science.Dna;

// One of DNA's four bases: the nucleobase's name, and the letter a strand is spelled with. Each base
// is a named value, so DnaBases lists them by name rather than as a row of letters and strings.
/// <summary>A DNA base: its name and the letter that stands for it.</summary>
/// <param name="Name">The base's name, such as "adenine".</param>
/// <param name="Letter">The letter a strand writes it as, such as 'A'.</param>
public readonly record struct DnaBase(string Name, char Letter)
{
    /// <summary>Adenine, written A.</summary>
    public static DnaBase Adenine { get; } = new("adenine", 'A');

    /// <summary>Cytosine, written C.</summary>
    public static DnaBase Cytosine { get; } = new("cytosine", 'C');

    /// <summary>Guanine, written G.</summary>
    public static DnaBase Guanine { get; } = new("guanine", 'G');

    /// <summary>Thymine, written T.</summary>
    public static DnaBase Thymine { get; } = new("thymine", 'T');
}
