namespace DSAExperimentation.DataStructures.SubstringIndex;

// The longest substring that occurs at least twice, its occurrences free to overlap ("aaa" in
// "aaaa"), as LC 1044 asks. It takes no pattern, so it does not extend ISubstringIndex: the bare
// SuffixArray answers it from its longest-common-prefix array without keeping its text. Each
// structure reads it off what it already holds - the largest entry of that array, the deepest
// suffix-tree node that two suffixes pass through, the longest automaton state that ends at two
// positions - in time linear in the structure's size.
/// <summary>
/// A text index that finds the longest substring occurring more than once in its text.
/// </summary>
public interface ILongestRepeatIndex
{
    /// <summary>
    /// Finds the longest substring that occurs at least twice in the text, overlaps allowed.
    /// </summary>
    /// <returns>
    /// One of its occurrences, any one when several longest repeats tie;
    /// <see cref="SubstringOccurrence.Empty"/> when no character repeats.
    /// </returns>
    SubstringOccurrence FindLongestRepeat();
}
