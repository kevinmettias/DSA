using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.LeetCode.SqrtX;

namespace DSAExperimentation.LeetCode.FourDivisors;

// LeetCode 1390. Four Divisors: sum the divisors of every number that has exactly
// four of them, and sum those sums across the input.
//
// Both strategies answer the same per-number question and differ only in where the
// divisor walk starts. The baseline scans 1..num. The composed strategy anchors at
// floor(sqrt(num)) with one BinarySearch.LowerBound over SqrtX's monotone
// SquareExceedsSequence - LC 69's own "does i^2 exceed x" witness, reused rather than
// copied, the same technique ClosestDivisors uses for LC 1362 - then walks down,
// collecting both members of each divisor pair and bailing out the moment a fifth
// divisor appears.
internal static class FourDivisorsSolution
{
    private const int TargetDivisorCount = 4;

    // A divisor below sqrt(num) and its partner above it are two distinct divisors.
    private const int DistinctDivisorPairCount = 2;

    // ceil(sqrt(int.MaxValue)): caps the binary-search anchor so squaring an index can
    // never overflow the search range.
    private const int SqrtAnchorCeiling = 46_341;

    // The textbook answer: trial-divide every number by 1..num, plain BCL arithmetic
    // with no search structure at all, giving up as soon as a fifth divisor appears.
    public static int SumFourDivisorsByFullRangeScan(int[] nums)
    {
        var total = 0;

        foreach (var num in nums)
        {
            total += FullRangeDivisorSumIfExactlyFour(num);
        }

        return total;
    }

    private static int FullRangeDivisorSumIfExactlyFour(int num)
    {
        var count = 0;
        var sum = 0;

        for (var divisor = 1; divisor <= num; divisor++)
        {
            if (num % divisor != 0)
            {
                continue;
            }

            count++;
            sum += divisor;

            if (count > TargetDivisorCount)
            {
                return 0;
            }
        }

        return count == TargetDivisorCount ? sum : 0;
    }

    // Anchor at floor(sqrt(num)) in O(log num), then walk down collecting divisor
    // pairs - a number with more than four divisors stops almost immediately instead
    // of scanning all the way to 1.
    public static int SumFourDivisorsByBinarySearchAnchor(int[] nums)
    {
        var total = 0;

        foreach (var num in nums)
        {
            total += AnchoredDivisorSumIfExactlyFour(num);
        }

        return total;
    }

    private static int AnchoredDivisorSumIfExactlyFour(int num)
    {
        var sequence = new SquareExceedsSequence(num, Math.Min(num, SqrtAnchorCeiling) + 1);
        var anchor = BinarySearch.LowerBound<int, SquareExceedsSequence>(sequence, 1) - 1;

        var tally = new DivisorTally();

        for (var divisor = anchor; divisor >= 1; divisor--)
        {
            if (!tally.TryAccumulate(num, divisor))
            {
                return 0;
            }
        }

        return tally.Count == TargetDivisorCount ? tally.Sum : 0;
    }

    // Running count and sum of the divisors found so far; TryAccumulate reports false
    // once the number is known to have more than four, so the walk can stop.
    private sealed class DivisorTally
    {
        public int Count { get; private set; }

        public int Sum { get; private set; }

        public bool TryAccumulate(int num, int divisor)
        {
            if (num % divisor != 0)
            {
                return true;
            }

            var paired = num / divisor;
            Count += divisor == paired ? 1 : DistinctDivisorPairCount;
            Sum += divisor == paired ? divisor : DivisorPairSum(divisor, paired);

            return Count <= TargetDivisorCount;
        }

        // A non-square divisor arrives with its distinct partner, and both count toward
        // the sum; a square divisor was already added alone.
        private static int DivisorPairSum(int divisor, int paired) => divisor + paired;
    }
}
