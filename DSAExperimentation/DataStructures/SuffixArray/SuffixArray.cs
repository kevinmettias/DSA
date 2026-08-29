namespace DSAExperimentation.DataStructures.SuffixArray;

// Precomputes, from a fixed text, the permutation of every suffix's starting index in ascending
// lexicographic order (Suffixes), that permutation's inverse (Rank), and the length of the longest
// common prefix between every pair of lexicographically adjacent suffixes (LongestCommonPrefixArray, via Kasai's
// algorithm) - all in the constructor, O(text.Length * log(text.Length)^2) total. Same "precompute
// once, expose O(1)-indexed query arrays" stateful-Representation shape as RollingHash, and the
// same reason this is an `internal sealed class` rather than a `readonly struct`: multi-array state,
// not a small value bundle.
//
// Construction: prefix doubling. Round k starts from each suffix's rank under the previous round's
// 2^(k/2)-character prefix comparison and refines it using the pair (rank[i], rank[i+k] or -1 if
// i+k is out of range) as the new sort key - the -1 sentinel is load-bearing, not a placeholder: it
// must sort strictly before every attainable real rank (always >= 0) so that a suffix running off
// the end of text is correctly treated as lexicographically smaller than one that doesn't, at every
// doubling round. This is O(n log^2 n) (O(log n) rounds, each an O(n log n) comparison sort) rather
// than the O(n log n) achievable with a radix/bucket-sort refinement at each round - a real,
// intentional trade-off, not an oversight: no radix-sort infrastructure exists anywhere else in
// this repo, and introducing single-use bucket-refinement machinery here to save one log factor
// would be exactly the "don't build structure before it's earned" instinct ARCHITECTURE.md §5 step
// 3 already applies to interfaces, applied here to sorting instead. A future O(n log n) upgrade
// would only ever need to replace the private sort-step helper below, not any public member.
//
// Character ordering (ordinal vs. culture-aware vs. case-insensitive) is an open-ended caller
// choice, so it stays a plain runtime IComparer<char> parameter with a Comparer<char>.Default
// default overload - not a witness, the same classification BinarySearch's own comparer gets - and
// notably the first IComparer<char> (ordering) rather than IEqualityComparer<char> (equality only)
// in this repo's string-matching family, since sorting suffixes fundamentally needs a total order.
//
// text is copied once into a local char[] at construction: a ReadOnlySpan<char> can't be captured
// by the Comparison<int> delegate Array.Sort needs, and unlike RollingHash's single synchronous
// pass, this algorithm re-reads characters repeatedly across every doubling round and again during
// Kasai's pass - a real necessity here, not an optional convenience.
internal sealed class SuffixArray
{
    private const int NoRankSentinel = -1;
    private const int DoublingFactor = 2;

    public int[] Suffixes { get; }

    public int[] Rank { get; }

    public int[] LongestCommonPrefixArray { get; }

    public SuffixArray(ReadOnlySpan<char> text)
        : this(text, Comparer<char>.Default)
    {
    }

    public SuffixArray(ReadOnlySpan<char> text, IComparer<char> comparer)
    {
        var buffer = text.ToArray();
        Suffixes = BuildSuffixArray(buffer, comparer);
        Rank = BuildRank(Suffixes);
        LongestCommonPrefixArray = BuildLongestCommonPrefixArray(buffer, Suffixes, Rank, comparer);
    }

    private static int[] BuildSuffixArray(char[] buffer, IComparer<char> comparer)
    {
        var n = buffer.Length;
        var suffixes = InitialOrder(n);
        var rank = BootstrapRank(buffer, suffixes, comparer);

        for (var k = 1; k < n; k *= DoublingFactor)
        {
            var context = new DoublingContext(rank, k, n);
            Array.Sort(suffixes, (a, b) => CompareByDoubledKey(a, b, context));
            rank = RefineRank(suffixes, context);

            if (rank[suffixes[n - 1]] == n - 1)
            {
                break;
            }
        }

        return suffixes;
    }

    private static int[] InitialOrder(int length)
    {
        var suffixes = new int[length];

        for (var i = 0; i < length; i++)
        {
            suffixes[i] = i;
        }

        return suffixes;
    }

    private static int[] BootstrapRank(char[] buffer, int[] suffixes, IComparer<char> comparer)
    {
        var n = buffer.Length;
        Array.Sort(suffixes, (a, b) => comparer.Compare(buffer[a], buffer[b]));

        var rank = new int[n];

        for (var i = 1; i < n; i++)
        {
            var comparison = comparer.Compare(buffer[suffixes[i - 1]], buffer[suffixes[i]]);
            rank[suffixes[i]] = rank[suffixes[i - 1]] + DistinctIncrement(comparison);
        }

        return rank;
    }

