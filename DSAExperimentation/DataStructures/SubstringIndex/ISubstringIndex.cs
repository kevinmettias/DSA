namespace DSAExperimentation.DataStructures.SubstringIndex;

// The weakest of the substring-index capabilities (ARCHITECTURE §8): one fixed text, indexed once,
// asked whether a pattern occurs in it. SuffixArrayIndex, SuffixTree and SuffixAutomaton each
// answer it, at a cost of their own, and the capabilities beside it are split so that each
// structure implements exactly what it answers well: counting (ICountingSubstringIndex), locating
// (ILocatingSubstringIndex, which the automaton does not answer) and the longest repeat
// (ILongestRepeatIndex). The longest common prefix of two suffixes has no contract, since only the
// suffix array answers it naturally (LongestCommonPrefixQueries), and an interface with one
// implementation is the speculative cost §5 step 3 declines.
//
// The laws every implementation keeps: characters compare exactly, by their ordinal value, as the
// tree's and the automaton's hash maps and the default comparer's suffix order do; and the empty
// pattern occurs, at every one of the text's Length + 1 insertion points. Callers reach a structure
// through its concrete type, or a generic constrained to it, never through this interface on a hot
// path: the structures are classes, so code generic over this contract is shared, and its calls go
// through a runtime lookup.
/// <summary>
/// The substrings of one fixed text, indexed so that a pattern's presence is found without scanning
/// the text.
/// </summary>
public interface ISubstringIndex
{
    /// <summary>Tests whether a pattern occurs in the text.</summary>
    /// <param name="pattern">The characters to look for, compared by ordinal value.</param>
    /// <returns>
    /// True when the pattern occurs in the text; always true for an empty pattern.
    /// </returns>
    bool HasSubstring(ReadOnlySpan<char> pattern);
}
