using DSAExperimentation.DataStructures.SubstringIndex;

namespace DSAExperimentation.DataStructures.SuffixArray;

// A suffix array with its text kept beside it, which is all a pattern query adds to the sorted
// order (ARCHITECTURE §8): the suffixes that start with a pattern are one contiguous run of the
// array, found by two binary searches that compare the pattern against the text at each probe. So a
// query costs O(pattern.Length * log(text.Length)), against the O(pattern.Length) a suffix tree or
// automaton walk costs, and pays for it with no per-node hash maps: the index is two int arrays and
// the text.
//
// The array is composed, not extended: SuffixArray keeps no text, so LC 899, 1044 and 1163, which
// only read the order, carry none, and their construction is the same code it was. The same reason
// SuffixTree gives for composing it licenses this (§5 step 5).
//
// The binary searches are written here rather than taken from MonotonePredicateSearch: its rule is
// a struct the search copies, and a struct cannot hold the ReadOnlySpan<char> the pattern arrives
// as, short of copying the pattern to the heap on every query.
/// <summary>
/// A suffix array kept with its text, answering whether, how often and where a pattern occurs.
/// </summary>
public sealed class SuffixArrayIndex : ICountingSubstringIndex, ILocatingSubstringIndex
{
    // A suffix's prefix compares at least this high against the pattern when it does not sort
    // before it, and at least the next value up when it sorts after it.
    private const int NotBefore = 0;
    private const int After = 1;

    // A suffix that runs out while it still matches the pattern is shorter, so it sorts first.
    private const int RunsOutFirst = -1;
    private const int StartsWithPattern = 0;

    private readonly string _text;

    /// <summary>Gets the sorted suffixes the index searches.</summary>
    public SuffixArray SuffixArray { get; }

    public SuffixArrayIndex(ReadOnlySpan<char> text)
        : this(text, new SuffixArray(text))
    {
    }

    // suffixArray must have been built over this exact text with the default comparer, whose order
    // is the ordinal one these searches compare in - an unenforced precondition, the same one
    // SuffixTree states for the array it is built from.
    public SuffixArrayIndex(ReadOnlySpan<char> text, SuffixArray suffixArray)
    {
        _text = text.ToString();
        SuffixArray = suffixArray;
    }

    /// <summary>
    /// Tests whether a pattern occurs in the text, with one binary search over the suffixes.
    /// </summary>
    /// <param name="pattern">The characters to look for, compared by ordinal value.</param>
    /// <returns>
    /// True when the pattern occurs in the text; always true for an empty pattern.
    /// </returns>
    public bool HasSubstring(ReadOnlySpan<char> pattern)
    {
        if (pattern.IsEmpty)
        {
            return true;
        }

        var firstNotBefore = FirstRankFrom(pattern, NotBefore);
        return HasPatternAtRank(firstNotBefore, pattern);
    }

    // Whether the suffix at a rank exists and starts with the pattern: the first suffix that does
    // not sort before a pattern starts with it exactly when any suffix does.
    private bool HasPatternAtRank(int rank, ReadOnlySpan<char> pattern)
        => rank < _text.Length
            && ComparePrefix(SuffixArray.Suffixes[rank], pattern) == StartsWithPattern;

    /// <summary>
    /// Counts a pattern's occurrences, overlapping ones included, with two binary searches.
    /// </summary>
    /// <param name="pattern">The characters to count, compared by ordinal value.</param>
    /// <returns>
    /// The count: 0 for an absent pattern, and the text's length plus one for an empty one.
    /// </returns>
    public int CountOccurrences(ReadOnlySpan<char> pattern)
    {
        if (pattern.IsEmpty)
        {
            return _text.Length + 1;
        }

        return FirstRankFrom(pattern, After) - FirstRankFrom(pattern, NotBefore);
    }

    /// <summary>
    /// Lists each position where a pattern starts, overlapping ones included, read off the run of
    /// sorted suffixes that start with it.
    /// </summary>
    /// <param name="pattern">The characters to look for, compared by ordinal value.</param>
    /// <returns>
    /// The start positions in the suffixes' sorted order: none for an absent pattern, and every
    /// position from 0 to the text's length for an empty one.
    /// </returns>
    public IEnumerable<int> FindOccurrences(ReadOnlySpan<char> pattern)
    {
        if (pattern.IsEmpty)
        {
            return Enumerable.Range(0, _text.Length + 1);
        }

        var first = FirstRankFrom(pattern, NotBefore);
        var count = FirstRankFrom(pattern, After) - first;
        return SuffixArray.Suffixes.AsSpan(first, count).ToArray();
    }

    // The first rank whose suffix compares at least this high against the pattern, or the text's
    // length when none does. The comparison only rises along the sorted suffixes, so the ranks that
    // reach a threshold are a tail of the array.
    private int FirstRankFrom(ReadOnlySpan<char> pattern, int threshold)
    {
        var low = 0;
        var high = _text.Length;

        while (low < high)
        {
            var middle = low + ((high - low) / AlgorithmConstants.HalvingFactor);

            if (ComparePrefix(SuffixArray.Suffixes[middle], pattern) < threshold)
            {
                low = middle + 1;
            }
            else
            {
                high = middle;
            }
        }

        return low;
    }

    // A suffix against the pattern over the pattern's length: negative when it sorts before every
    // string that starts with the pattern, zero when it starts with the pattern, positive when it
    // sorts after them. SequenceCompareTo compares chars by ordinal value, the default comparer's
    // order.
    private int ComparePrefix(int suffixStart, ReadOnlySpan<char> pattern)
    {
        var suffix = _text.AsSpan(suffixStart);
        var compared = Math.Min(suffix.Length, pattern.Length);
        var comparison = suffix[..compared].SequenceCompareTo(pattern[..compared]);

        if (comparison != 0)
        {
            return comparison;
        }

        return suffix.Length < pattern.Length ? RunsOutFirst : StartsWithPattern;
    }
}
