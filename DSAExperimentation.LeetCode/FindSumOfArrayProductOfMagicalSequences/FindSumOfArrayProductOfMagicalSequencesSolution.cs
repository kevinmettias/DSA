using System.Numerics;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindSumOfArrayProductOfMagicalSequences;

// LeetCode 3539. Find Sum of Array Product of Magical Sequences: sum, over every
// sequence of sequenceLength indices into nums whose selected powers of two
// (2^seq[0] + ... + 2^seq[m-1]) add up to a binary value whose pop count is exactly
// requiredSetBits, of the product nums[seq[0]] * ... * nums[seq[m-1]], modulo 1e9+7.
//
// A sequence is fully described by how many times each index 0..n-1 is chosen
// (c_0, ..., c_{n-1} summing to sequenceLength): every ordering of the same multiset
// contributes the same product, and there are m! / (c_0! * ... * c_{n-1}!) such
// orderings. Both strategies exploit that - one by enumerating orderings directly and
// letting equal products naturally sum together, the other by computing the
// multinomial weight per count-assignment instead of visiting every ordering.
internal static class FindSumOfArrayProductOfMagicalSequencesSolution
{
    // Every one of the nums.Length^sequenceLength sequences enumerated directly via
    // this repo's own Backtrack.Search - one choice per sequence slot, exactly the
    // "Combination Sum"/"Generate Parentheses"-shaped exhaustive enumeration its own
    // doc comment names as the intended use. Correct at LeetCode's own tiny examples;
    // the arm the digit DP below has to justify itself against.
    public static int SumOfProductsByBacktrackEnumeration(int sequenceLength, int requiredSetBits, int[] nums)
    {
        var total = 0L;
        var state = new SequenceState();

        Backtrack.Search<SequenceState, int>(
            state,
            isSolution: s => s.Chosen.Count == sequenceLength,
            candidates: s => s.Chosen.Count == sequenceLength ? Array.Empty<int>() : Enumerable.Range(0, nums.Length),
            choose: AddChoice,
            unchoose: RemoveChoice,
            onSolution: s => total = (total + MatchingProduct(s, requiredSetBits, nums)) % ModularArithmetic.Modulo);

        return (int)total;
    }

    // What one completed sequence contributes: the product of the values it chose,
    // or zero when its binary value does not have exactly requiredSetBits set bits. A
    // non-matching sequence contributes zero, which leaves the running total unchanged -
    // the same outcome as the guard that used to skip it.
    private static long MatchingProduct(SequenceState state, int requiredSetBits, int[] nums)
    {
        if (BitOperations.PopCount((ulong)state.Sum) != requiredSetBits)
        {
            return 0;
        }

        var product = 1L;

        foreach (var index in state.Chosen)
        {
            product = product * nums[index] % ModularArithmetic.Modulo;
        }

        return product;
    }

    // The search's own choose/unchoose pair: adding the index at the end of the
    // sequence contributes 2^index to the running binary value, so undoing it
    // subtracts exactly what it added.
    private static void AddChoice(SequenceState state, int index)
    {
        state.Chosen.Add(index);
        state.Sum += 1L << index;
    }

    private static void RemoveChoice(SequenceState state, int index)
    {
        state.Chosen.RemoveAt(state.Chosen.Count - 1);
        state.Sum -= 1L << index;
    }

    private sealed class SequenceState
    {
        public List<int> Chosen { get; } = [];

        public long Sum { get; set; }
    }

