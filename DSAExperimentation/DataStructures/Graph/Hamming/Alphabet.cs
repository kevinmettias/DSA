namespace DSAExperimentation.DataStructures.Graph.Hamming;

// The character set a Hamming-graph problem mutates over. A distinct type rather
// than a bare string so it can never be transposed with the value being mutated -
// OneCharacterMutations(value, alphabet) takes two strings conceptually, and the
// compiler would not object to swapping them.
/// <summary>The set of characters a one-character mutation may write into a position.</summary>
/// <param name="Characters">The characters themselves, in the order mutations try them.</param>
public readonly record struct Alphabet(string Characters);
