namespace DSAExperimentation.LeetCode.MaximumScoreWithCoPrimeElement;

// LeetCode 3953. Maximum Score with Co-Prime Element: pick a final value v for
// one index and pay 1 for every OTHER element that has to be changed (to
// something <= maxVal) to stay coprime with v; score = v - cost, maximized.
//
// Every candidate v falls into one of two shapes:
//   (a) v = some original nums[i], left unchanged (cost 0 for that slot).
//       nums[i] can exceed maxVal here - it was never "changed", so the cap
//       never applied to it.
//   (b) v is any value in [1, maxVal], with some index forced to become it
//       (cost 1 for that slot - cheapest when that slot was already going to
//       be changed anyway, i.e. was already non-coprime with v).
// Either way the cost is "how many of the n elements conflict with v" (call it
// bad(v)), adjusted for whichever element is v's own seat: case (a) subtracts 1
// only when v != 1, because gcd(v, v) = v registers index i itself as a
// conflict inside bad(v) whenever v > 1, and that self-conflict has to be
// excluded, not fixed; case (b) spends at least 1 regardless, since some slot
// must actually become v. Both arms below just evaluate every v this way; they
// differ only in how bad(v) - "n minus how many original elements are coprime
// with v" - gets computed.
internal static class MaximumScoreWithCoPrimeElementSolution
{
    // Textbook baseline: bad(v) by direct Euclidean gcd against every element,
    // for every candidate v. O(n * (n + maxVal)) - the arm the divisor-sieve
    // strategy below has to beat.
    public static int MaximumScoreByBruteForce(int[] nums, int maxVal)
    {
        var best = int.MinValue;

        foreach (var value in nums)
        {
            var bad = CountConflicts(nums, value);
            var cost = value == 1 ? bad : bad - 1;
            best = Math.Max(best, value - cost);
        }

        for (var v = 1; v <= maxVal; v++)
        {
            var bad = CountConflicts(nums, v);
            best = Math.Max(best, v - Math.Max(bad, 1));
        }

        return best;
    }

    private static int CountConflicts(int[] nums, int v)
    {
        var bad = 0;

        foreach (var value in nums)
        {
            if (Gcd(value, v) != 1)
            {
                bad++;
            }
        }

        return bad;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    // Composed: bad(v) = n - coprimeCount(v), and coprimeCount(v) is answered
    // by inclusion-exclusion over v's own distinct prime factors against a
    // precomputed divisor-multiple sieve - "how many array elements are
    // divisible by d" for every d up to the shared value ceiling - rather than
    // a fresh O(n) gcd scan per candidate. v has at most ~6 distinct prime
    // factors below 1e5 (2*3*5*7*11*13 = 30030, the next prime already exceeds
    // it), so each candidate costs O(2^6) once the two sieves are built.
    public static int MaximumScoreByDivisorSieve(int[] nums, int maxVal)
    {
        var limit = Math.Max(maxVal, nums.Length == 0 ? 1 : nums.Max());
        var smallestPrimeFactor = BuildSmallestPrimeFactorSieve(limit);
        var divisorCount = BuildDivisorCountSieve(nums, limit);
        var n = nums.Length;
        var best = int.MinValue;

        foreach (var value in nums)
        {
            var bad = n - CoprimeCount(value, smallestPrimeFactor, divisorCount);
            var cost = value == 1 ? bad : bad - 1;
            best = Math.Max(best, value - cost);
        }

        for (var v = 1; v <= maxVal; v++)
        {
            var bad = n - CoprimeCount(v, smallestPrimeFactor, divisorCount);
            best = Math.Max(best, v - Math.Max(bad, 1));
        }

        return best;
    }

    private static int[] BuildSmallestPrimeFactorSieve(int limit)
    {
        var smallestPrimeFactor = new int[limit + 1];

        for (var candidate = 2; candidate <= limit; candidate++)
        {
            if (smallestPrimeFactor[candidate] != 0)
            {
                continue;
            }

            for (var multiple = candidate; multiple <= limit; multiple += candidate)
            {
                if (smallestPrimeFactor[multiple] == 0)
                {
                    smallestPrimeFactor[multiple] = candidate;
                }
            }
        }

        return smallestPrimeFactor;
    }

    // divisorCount[d] = how many elements of nums are divisible by d, built by
    // summing each value's own frequency across every one of its multiples -
    // the same "iterate multiples of d" sieve shape as Eratosthenes, O(limit
    // log limit) total.
    private static int[] BuildDivisorCountSieve(int[] nums, int limit)
    {
        var valueFrequency = new int[limit + 1];

        foreach (var value in nums)
        {
            valueFrequency[value]++;
        }

        var divisorCount = new int[limit + 1];

        for (var d = 1; d <= limit; d++)
        {
            var count = 0;

            for (var multiple = d; multiple <= limit; multiple += d)
            {
                count += valueFrequency[multiple];
            }

            divisorCount[d] = count;
        }

        return divisorCount;
    }

    private static int CoprimeCount(int v, int[] smallestPrimeFactor, int[] divisorCount)
    {
        var primes = DistinctPrimeFactors(v, smallestPrimeFactor);
        var subsetCount = 1 << primes.Count;
        var total = 0;

        for (var mask = 0; mask < subsetCount; mask++)
        {
            var product = 1;
            var bitCount = 0;

            for (var bit = 0; bit < primes.Count; bit++)
            {
                if ((mask & (1 << bit)) == 0)
                {
                    continue;
                }

                product *= primes[bit];
                bitCount++;
            }

            total += bitCount % 2 == 0 ? divisorCount[product] : -divisorCount[product];
        }

        return total;
    }

    private static List<int> DistinctPrimeFactors(int v, int[] smallestPrimeFactor)
    {
        var primes = new List<int>();

        while (v > 1)
        {
            var prime = smallestPrimeFactor[v];
            primes.Add(prime);

            while (v % prime == 0)
            {
                v /= prime;
            }
        }

        return primes;
    }
}
