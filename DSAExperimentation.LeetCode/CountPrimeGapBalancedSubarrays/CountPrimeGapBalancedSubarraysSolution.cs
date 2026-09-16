using DSAExperimentation.DataStructures.Deque;

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
    // window over that compact subsequence: this repo's own Deque<int> (of
    // prime-subsequence indices) tracks the window's running max and min in
    // amortized O(1) per step - the "monotonic deque" technique
    // DataStructures.Deque exists to support - and a prefix sum over each
    // prime's left-side gap width turns "sum over every valid left endpoint"
    // into an O(1) lookup per right endpoint.
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
    // that carry the window's running max and min.
    private static long CountSubarraysOverPrimes(
        List<int> primeValues, List<int> primeIndices, int arrayLength, int maxGap)
    {
        var (leftChoices, rightChoices) = BuildChoiceCounts(primeIndices, arrayLength);
        var prefixLeft = BuildPrefixSums(leftChoices);
        var maxDeque = new Deque<int>();
        var minDeque = new Deque<int>();
        var windowStart = 0;
        long total = 0;

        for (var windowEnd = 0; windowEnd < primeValues.Count; windowEnd++)
        {
            PushPrimeIntoWindow(maxDeque, minDeque, primeValues, windowEnd);
            windowStart = ShrinkToGapLimit((maxDeque, minDeque), primeValues, maxGap, windowStart);
            total += CountSubarraysEndingAt(prefixLeft, rightChoices, windowStart, windowEnd);
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

    // Running totals of each prime's left-side gap width, so the sum over every
    // valid left endpoint is one subtraction.
    private static long[] BuildPrefixSums(long[] leftChoices)
    {
        var prefixLeft = new long[leftChoices.Length];
        prefixLeft[0] = leftChoices[0];

        for (var i = 1; i < leftChoices.Length; i++)
        {
            prefixLeft[i] = prefixLeft[i - 1] + leftChoices[i];
        }

        return prefixLeft;
    }

    // The prime at windowEnd joins both monotonic deques: each one pops every back
    // index whose prime no longer holds its extreme, then takes windowEnd as the newest
    // candidate.
    private static void PushPrimeIntoWindow(
        Deque<int> maxDeque, Deque<int> minDeque, List<int> primeValues, int windowEnd)
    {
        while (maxDeque.TryPeekBack(out var back) && primeValues[back] <= primeValues[windowEnd])
        {
            maxDeque.TryPopBack(out _);
        }

        maxDeque.PushBack(windowEnd);

        while (minDeque.TryPeekBack(out var back) && primeValues[back] >= primeValues[windowEnd])
        {
            minDeque.TryPopBack(out _);
        }

        minDeque.PushBack(windowEnd);
    }

    // Advance the left end until the two deques' fronts sit within maxGap of each
    // other, dropping whichever front has fallen behind the new start.
    private static int ShrinkToGapLimit(
        (Deque<int> MaxDeque, Deque<int> MinDeque) deques, List<int> primeValues, int maxGap, int windowStart)
    {
        while (IsGapOverLimit(deques.MaxDeque, deques.MinDeque, primeValues, maxGap))
        {
            windowStart++;

            if (deques.MaxDeque.TryPeekFront(out var staleHi) && staleHi < windowStart)
            {
                deques.MaxDeque.TryPopFront(out _);
            }

            if (deques.MinDeque.TryPeekFront(out var staleLo) && staleLo < windowStart)
            {
                deques.MinDeque.TryPopFront(out _);
            }
        }

        return windowStart;
    }

    // The window's widest prime gap is over the limit, so its left end has to
    // advance - only decidable while both deques still hold the front index.
    private static bool IsGapOverLimit(
        Deque<int> maxDeque, Deque<int> minDeque, List<int> primeValues, int maxGap)
        => maxDeque.TryPeekFront(out var hi) && minDeque.TryPeekFront(out var lo)
            && primeValues[hi] - primeValues[lo] > maxGap;

    // How many subarrays ending at the prime at windowEnd the window contributes: the
    // width sum over the left endpoints still inside it, times the choices of right end
    // that keep that prime rightmost. A window that has not yet reached the prime
    // before windowEnd admits none, and the prefix lookup would run off the front.
    private static long CountSubarraysEndingAt(
        long[] prefixLeft, long[] rightChoices, int windowStart, int windowEnd)
    {
        if (windowEnd - 1 < windowStart)
        {
            return 0;
        }

        var leftSum = prefixLeft[windowEnd - 1] -
            (windowStart > 0 ? PrefixSumBefore(prefixLeft, windowStart) : 0);

        return leftSum * rightChoices[windowEnd];
    }

    // The prefix total immediately before a position - the sum of every
    // left-choice width ahead of it, and so of the window's own left endpoints.
    private static long PrefixSumBefore(long[] prefixLeft, int index) => prefixLeft[index - 1];

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
