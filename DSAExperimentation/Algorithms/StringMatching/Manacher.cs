namespace DSAExperimentation.Algorithms.StringMatching;

// Manacher's algorithm: for every position, the radius of the longest odd-length
// and even-length palindrome centered there, computed in O(text.Length) total
// despite the appearance of a nested/expanding loop inside the outer scan - the
// key insight (seeding each expansion from the mirror of an already-computed
// radius, bounded by the rightmost edge any earlier palindrome has reached) caps
// the total expansion work across the whole scan, the same amortized-bound shape
// PrefixFunctionSearch's failure-function fallback already relies on.
//
// Named for the technique's inventor, not descriptively - unlike BinarySearch/
// MergeSort/PrefixFunctionSearch, "Manacher's algorithm" has no equally crisp
// descriptive alternative that wouldn't just restate "compute palindrome radii,"
// which is already the method names below.
//
// No transformed/separator-injected string, the common textbook presentation
// (e.g. inserting '#' between every character so odd/even collapse into one
// pass): that needs a sentinel character guaranteed absent from the input
// alphabet, which ReadOnlySpan<char> gives no generic way to guarantee, plus
// off-by-two index translation back to the caller's original positions. The
// twin-array (odd/even) formulation below needs neither - same technique, same
// O(n) bound, no transformation and no sentinel.
//
// Same Representation/comparer shape as PrefixFunctionSearch: no repo-authored
// Representation contract (ReadOnlySpan<char> already gives O(1) indexed access,
// structurally guaranteed rather than an unenforced obligation), and character
// equality stays a plain IEqualityComparer<char> parameter rather than a witness
// - case-insensitive/culture-aware matching are open-ended caller choices, not a
// closed set this library enumerates.
internal static class Manacher
{
    // A radius counts outward from the center on one side only; a palindrome's
    // full length always covers both sides of it.
    private const int RadiusToLengthFactor = 2;

    // ComputeOddRadii(text)[i] = k means text[i-k+1 .. i+k-1] (length 2k-1) is the
    // longest odd-length palindrome centered at i; k is always >= 1 (a single
    // character is its own odd-length palindrome).
    public static int[] ComputeOddRadii(ReadOnlySpan<char> text)
        => ComputeOddRadii(text, EqualityComparer<char>.Default);

    public static int[] ComputeOddRadii(ReadOnlySpan<char> text, IEqualityComparer<char> comparer)
    {
        var radii = new int[text.Length];
        var edge = new RightmostPalindromeEdge(0, -1);

        for (var i = 0; i < text.Length; i++)
        {
            var k = i > edge.Right ? 1 : Math.Min(radii[edge.Left + edge.Right - i], edge.Right - i + 1);

            while (CanExtendOddMatch(text, i, k, comparer))
            {
                k++;
            }

            radii[i] = k;
            edge = edge.ExpandTo(i - k + 1, i + k - 1);
        }

        return radii;
    }

    private static bool CanExtendOddMatch(
        ReadOnlySpan<char> text, int center, int radius, IEqualityComparer<char> comparer)
        => center - radius >= 0 && center + radius < text.Length
            && comparer.Equals(text[center - radius], text[center + radius]);

    // ComputeEvenRadii(text)[i] = k means text[i-k .. i+k-1] (length 2k) is the
    // longest even-length palindrome whose center falls between i-1 and i; k may
    // be 0 (no even-length palindrome is centered there).
    public static int[] ComputeEvenRadii(ReadOnlySpan<char> text)
        => ComputeEvenRadii(text, EqualityComparer<char>.Default);

    public static int[] ComputeEvenRadii(ReadOnlySpan<char> text, IEqualityComparer<char> comparer)
    {
        var radii = new int[text.Length];
        var edge = new RightmostPalindromeEdge(0, -1);

        for (var i = 0; i < text.Length; i++)
        {
            var k = i > edge.Right ? 0 : Math.Min(radii[edge.Left + edge.Right - i + 1], edge.Right - i + 1);

            while (CanExtendEvenMatch(text, i, k, comparer))
            {
                k++;
            }

            radii[i] = k;
            edge = edge.ExpandTo(i - k, i + k - 1);
        }

        return radii;
    }

    private static bool CanExtendEvenMatch(
        ReadOnlySpan<char> text, int center, int radius, IEqualityComparer<char> comparer)
        => center - radius - 1 >= 0 && center + radius < text.Length
            && comparer.Equals(text[center - radius - 1], text[center + radius]);

    // The [Start, Start+Length) bounds of a longest palindromic substring. Ties
    // (multiple palindromes of the same maximal length) resolve to whichever
    // center this scan reaches first. Empty text returns (0, 0).
    public static (int Start, int Length) FindLongestPalindromicSubstring(ReadOnlySpan<char> text)
        => FindLongestPalindromicSubstring(text, EqualityComparer<char>.Default);

    public static (int Start, int Length) FindLongestPalindromicSubstring(
        ReadOnlySpan<char> text, IEqualityComparer<char> comparer)
    {
        if (text.IsEmpty)
        {
            return (0, 0);
        }

        var oddRadii = ComputeOddRadii(text, comparer);
        var evenRadii = ComputeEvenRadii(text, comparer);
        var longestOdd = LongestFromOddRadii(oddRadii);
        var longestEven = LongestFromEvenRadii(evenRadii);

        return longestEven.Length > longestOdd.Length ? longestEven : longestOdd;
    }

    private static (int Start, int Length) LongestFromOddRadii(int[] oddRadii)
    {
        var center = IndexOfMax(oddRadii);
        var radius = oddRadii[center];
        return (center - radius + 1, (RadiusToLengthFactor * radius) - 1);
    }

    private static (int Start, int Length) LongestFromEvenRadii(int[] evenRadii)
    {
        var center = IndexOfMax(evenRadii);
        var radius = evenRadii[center];
        return (center - radius, RadiusToLengthFactor * radius);
    }

    private static int IndexOfMax(int[] values)
    {
        var bestIndex = 0;

        for (var i = 1; i < values.Length; i++)
        {
            if (values[i] > values[bestIndex])
            {
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    // The [Left, Right] bounds of the rightmost-reaching palindrome found so far -
    // the one piece of state ComputeOddRadii/ComputeEvenRadii each carry across
    // loop iterations to seed the next expansion (via each index's mirror across
    // this palindrome, Left + Right - i) instead of starting from scratch.
    // ExpandTo only replaces it when the new palindrome actually reaches further
    // right, mirroring PrefixFunctionSearch.Advance's own "only fall back through
    // what's already known, never restart" shape.
    private readonly record struct RightmostPalindromeEdge(int Left, int Right)
    {
        public RightmostPalindromeEdge ExpandTo(int left, int right)
            => right > Right ? new RightmostPalindromeEdge(left, right) : this;
    }
}
