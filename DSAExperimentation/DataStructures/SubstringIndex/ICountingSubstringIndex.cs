namespace DSAExperimentation.DataStructures.SubstringIndex;

// A counting query, in the text-index sense: how many times a pattern occurs, without listing
// where. Each implementation counts in time bounded by the pattern rather than by the occurrences,
// from a count it settled at construction or from a range it finds by search: the suffix tree keeps
// how many suffixes run through each node, the automaton how many positions each state ends at, and
// the suffix array finds the run of suffixes that start with the pattern. HasSubstring holds
// exactly when the count is positive.
/// <summary>A substring index that counts a pattern's occurrences without listing them.</summary>
public interface ICountingSubstringIndex : ISubstringIndex
{
    /// <summary>Counts a pattern's occurrences in the text, overlapping ones included.</summary>
    /// <param name="pattern">The characters to count, compared by ordinal value.</param>
    /// <returns>
    /// The count: 0 for an absent pattern, and the text's length plus one for an empty one.
    /// </returns>
    int CountOccurrences(ReadOnlySpan<char> pattern);
}
