namespace DSAExperimentation.Algorithms.StringMatching;

// The Z-function (also called the Z-array): result[i] is the length of the longest common prefix
// of text and text[i:]. Kept eponymous-adjacent as "ZFunction" rather than translated into a fully
// descriptive name, for the same reason Manacher stays eponymous - unlike "prefix function" (which
// already has a standard non-symbolic name in the literature), "Z-function"/"Z-array" has no
// equally crisp descriptive alternative that wouldn't just restate "self-overlap array," which is
// already PrefixFunctionSearch's own name for a related concept.
//
// Same Representation/comparer shape as PrefixFunctionSearch/Manacher: no repo-authored
// Representation contract (ReadOnlySpan<char> already gives O(1) indexed access), and character
// equality stays a plain IEqualityComparer<char> parameter rather than a witness.
//
// result[0] is left at its zero-initialized default rather than the "true" self-comparison value
// (text.Length) - the same treatment PrefixFunctionSearch.ComputeFailureFunction gives failure[0].
// The true value is degenerate (a string trivially shares its whole self as a prefix) and not
// useful to any consumer; FindAll below never dereferences it (see its own doc comment).
internal static class ZFunction
{
    // O(text.Length): each character is visited a number of times bounded by the box-tracking
    // invariant below, never re-scanned from the start on a mismatch.
    public static int[] Compute(ReadOnlySpan<char> text)
        => Compute(text, EqualityComparer<char>.Default);

    public static int[] Compute(ReadOnlySpan<char> text, IEqualityComparer<char> comparer)
    {
        var z = new int[text.Length];
        var box = new MatchedWindow(0, 0);

        for (var i = 1; i < text.Length; i++)
        {
            var k = SeedFromBox(z, box, i);

            while (CanExtendSelfReferential(text, i, k, comparer))
            {
                k++;
            }

            z[i] = k;
            box = box.ExpandTo(i, i + k);
        }

        return z;
    }

    private static int SeedFromBox(int[] selfZ, MatchedWindow box, int position)
        => position < box.Right ? Math.Min(selfZ[position - box.Left], box.Right - position) : 0;

    private static bool CanExtendSelfReferential(ReadOnlySpan<char> text, int position, int matched, IEqualityComparer<char> comparer)
        => position + matched < text.Length && comparer.Equals(text[matched], text[position + matched]);

    // Every starting index in text where pattern occurs, including overlapping occurrences, in the
    // order they're found. O(text.Length + pattern.Length).
    //
    // No concatenated/sentinel-separated buffer, the common textbook presentation of Z-function
    // pattern search: the same reason Manacher rejects sentinel-injected transforms applies
    // identically here - ReadOnlySpan<char> gives no generic way to guarantee a separator char is
    // absent from the input alphabet. Instead, patternZ (a normal, self-referential Z-array over
    // pattern alone) is computed once, then a second, independent, CROSS-referential box-tracked
    // scan walks text directly against pattern - never physically joining the two spans. The
    // extension is capped at pattern.Length: a search never needs to know a match extends past the
    // whole pattern, which is exactly the bound a real separator would otherwise enforce.
    //
    // An empty pattern matches at every insertion point, 0..text.Length inclusive - the same
    // convention PrefixFunctionSearch.FindAll uses - handled as an explicit guard rather than
    // falling through naturally, since the unguarded scan below dereferences pattern[0] before it
    // would ever notice pattern is empty.
    public static List<int> FindAll(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern)
        => FindAll(text, pattern, EqualityComparer<char>.Default);

    public static List<int> FindAll(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern, IEqualityComparer<char> comparer)
    {
        if (pattern.IsEmpty)
        {
            return MatchEveryInsertionPoint(text);
        }

        var patternZ = Compute(pattern, comparer);
        return SearchAgainstPattern(text, pattern, patternZ, comparer);
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

    private static List<int> SearchAgainstPattern(
        ReadOnlySpan<char> text, ReadOnlySpan<char> pattern, int[] patternZ, IEqualityComparer<char> comparer)
    {
        var matches = new List<int>();
        var box = new MatchedWindow(0, 0);
        var probe = new CrossReferentialProbe(text, pattern, comparer);

        for (var i = 0; i < text.Length; i++)
        {
            var k = SeedFromPatternBox(patternZ, box, i);

            while (CanExtendCrossReferential(probe, i, k))
            {
                k++;
            }

            if (k == pattern.Length)
            {
                matches.Add(i);
            }

            box = box.ExpandTo(i, i + k);
        }

        return matches;
    }

    // Mirrors SeedFromBox, but the seed source is patternZ (already fully computed over pattern
    // alone) rather than the in-progress text-side result - a different, cross-referential box
    // whose mirror value must never be read from the same array the caller is still building.
    private static int SeedFromPatternBox(int[] patternZ, MatchedWindow box, int position)
        => position < box.Right ? Math.Min(patternZ[position - box.Left], box.Right - position) : 0;

    private static bool CanExtendCrossReferential(CrossReferentialProbe probe, int position, int matched)
        => matched < probe.Pattern.Length && position + matched < probe.Text.Length
            && probe.Comparer.Equals(probe.Pattern[matched], probe.Text[position + matched]);

    // Bundles the three values SearchAgainstPattern's loop holds fixed across every iteration -
    // text, pattern, comparer - into one parameter, the same "group the least-frequently-varying
    // args into a type" recipe ARCHITECTURE.md §6/§11.3 already names, keeping
    // CanExtendCrossReferential at this repo's parameter-count limit. A `ref struct`, not a
    // `record struct` like FailureFunctionMatcher/LaneState: ReadOnlySpan<char> can only ever be a
    // field of another ref struct, never of an ordinary struct or class.
    private readonly ref struct CrossReferentialProbe(ReadOnlySpan<char> text, ReadOnlySpan<char> pattern, IEqualityComparer<char> comparer)
    {
        public ReadOnlySpan<char> Text { get; } = text;

        public ReadOnlySpan<char> Pattern { get; } = pattern;

        public IEqualityComparer<char> Comparer { get; } = comparer;
    }

    // The [Left, Right) bounds of the rightmost-reaching match found so far - the one piece of
    // state both scans above carry across loop iterations to seed the next extension via each
    // index's mirror (i - Left) instead of starting from scratch. ExpandTo only replaces it when
    // the new match actually reaches further right, mirroring Manacher's RightmostPalindromeEdge.
    private readonly record struct MatchedWindow(int Left, int Right)
    {
        public MatchedWindow ExpandTo(int left, int right)
            => right > Right ? new MatchedWindow(left, right) : this;
    }
}