    private static int DistinctIncrement(int comparison) => comparison == 0 ? 0 : 1;

    private static int CompareByDoubledKey(int left, int right, DoublingContext context)
    {
        var primary = context.Rank[left].CompareTo(context.Rank[right]);

        if (primary != 0)
        {
            return primary;
        }

        var secondHalfLeft = SecondHalfRank(left, context);
        var secondHalfRight = SecondHalfRank(right, context);
        return secondHalfLeft.CompareTo(secondHalfRight);
    }

    private static int[] RefineRank(int[] suffixes, DoublingContext context)
    {
        var rank = new int[context.N];

        for (var i = 1; i < context.N; i++)
        {
            var tied = HasEqualDoubledKey(suffixes[i - 1], suffixes[i], context);
            rank[suffixes[i]] = rank[suffixes[i - 1]] + (tied ? 0 : 1);
        }

        return rank;
    }

    private static bool HasEqualDoubledKey(int left, int right, DoublingContext context)
        => context.Rank[left] == context.Rank[right] && SecondHalfRank(left, context) == SecondHalfRank(right, context);

    private static int[] BuildRank(int[] suffixes)
    {
        var rank = new int[suffixes.Length];

        for (var i = 0; i < suffixes.Length; i++)
        {
            rank[suffixes[i]] = i;
        }

        return rank;
    }

    // Kasai's algorithm: walks text in ORIGINAL index order (not suffix-array order), which is what
    // lets h only ever drop by at most 1 between consecutive i's (the key amortized-O(n) insight) -
    // dropping straight to a fresh scan at every i would be O(n^2) on a degenerate all-one-character
    // text.
    private static int[] BuildLongestCommonPrefixArray(char[] buffer, int[] suffixes, int[] rank, IComparer<char> comparer)
    {
        var n = buffer.Length;
        var longestCommonPrefixes = new int[Math.Max(n - 1, 0)];
        var h = 0;
        var context = new KasaiContext(buffer, comparer);

        for (var currentSuffixStart = 0; currentSuffixStart < n; currentSuffixStart++)
        {
            var hasPrecedingSuffix = rank[currentSuffixStart] > 0;
            h = hasPrecedingSuffix
                ? ExtendedCommonPrefixLength(context, suffixes[rank[currentSuffixStart] - 1], currentSuffixStart, h)
                : 0;

            if (hasPrecedingSuffix)
            {
                longestCommonPrefixes[rank[currentSuffixStart] - 1] = h;
                h = Math.Max(h - 1, 0);
            }
        }

        return longestCommonPrefixes;
    }

    private static int ExtendedCommonPrefixLength(KasaiContext context, int previousSuffixStart, int currentSuffixStart, int startingOffset)
    {
        while (HasMatchingCharactersAtOffset(context, previousSuffixStart, currentSuffixStart, startingOffset))
        {
            startingOffset++;
        }

        return startingOffset;
    }

    private static bool HasMatchingCharactersAtOffset(KasaiContext context, int previousSuffixStart, int currentSuffixStart, int offset)
        => previousSuffixStart + offset < context.Buffer.Length && currentSuffixStart + offset < context.Buffer.Length
            && context.Comparer.Compare(context.Buffer[previousSuffixStart + offset], context.Buffer[currentSuffixStart + offset]) == 0;

    // SecondHalfRank is shared by CompareByDoubledKey AND HasEqualDoubledKey (two distinct callers),
    // placed here last for that reason - contrast DistinctIncrement/CompareByDoubledKey/RefineRank/
    // HasEqualDoubledKey/ExtendedCommonPrefixLength above, each with exactly one caller, placed
    // immediately after it in DFS call order.
    private static int SecondHalfRank(int suffixIndex, DoublingContext context)
    {
        var hasSecondHalf = suffixIndex + context.K < context.N;
        return hasSecondHalf ? RankAfterOffset(suffixIndex, context) : NoRankSentinel;
    }

    private static int RankAfterOffset(int suffixIndex, DoublingContext context) => context.Rank[suffixIndex + context.K];

    // Bundles the three values every doubling-round helper needs beyond the two suffix indices
    // being compared - the previous round's rank array, the current doubling step k, and text
    // length n - the same "group the args every helper in this round shares" recipe
    // ARCHITECTURE.md §6/§11.3 already names, keeping CompareByDoubledKey/HasEqualDoubledKey at
    // this repo's parameter-count limit.
    private readonly record struct DoublingContext(int[] Rank, int K, int N);

    // Bundles the buffer/comparer pair every Kasai-pass helper needs beyond its own per-suffix
    // arguments - both fixed for the whole pass, the same "least-frequently-varying args" grouping
    // DoublingContext already uses above.
    private readonly record struct KasaiContext(char[] Buffer, IComparer<char> Comparer);
}
