namespace DSAExperimentation.DataStructures.Graph.Hamming;

// The named Alphabet values LeetCode's mutation problems actually use. Which one
// is in play is a property of the problem, not of the graph, so it is passed in
// rather than baked into HammingGraph.
/// <summary>The <see cref="Alphabet"/> values LeetCode's mutation problems use.</summary>
public static class StandardAlphabets
{
    /// <summary>The 26 lowercase Latin letters, a through z.</summary>
    public static Alphabet LowercaseLatin => new("abcdefghijklmnopqrstuvwxyz");

    /// <summary>The four DNA bases A, C, G and T.</summary>
    public static Alphabet Dna => new("ACGT");
}
