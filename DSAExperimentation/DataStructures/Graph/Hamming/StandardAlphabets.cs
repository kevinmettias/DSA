namespace DSAExperimentation.DataStructures.Graph.Hamming;

// The Latin alphabet as an Alphabet, the one the word-ladder problems walk. Which
// alphabet is in play is a property of the problem, not of the graph, so it is
// passed in rather than baked into HammingGraph. The Latin letters are a shape many
// problems share, like EnglishLowercase; a world's own alphabet is Domain's, as
// DNA's four bases are Domain/Science/Dna's DnaBases.Alphabet (ARCHITECTURE §17.6).
/// <summary>The Latin <see cref="Alphabet"/> the word-ladder problems use.</summary>
public static class StandardAlphabets
{
    /// <summary>The 26 lowercase Latin letters, a through z.</summary>
    public static Alphabet LowercaseLatin => new("abcdefghijklmnopqrstuvwxyz");
}