    // A carry-propagating digit DP: sweep bit positions low to high, choosing how
    // many of the sequenceLength slots land on each nums index (contributing that many
    // copies of 2^index), letting the running carry absorb what overflows into the
    // next bit - exactly binary addition with a multi-unit digit at each of the
    // first nums.Length positions, continued a few positions past nums.Length so the
    // last carry can fully drain. This repo's own Memoizer turns the recursion into a
    // DP over (bitPosition, slotsLeft, setBitsNeeded, carry); the weight for a count
    // split is "choose c of what's left" (the classic telescoping identity for
    // m!/(c_0!...c_{n-1}!)), read from a factorial table built with this repo's own
    // ModularArithmetic.Inverse - the same technique RoomWaysPrecomputedFactorialAlgebra
    // uses for LC 1916, but this problem's own since no other solution shares it.
    public static int SumOfProductsByCarryDigitDp(int sequenceLength, int requiredSetBits, int[] nums)
    {
        var maxBit = nums.Length + CarryDrainSteps(sequenceLength);
        var factorial = BuildFactorial(sequenceLength);
        var digits = (
            Nums: nums,
            Factorial: factorial,
            InverseFactorial: BuildInverseFactorial(factorial),
            MaxBit: maxBit);

        var total = Memoizer.Memoize<(int, int, int, int), long>(
            (0, sequenceLength, requiredSetBits, 0), new CarryDigitSweep(digits));

        return (int)total;
    }

    // How many pure-halving steps a carry as large as maxCarry needs to reach zero once
    // no further slots feed it - a sequence of at most 30 slots drains in at most 5, so
    // a little slack keeps the bound honest without inflating the state space.
    private static int CarryDrainSteps(int maxCarry)
    {
        var steps = 1;

        while (maxCarry > 0)
        {
            maxCarry >>= 1;
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

    // Past the last bit position the sweep only succeeded if every slot was placed,
    // every set bit asked for was produced, and nothing was left to carry.
    private static bool IsExactSolution(int remaining, int need, int carry)
        => remaining == 0 && carry == 0 && need == 0;

    /// <summary>
    /// The recurrence, named: one DP step at bit position <c>Bit</c>, handing
    /// <c>Remaining</c> slots to that position's nums entry and letting the carry absorb
    /// what overflows into the next position - and, past the last position, succeeding
    /// only when every slot was placed, every set bit produced, and nothing left to carry.
    /// </summary>
    private sealed class CarryDigitSweep(
        (int[] Nums, long[] Factorial, long[] InverseFactorial, int MaxBit) digits)
        : IRecurrence<(int Bit, int Remaining, int Need, int Carry), long>
    {
        /// <inheritdoc/>
        public long Replay(
            (int Bit, int Remaining, int Need, int Carry) state,
            IRecurrence<(int Bit, int Remaining, int Need, int Carry), long> rest)
        {
            var (bit, remaining, need, carry) = state;

            if (bit == digits.MaxBit)
            {
                return IsExactSolution(remaining, need, carry) ? 1 : 0;
            }

            var maxCount = bit < digits.Nums.Length ? remaining : 0;
            var result = 0L;

            for (var count = 0; count <= maxCount; count++)
            {
                result = (result + CountTerm(state, count, rest)) % ModularArithmetic.Modulo;
            }

            return result;
        }

        // What placing `count` further slots at nums[bit] is worth: the multinomial
        // weight of that split times nums[bit]^count, times the sub-count below - or
        // nothing at all when the carry would push the set-bit count past requiredSetBits.
        private long CountTerm(
            (int Bit, int Remaining, int Need, int Carry) state,
            int count,
            IRecurrence<(int Bit, int Remaining, int Need, int Carry), long> rest)
        {
            var (bit, remaining, need, carry) = state;
            var total = carry + count;
            var nextNeed = need - (total & 1);

            if (nextNeed < 0)
            {
                return 0;
            }

            var ways = Choose(digits.Factorial, digits.InverseFactorial, remaining, count);
            var valuePower = bit < digits.Nums.Length ? ModularArithmetic.Power(digits.Nums[bit], count) : 1;
            var weight = ways * valuePower % ModularArithmetic.Modulo;
            var sub = rest.Replay((bit + 1, remaining - count, nextNeed, total >> 1), rest);

            return weight * sub % ModularArithmetic.Modulo;
        }
    }

    private static long Choose(long[] factorial, long[] inverseFactorial, int itemCount, int selectedCount) =>
        factorial[itemCount] * inverseFactorial[selectedCount] % ModularArithmetic.Modulo
        * inverseFactorial[itemCount - selectedCount] % ModularArithmetic.Modulo;
}
