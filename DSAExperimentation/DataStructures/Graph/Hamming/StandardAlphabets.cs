namespace DSAExperimentation.DataStructures.Graph.Hamming;

// The named Alphabet values LeetCode's mutation problems actually use. Which one
// is in play is a property of the problem, not of the graph, so it is passed in
// rather than baked into HammingGraph.
internal static class StandardAlphabets
{
    public static Alphabet LowercaseLatin => new("abcdefghijklmnopqrstuvwxyz");

    public static Alphabet Dna => new("ACGT");
}
