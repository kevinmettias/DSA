using DSAExperimentation.DataStructures.Deque;

namespace DSAExperimentation.LeetCode.CountPrimeGapBalancedSubarrays;

// LeetCode 3589. Count Prime-Gap Balanced Subarrays: count contiguous subarrays
// of nums that contain at least two primes, where the gap between the largest
// and smallest prime inside the subarray is at most k.
internal static class CountPrimeGapBalancedSubarraysSolution
{
    // Textbook O(n^2): fix the left end, extend the right end one element at a
    // time, tracking the running prime count/min/max directly against every
    // window - the arm the compact-prime two-pointer strategy below has to beat.
    public static long CountByBruteForce(int[] nums, int k)
    {
        var isPrime = BuildPrimeFlags(nums);
        long total = 0;

        for (var left = 0; left < nums.Length; left++)
        {
            var count = 0;
            var min = int.MaxValue;
            var max = int.MinValue;

            for (var right = left; right < nums.Length; right++)
            {
                if (isPrime[right])
                {
                    count++;
                    min = Math.Min(min, nums[right]);
                    max = Math.Max(max, nums[right]);
                }

                if (count >= 2 && max - min <= k)
                {
                    total++;
                }
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
    public static long CountByPrimeWindowDeque(int[] nums, int k)
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

        var primeCount = primeValues.Count;

        if (primeCount < 2)
        {
            return 0;
        }

        // leftChoices[i]: how many array positions l make primeIndices[i] the
        // leftmost prime in [l, r] (from just past the previous prime up to this
        // prime's own index). rightChoices[i]: the symmetric count for r making
        // primeIndices[i] the rightmost prime.
        var leftChoices = new long[primeCount];
        var rightChoices = new long[primeCount];

        for (var i = 0; i < primeCount; i++)
        {
            var previousPrimeIndex = i > 0 ? primeIndices[i - 1] : -1;
            var nextPrimeIndex = i < primeCount - 1 ? primeIndices[i + 1] : n;
            leftChoices[i] = primeIndices[i] - previousPrimeIndex;
            rightChoices[i] = nextPrimeIndex - primeIndices[i];
        }

        var prefixLeft = new long[primeCount];
        prefixLeft[0] = leftChoices[0];

        for (var i = 1; i < primeCount; i++)
        {
            prefixLeft[i] = prefixLeft[i - 1] + leftChoices[i];
        }

        var maxDeque = new Deque<int>();
        var minDeque = new Deque<int>();
        var windowStart = 0;
        long total = 0;

        for (var q = 0; q < primeCount; q++)
        {
            while (maxDeque.TryPeekBack(out var back) && primeValues[back] <= primeValues[q])
            {
                maxDeque.TryPopBack(out _);
            }

            maxDeque.PushBack(q);

            while (minDeque.TryPeekBack(out var back) && primeValues[back] >= primeValues[q])
            {
                minDeque.TryPopBack(out _);
            }

            minDeque.PushBack(q);

            while (maxDeque.TryPeekFront(out var hi) && minDeque.TryPeekFront(out var lo) &&
                   primeValues[hi] - primeValues[lo] > k)
            {
                windowStart++;

                if (maxDeque.TryPeekFront(out var staleHi) && staleHi < windowStart)
                {
                    maxDeque.TryPopFront(out _);
                }

                if (minDeque.TryPeekFront(out var staleLo) && staleLo < windowStart)
                {
                    minDeque.TryPopFront(out _);
                }
            }

            if (q - 1 < windowStart)
            {
                continue;
            }

            var leftSum = prefixLeft[q - 1] - (windowStart > 0 ? prefixLeft[windowStart - 1] : 0);
            total += leftSum * rightChoices[q];
        }

        return total;
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
