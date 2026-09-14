using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.ClosestPrimeNumbersInRange;

// LeetCode 2523. Closest Prime Numbers in Range: the pair of primes in
// [left, right] with the smallest difference, smallest first element winning ties,
// or [-1, -1] when the range holds fewer than two primes.
//
// Both strategies walk the range in ascending order and feed every prime they see
// to the same ClosestPairScan, so the tie rule is stated once; they differ only in
// how a candidate's primality is decided - O(sqrt(n)) trial division per candidate,
// or one Sieve of Eratosthenes over this repo's own DynamicArray<bool> composite
// tracker and an O(1) lookup per candidate afterwards. That is the same "share the
// walk, contrast the primality test" split MostFrequentPrime uses for LC 3044.
internal static class ClosestPrimeNumbersInRangeSolution
{
    private const int SmallestPrime = 2;

    // The textbook answer: test each candidate on its own by dividing it by every
    // odd divisor up to its square root. BCL-only, and it redoes that work for
    // every candidate in the range.
    public static int[] ClosestPrimesByTrialDivision(int left, int right)
    {
        var scan = new ClosestPairScan();

        for (var candidate = Math.Max(left, SmallestPrime); candidate <= right; candidate++)
        {
            if (IsPrimeByTrialDivision(candidate))
            {
                scan.Observe(candidate);
            }
        }

        return scan.Pair;
    }

    private static bool IsPrimeByTrialDivision(int value)
    {
        if (value < SmallestPrime)
        {
            return false;
        }

        if (value % SmallestPrime == 0)
        {
            return value == SmallestPrime;
        }

        // Evens are already settled above, so only odd divisors remain.
        for (var divisor = 3; (long)divisor * divisor <= value; divisor += 2)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }

    // One sieve up to `right` over this repo's own DynamicArray<bool>, the same
    // composition CountPrimes builds, so each candidate costs a single lookup and
    // the whole range is settled in O(right log log right).
    public static int[] ClosestPrimesBySieve(int left, int right)
    {
        var isComposite = SieveComposites(right);
        var scan = new ClosestPairScan();

        for (var candidate = Math.Max(left, SmallestPrime); candidate <= right; candidate++)
        {
            if (!isComposite.Get(candidate))
            {
                scan.Observe(candidate);
            }
        }

        return scan.Pair;
    }

    // Index i is "i is not prime", so 0 and 1 start out set and every multiple of a
    // surviving prime is struck from its square upwards.
    private static DynamicArray<bool> SieveComposites(int right)
    {
        var isComposite = new DynamicArray<bool>();

        for (var i = 0; i <= right; i++)
        {
            isComposite.Add(i < SmallestPrime);
        }

        for (var i = SmallestPrime; (long)i * i <= right; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= right; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        return isComposite;
    }

    // The closest-pair rule itself, shared by both strategies so neither can drift
    // from it: primes arrive in ascending order, and a later pair replaces the best
    // one only on a strictly smaller gap - which is what makes the smallest first
    // element win a tie.
    private struct ClosestPairScan()
    {
        private int _previousPrime = LeetCodeAnswer.None;
        private int _bestLow = LeetCodeAnswer.None;
        private int _bestHigh = LeetCodeAnswer.None;

        public readonly int[] Pair => [_bestLow, _bestHigh];

        public void Observe(int prime)
        {
            if (_previousPrime != LeetCodeAnswer.None && IsCloserThanBest(prime))
            {
                _bestLow = _previousPrime;
                _bestHigh = prime;
            }

            _previousPrime = prime;
        }

        private readonly bool IsCloserThanBest(int prime)
            => _bestLow == LeetCodeAnswer.None || prime - _previousPrime < _bestHigh - _bestLow;
    }
}
