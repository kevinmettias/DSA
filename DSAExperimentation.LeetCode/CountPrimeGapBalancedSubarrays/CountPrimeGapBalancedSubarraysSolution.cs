using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.MonotonicDeque;
using DSAExperimentation.DataStructures.PrefixSums;

namespace DSAExperimentation.LeetCode.CountPrimeGapBalancedSubarrays;

// LeetCode 3589. Count Prime-Gap Balanced Subarrays: count contiguous subarrays
// of nums that contain at least two primes, where the gap between the largest
// and smallest prime inside the subarray is at most maxGap.
internal static class CountPrimeGapBalancedSubarraysSolution
{
    // Textbook O(n^2): fix the left end, extend the right end one element at a
    // time, tracking the running prime count/min/max directly against every
    // window - the arm the compact-prime two-pointer strategy below has to beat.
    public static long CountByBruteForce(int[] nums, int maxGap)
    {
        var isPrime = BuildPrimeFlags(nums);
        long total = 0;

        for (var left = 0; left < nums.Length; left++)
        {
            total += CountSubarraysFromLeftAnchor(nums, isPrime, maxGap, left);
        }

        return total;
    }

    // One fixed left end: walk the right end outwards keeping the window's running
    // prime count and its smallest and largest prime, and count every extension
    // that holds at least two primes whose gap stays within maxGap.
    private static long CountSubarraysFromLeftAnchor(
        int[] nums, bool[] isPrime, int maxGap, int left)
    {
        var count = 0;
        var min = int.MaxValue;
        var max = int.MinValue;
        long total = 0;

        for (var right = left; right < nums.Length; right++)
        {
            if (isPrime[right])
            {
                count++;
                min = Math.Min(min, nums[right]);
                max = Math.Max(max, nums[right]);
            }

            if (count >= 2 && max - min <= maxGap)
            {
                total++;
            }
        }

        return total;
    }

    // Composed: only the primes' own positions and values ever affect the
    // balance test, so collapse nums to its prime subsequence first. Every
    // subarray of nums is then uniquely identified by which two primes it
    // starts and ends on (the non-prime run on either side just widens how many
    // array boundaries choose that same pair), so counting reduces to a sliding
    // window over that compact subsequence: two of this repo's MonotonicDeques
    // (positioned by prime-subsequence index) track the window's running max and
    // min in amortized O(1) per step, and PrefixSums over each prime's left-side
    // gap width turns "sum over every valid left endpoint" into an O(1) range
    // query per right endpoint.
    public static long CountByPrimeWindowDeque(int[] nums, int maxGap)
    {
        var isPrime = BuildPrimeFlags(nums);
        var n = nums.Length;

        var primeValues = new List<int>();
        var primeIndices = new List<int>();

        for (var i = 0; i < n; i++)
        {
            if (isPrime[i])
            {
                primeValues.Add(nums[i]);
                primeIndices.Add(i);
            }
        }

        if (primeValues.Count < 2)
        {
            return 0;
        }

        return CountSubarraysOverPrimes(primeValues, primeIndices, n, maxGap);
    }

    // The sliding window over the compact prime subsequence, in the two deques
    // that carry the window's running max and min. The prime at windowEnd joins
    // both, each evicting every earlier prime it dominates as that extreme.
    private static long CountSubarraysOverPrimes(
        List<int> primeValues, List<int> primeIndices, int arrayLength, int maxGap)
    {
        var (leftChoices, rightChoices) = BuildChoiceCounts(primeIndices, arrayLength);
        var leftChoiceTotals = new PrefixSums<long, SumOperation<long>>(leftChoices);
        var maxDeque = new MonotonicDeque<int, MaxWindowOrder<int>>();
        var minDeque = new MonotonicDeque<int, MinWindowOrder<int>>();
        var windowStart = 0;
        long total = 0;

        for (var windowEnd = 0; windowEnd < primeValues.Count; windowEnd++)
        {
            maxDeque.Push(windowEnd, primeValues[windowEnd]);
            minDeque.Push(windowEnd, primeValues[windowEnd]);
            windowStart = ShrinkToGapLimit((maxDeque, minDeque), maxGap, windowStart);
            total += CountSubarraysEndingAt(leftChoiceTotals, rightChoices, windowStart, windowEnd);
        }

        return total;
    }

