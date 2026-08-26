namespace DSAExperimentation.Algorithms.StringMatching;

// Knuth-Morris-Pratt (KMP) string matching. Named for its core mechanism - the prefix/failure
// function - rather than eponymously, matching this repo's existing preference for descriptive
// names over a person's name for a sole technique (BinarySearch, MergeSort); KMP has no equally
// crisp descriptive alternative, so the technique's own name stays in this comment and in
// ComputeFailureFunction's name instead.
//
// Unlike BinarySearch/MergeSort, there is no repo-authored Representation contract to be generic
// over here. Representation is supplied entirely by the BCL: ReadOnlySpan<char> already gives O(1)
// indexed access uniformly over a string, an array, or a substring slice, and both methods below
// depend on that cost the same way BinarySearch depends on IRandomAccessSequence<Element>.Get being
// O(1) - except structurally guaranteed here rather than an unenforced obligation, since
// ReadOnlySpan<char> is a sealed ref struct implementing no interfaces: it could not satisfy this
// repo's `where TSequence : struct, IRandomAccessSequence<Element>`-shaped constraints even if a
// second physical layout of text existed in this codebase to justify trying, which none does.
//
// Character equality is a plain IEqualityComparer<char> parameter, not a witness, for the same
// reason BinarySearch/MergeSort keep their comparer a runtime object rather than a closed set this
// library enumerates itself: case-insensitive and culture-aware matching are both ordinary,
// open-ended variants a caller might want, not a small fixed set of named choices.
internal static class PrefixFunctionSearch
{
    // The prefix function (also called the failure function, or the "longest proper prefix that is
    // also a suffix" array): result[i] is the length of the longest proper prefix of pattern[0..i]
    // that is also a suffix of pattern[0..i]. O(pattern.Length). Independently useful beyond FindAll
    // below - e.g. the longest prefix of the whole pattern that is also a suffix is result[^1]
    // characters - so this is exposed publicly rather than kept as a FindAll-only implementation
    // detail.
    public static int[] ComputeFailureFunction(ReadOnlySpan<char> pattern)
        => ComputeFailureFunction(pattern, EqualityComparer<char>.Default);

    public static int[] ComputeFailureFunction(ReadOnlySpan<char> pattern, IEqualityComparer<char> comparer)
    {
        var failure = new int[pattern.Length];
        var matcher = new FailureFunctionMatcher(failure, comparer);
        var matched = 0;

        for (var i = 1; i < pattern.Length; i++)
        {
            matched = Advance(pattern[i], pattern, matched, matcher);
            failure[i] = matched;
        }

        return failure;
    }

    // Every starting index in text where pattern occurs, including overlapping occurrences, in the
    // order they're found. O(text.Length + pattern.Length): each character of text is examined a
    // number of times bounded by the failure-function fallback, never re-scanned from the start of
    // pattern on a mismatch the way a naive search would.
    //
    // An empty pattern matches at every insertion point, 0..text.Length inclusive - the same
    // convention string.IndexOf("") uses - handled as an explicit guard rather than falling through
    // naturally: the unguarded search loop below dereferences pattern[0] before it would ever notice
    // pattern is empty, which is a real out-of-range read, not just a different answer.
    public static List<int> FindAll(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern)
        => FindAll(text, pattern, EqualityComparer<char>.Default);

    public static List<int> FindAll(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern, IEqualityComparer<char> comparer)
    {
        if (pattern.IsEmpty)
        {
            return MatchEveryInsertionPoint(text);
        }

        var failure = ComputeFailureFunction(pattern, comparer);
        var matcher = new FailureFunctionMatcher(failure, comparer);
        return SearchWithMatcher(text, pattern, matcher);
    }

    private static List<int> MatchEveryInsertionPoint(ReadOnlySpan<char> text)
    {
        var matches = new List<int>();

        for (var position = 0; position <= text.Length; position++)
        {
            matches.Add(position);
        }

        return matches;
    }

    // The KMP search walk proper: advances matched forward via Advance below (which falls back
    // through the failure function on mismatch instead of restarting from the beginning of
    // pattern), and records a match - also via the failure function, so overlapping occurrences
    // aren't missed - whenever matched reaches the full pattern length.
    private static List<int> SearchWithMatcher(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern, FailureFunctionMatcher matcher)
    {
        var matches = new List<int>();
        var matched = 0;

        for (var i = 0; i < text.Length; i++)
        {
            matched = Advance(text[i], pattern, matched, matcher);

            if (matched == pattern.Length)
            {
                matches.Add(i - pattern.Length + 1);
                matched = matcher.Failure[matched - 1];
            }
        }

        return matches;
    }

    // The one piece of KMP's fallback logic ComputeFailureFunction and SearchWithMatcher both
    // need: given the next character to match and how much of pattern already matches, either
    // extend that match by one or fall back through the failure function until it can - never
    // restarting the comparison from pattern's own beginning.
    private static int Advance(char current, ReadOnlySpan<char> pattern, int matched, FailureFunctionMatcher matcher)
    {
        while (matched > 0 && !matcher.Comparer.Equals(current, pattern[matched]))
        {
            matched = matcher.Failure[matched - 1];
        }

        return matcher.Comparer.Equals(current, pattern[matched]) ? ExtendMatch(matched) : matched;
    }

    private static int ExtendMatch(int matched) => matched + 1;

    // Bundles the two pieces Advance needs beyond the current character/pattern/matched-so-far -
    // grouping them keeps Advance/SearchWithMatcher/ComputeFailureFunction at or under this
    // repo's parameter-count limit. Private and un-fileworthy on its own, the same status
    // BinarySearch's BisectionStep has: it never crosses either public method's boundary, unlike
    // SearchRange/SortBounds.
    private readonly record struct FailureFunctionMatcher(int[] Failure, IEqualityComparer<char> Comparer);
}
