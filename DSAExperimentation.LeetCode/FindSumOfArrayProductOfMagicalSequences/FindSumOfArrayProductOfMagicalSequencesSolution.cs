using System.Numerics;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindSumOfArrayProductOfMagicalSequences;

// LeetCode 3539. Find Sum of Array Product of Magical Sequences: sum, over every
// length-m sequence of indices into nums whose selected powers of two
// (2^seq[0] + ... + 2^seq[m-1]) add up to a binary value with exactly k set bits,
// of the product nums[seq[0]] * ... * nums[seq[m-1]], modulo 1e9+7.
//
// A sequence is fully described by how many times each index 0..n-1 is chosen
// (c_0, ..., c_{n-1} summing to m): every ordering of the same multiset contributes
// the same product, and there are m! / (c_0! * ... * c_{n-1}!) such orderings. Both
// strategies exploit that - one by enumerating orderings directly and letting equal
// products naturally sum together, the other by computing the multinomial weight
// per count-assignment instead of visiting every ordering.
internal static class FindSumOfArrayProductOfMagicalSequencesSolution
{
    // Every one of the n^m sequences enumerated directly via this repo's own
    // Backtrack.Search - one choice per sequence slot, exactly the "Combination
    // Sum"/"Generate Parentheses"-shaped exhaustive enumeration its own doc comment
    // names as the intended use. Correct at LeetCode's own tiny examples; the arm
    // the digit DP below has to justify itself against.
    public static int SumOfProductsByBacktrackEnumeration(int m, int k, int[] nums)
    {
        var total = 0L;
        var state = new SequenceState();

        Backtrack.Search<SequenceState, int>(
            state,
            isSolution: s => s.Chosen.Count == m,
            candidates: s => s.Chosen.Count == m ? Array.Empty<int>() : Enumerable.Range(0, nums.Length),
            choose: (s, index) =>
            {
                s.Chosen.Add(index);
                s.Sum += 1L << index;
            },
            unchoose: (s, index) =>
            {
                s.Chosen.RemoveAt(s.Chosen.Count - 1);
                s.Sum -= 1L << index;
            },
            onSolution: s =>
            {
                if (BitOperations.PopCount((ulong)s.Sum) != k)
                {
                    return;
                }

                var product = 1L;

                foreach (var index in s.Chosen)
                {
                    product = product * nums[index] % ModularArithmetic.Modulo;
                }

                total = (total + product) % ModularArithmetic.Modulo;
            });

        return (int)total;
    }

    private sealed class SequenceState
    {
        public List<int> Chosen { get; } = [];

        public long Sum { get; set; }
    }

    // A carry-propagating digit DP: sweep bit positions low to high, choosing how
    // many of the m remaining slots land on each nums index (contributing that many
    // copies of 2^index), letting the running carry absorb what overflows into the
    // next bit - exactly binary addition with a multi-unit digit at each of the
    // first n positions, continued a few positions past n so the last carry can
    // fully drain. This repo's own Memoizer turns the recursion into a DP over
    // (bitPosition, slotsLeft, setBitsNeeded, carry); the weight for a count split
    // is "choose c of what's left" (the classic telescoping identity for
    // m!/(c_0!...c_{n-1}!)), read from a factorial table built with this repo's own
    // ModularArithmetic.Inverse - the same technique RoomWaysPrecomputedFactorialAlgebra
    // uses for LC 1916, but this problem's own since no other solution shares it.
    public static int SumOfProductsByCarryDigitDp(int m, int k, int[] nums)
    {
        var n = nums.Length;
        var maxBit = n + CarryDrainSteps(m);
        var factorial = BuildFactorial(m);
        var inverseFactorial = BuildInverseFactorial(factorial);

        long Recurrence((int Bit, int Remaining, int Need, int Carry) state, Func<(int, int, int, int), long> recurse)
        {
            var (bit, remaining, need, carry) = state;

            if (bit == maxBit)
            {
                return remaining == 0 && carry == 0 && need == 0 ? 1 : 0;
            }

            var maxCount = bit < n ? remaining : 0;
            var result = 0L;

            for (var count = 0; count <= maxCount; count++)
            {
                var total = carry + count;
                var nextNeed = need - (total & 1);

                if (nextNeed < 0)
                {
                    continue;
                }

                var ways = Choose(factorial, inverseFactorial, remaining, count);
                var valuePower = bit < n ? ModularArithmetic.Power(nums[bit], count) : 1;
                var weight = ways * valuePower % ModularArithmetic.Modulo;
                var sub = recurse((bit + 1, remaining - count, nextNeed, total >> 1));

                result = (result + weight * sub) % ModularArithmetic.Modulo;
            }

            return result;
        }

        var total = Memoizer.Memoize<(int, int, int, int), long>((0, m, k, 0), Recurrence);
        return (int)total;
    }

    // How many pure-halving steps a carry as large as m needs to reach zero once no
    // further slots feed it - m <= 30 drains in at most 5, so a little slack keeps
    // the bound honest without inflating the state space.
    private static int CarryDrainSteps(int m)
    {
        var steps = 1;

        while (m > 0)
        {
            m >>= 1;
            steps++;
        }

        return steps;
    }

    private static long[] BuildFactorial(int upTo)
    {
        var factorial = new long[upTo + 1];
        factorial[0] = 1;

        for (var i = 1; i <= upTo; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        return factorial;
    }

    private static long[] BuildInverseFactorial(long[] factorial)
    {
        var inverseFactorial = new long[factorial.Length];

        for (var i = 0; i < factorial.Length; i++)
        {
            inverseFactorial[i] = ModularArithmetic.Inverse(factorial[i]);
        }

        return inverseFactorial;
    }

    private static long Choose(long[] factorial, long[] inverseFactorial, int n, int r) =>
        factorial[n] * inverseFactorial[r] % ModularArithmetic.Modulo * inverseFactorial[n - r] % ModularArithmetic.Modulo;
}