    // leftChoices[i]: how many array positions l make primeIndices[i] the
    // leftmost prime in [l, r] (from just past the previous prime up to this
    // prime's own index). rightChoices[i]: the symmetric count for r making
    // primeIndices[i] the rightmost prime.
    private static (long[] LeftChoices, long[] RightChoices) BuildChoiceCounts(
        List<int> primeIndices, int arrayLength)
    {
        var primeCount = primeIndices.Count;
        var leftChoices = new long[primeCount];
        var rightChoices = new long[primeCount];

        for (var i = 0; i < primeCount; i++)
        {
            var hasNextPrime = i < primeCount - 1;
            var previousPrimeIndex = i > 0 ? PreviousPrimeIndex(primeIndices, i) : -1;
            var nextPrimeIndex = hasNextPrime ? NextPrimeIndex(primeIndices, i) : arrayLength;
            leftChoices[i] = primeIndices[i] - previousPrimeIndex;
            rightChoices[i] = nextPrimeIndex - primeIndices[i];
        }

        return (leftChoices, rightChoices);
    }

    // The prime immediately before position i in the compact subsequence - read
    // only when that prime exists.
    private static int PreviousPrimeIndex(List<int> primeIndices, int index) => primeIndices[index - 1];

    // The prime immediately after position i - read only when that prime exists.
    private static int NextPrimeIndex(List<int> primeIndices, int index) => primeIndices[index + 1];

    // Advance the left end until the two deques' fronts sit within maxGap of each
    // other, dropping whichever front has fallen behind the new start.
    private static int ShrinkToGapLimit(
        (MonotonicDeque<int, MaxWindowOrder<int>> MaxDeque, MonotonicDeque<int, MinWindowOrder<int>> MinDeque) deques,
        int maxGap,
        int windowStart)
    {
        while (IsGapOverLimit(deques.MaxDeque, deques.MinDeque, maxGap))
        {
            windowStart++;
            deques.MaxDeque.EvictBefore(windowStart);
            deques.MinDeque.EvictBefore(windowStart);
        }

        return windowStart;
    }

    // The window's widest prime gap is over the limit, so its left end has to
    // advance - only decidable while both deques still hold a front.
    private static bool IsGapOverLimit(
        MonotonicDeque<int, MaxWindowOrder<int>> maxDeque, MonotonicDeque<int, MinWindowOrder<int>> minDeque, int maxGap)
        => maxDeque.TryPeekFront(out var hi) && minDeque.TryPeekFront(out var lo)
            && hi.Key - lo.Key > maxGap;

    // How many subarrays ending at the prime at windowEnd the window contributes: the
    // width sum over the left endpoints still inside it - primes windowStart through
    // windowEnd - 1 - times the choices of right end that keep that prime rightmost.
    // A window that has not yet reached the prime before windowEnd admits none, and
    // that empty range has no left endpoints to sum.
    private static long CountSubarraysEndingAt(
        PrefixSums<long, SumOperation<long>> leftChoiceTotals, long[] rightChoices, int windowStart, int windowEnd)
    {
        if (windowEnd - 1 < windowStart)
        {
            return 0;
        }

        var leftSum = leftChoiceTotals.Query(windowStart, windowEnd - 1);

        return leftSum * rightChoices[windowEnd];
    }

    private static bool[] BuildPrimeFlags(int[] nums)
    {
        var flags = new bool[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            flags[i] = IsPrime(nums[i]);
        }

        return flags;
    }

    private static bool IsPrime(int value)
    {
        if (value < 2)
        {
            return false;
        }

        for (var divisor = 2; (long)divisor * divisor <= value; divisor++)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }
}
