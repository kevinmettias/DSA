namespace DSAExperimentation.DataStructures.SubstringIndex;

// A locating query, in the text-index sense: every position where a pattern starts. The suffix
// array answers it with the run of its sorted suffixes that start with the pattern, and the suffix
// tree with the leaves below the node the pattern reaches, each in time linear in the pattern plus
// the positions reported (with a log factor for the array's search). The suffix automaton does not
// implement it: a state records how many positions it ends at, not which, and listing them would
// need each state's first end position and its suffix-link tree turned around, structure it does
// not keep.
/// <summary>A substring index that lists every position where a pattern starts.</summary>
public interface ILocatingSubstringIndex : ISubstringIndex
{
    /// <summary>
    /// Lists each position where a pattern starts in the text, overlapping ones included.
    /// </summary>
    /// <param name="pattern">The characters to look for, compared by ordinal value.</param>
    /// <returns>
    /// The start positions in no particular order: none for an absent pattern, and every position
    /// from 0 to the text's length for an empty one.
    /// </returns>
    IEnumerable<int> FindOccurrences(ReadOnlySpan<char> pattern);
}
