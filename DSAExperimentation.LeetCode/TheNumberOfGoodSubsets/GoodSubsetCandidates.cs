namespace DSAExperimentation.LeetCode.TheNumberOfGoodSubsets;

// LC 1994's whole input preparation. Only a SQUAREFREE value in [2, 30] can ever
// appear in a good subset - any repeated prime factor (4 = 2^2, 12 = 2^2 * 3) makes
// every subset containing that value automatically bad - so the ~18 surviving values
// are reduced once to (prime mask over the 10 primes <= 30, how many times nums holds
// that value), and the search never looks at nums again. The count of 1s is carried
// alongside rather than folded in: 1 has no prime factors, so it never enters the
// recursion and instead doubles the final answer once per occurrence.
//
// It lives beside the solution rather than in Domain/ because "squarefree, over the
// primes below 30" fixes THIS problem's content and nothing else's (ARCHITECTURE
// section 17.3), and it is deliberately not an IEnumerable: it is the parameter type
// of the prepared-input overloads (section 17.4), so it must never be bindable by the
// int[]-shaped overloads the LeetCode-shaped calls go through.
internal sealed class GoodSubsetCandidates((int Mask, int Weight)[] candidates, int onesCount)
{
    private const int MinCandidateValue = 2;
    private const int MaxCandidateValue = 30;
    private const int ValueUpperBoundExclusive = MaxCandidateValue + 1;

    // 1 is the one value that is neither a candidate nor disqualifying.
    private const int OneValue = 1;

    private static readonly int[] PrimesUpToMaxCandidate = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29];

    // Fixed by the problem's own constraints, not by the input's length: every
    // squarefree value in [2, 30] is listed, with weight 0 when nums never holds it.
    public int Count => candidates.Length;

    // Read by name from both strategies' doubling step, so it stays a property over
    // the primary constructor's own value.
    public int OnesCount => onesCount;

    public (int Mask, int Weight) At(int index) => candidates[index];

    public static GoodSubsetCandidates Build(int[] nums)
    {
        var counts = new int[ValueUpperBoundExclusive];

        foreach (var num in nums)
        {
            counts[num]++;
        }

        return new GoodSubsetCandidates(BuildSquarefreeCandidates(counts), counts[OneValue]);
    }

    private static (int Mask, int Weight)[] BuildSquarefreeCandidates(int[] counts)
    {
        var candidates = new List<(int Mask, int Weight)>();

        for (var value = MinCandidateValue; value <= MaxCandidateValue; value++)
        {
            if (TryComputeSquarefreePrimeMask(value, out var mask))
            {
                candidates.Add((mask, counts[value]));
            }
        }

        return candidates.ToArray();
    }

    // One bit per prime in PrimesUpToMaxCandidate. Dividing the prime out once and
    // finding it still divides the remainder is exactly "this prime appears twice",
    // which disqualifies the value outright.
    private static bool TryComputeSquarefreePrimeMask(int value, out int mask)
    {
        mask = 0;

        for (var i = 0; i < PrimesUpToMaxCandidate.Length; i++)
        {
            if (!TryAbsorbPrime(ref value, ref mask, i))
            {
                return false;
            }
        }

        return true;
    }

    // Absorbs PrimesUpToMaxCandidate[primeIndex] into the running mask: a prime that
    // does not divide value sets no bit, and one that still divides the remainder
    // after being divided out once reports false, which is what disqualifies the
    // value as non-squarefree.
    private static bool TryAbsorbPrime(ref int value, ref int mask, int primeIndex)
    {
        var prime = PrimesUpToMaxCandidate[primeIndex];

        if (value % prime != 0)
        {
            return true;
        }

        value /= prime;

        if (value % prime == 0)
        {
            mask = 0;
            return false;
        }

        mask |= 1 << primeIndex;
        return true;
    }
}
