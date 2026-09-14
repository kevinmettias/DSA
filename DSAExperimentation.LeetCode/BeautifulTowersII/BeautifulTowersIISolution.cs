using DSAExperimentation.LeetCode.BeautifulTowersI;

namespace DSAExperimentation.LeetCode.BeautifulTowersII;

// LeetCode 2866. Beautiful Towers II: word for word LeetCode 2865 (Beautiful Towers
// I) with one constraint loosened - n and maxHeights[i] rise to 1e5 and 1e9 - which
// is exactly what makes I's also-acceptable O(n^2) per-peak walk stop scaling and
// leaves the O(n) monotonic-stack sweep as the only arm that finishes.
//
// Because the question is the same question, both strategies ARE
// BeautifulTowersISolution's, reused rather than copied. Two problem folders each
// holding their own transcription of one mountain-clamp sweep is the drift
// ARCHITECTURE.md section 17.1 exists to remove, and the reuse follows the same
// tier-4-composes-tier-4 precedent as ThreeDivisors/FourDivisors over SqrtX's
// square-exceeds witness. What is genuinely this problem's own is the scale its
// harnesses measure and assert at, and that lives in the harnesses.
internal static class BeautifulTowersIISolution
{
    // The O(n^2) per-peak clamped walk. Kept as a named strategy here even though
    // LC 2866's own bound outruns it, because it is the arm the sweep's whole
    // justification is measured against - and a baseline nothing asserts is the
    // second half of the same defect.
    public static long MaximumSumOfHeightsByBruteForce(int[] maxHeights)
        => BeautifulTowersISolution.MaximumSumOfHeightsByBruteForce(maxHeights);

    // One monotonic-stack sweep per direction over this repo's own Stack<int>.
    public static long MaximumSumOfHeightsByMonotonicStack(int[] maxHeights)
        => BeautifulTowersISolution.MaximumSumOfHeightsByMonotonicStack(maxHeights);
}
