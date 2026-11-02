using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Domain.Dna;

// DNA's four bases as LC 187 and LC 433 spell a strand: adenine, cytosine, guanine and thymine,
// written A, C, G and T. Which alphabet a mutation problem walks is a property of the problem, not
// of the graph, so the bases are Domain while the Alphabet record they fill stays tier 1 (ARCHITECTURE
// §17.6), as LockWheels' numbers are Domain and a graph is tier 1. Which number a solution gives each
// base is its own encoding, not DNA's: LC 187's 2-bit code is its strategy, and stays in its folder.
/// <summary>The four DNA bases, as letters and as an alphabet for one-base mutations.</summary>
public static class DnaBases
{
    /// <summary>The bases' letters, in alphabetical order.</summary>
    public const string Letters = "ACGT";

    /// <summary>The bases as an alphabet, for walking the strands one base away.</summary>
    public static Alphabet Alphabet => new(Letters);
}
