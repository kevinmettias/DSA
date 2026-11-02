using System.Numerics;
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
    // Every one of the nums.Length^sequenceLength sequences enumerated directly by
    // plain choose/recurse/unchoose recursion - one choice per sequence slot, the
    // textbook "Combination Sum"/"Generate Parentheses"-shaped exhaustive enumeration.
    // Correct at LeetCode's own tiny examples; the arm the digit DP below has to
    // justify itself against.
    public static int SumOfProductsByBacktrackEnumeration(int sequenceLength, int requiredSetBits, int[] nums)
    {
        var state = new SequenceState();
        var total = SumOverSequences(state, sequenceLength, requiredSetBits, nums);

        return (int)total;
    }

    // A carry-propagating digit DP: sweep bit positions low to high, choosing how
    // many of the sequenceLength slots land on each nums index (contributing that many
    // copies of 2^index), letting the running carry absorb what overflows into the
    // next bit - exactly binary addition with a multi-unit digit at each of the
    // first nums.Length positions, continued a few positions past nums.Length so the
    // last carry can fully drain. This repo's own Memoizer turns the recursion into a
    // DP over (bitPosition, slotsLeft, setBitsNeeded, carry); the weight for a count
    // split is "choose c of what's left" (the classic telescoping identity for
    // m!/(c_0!...c_{n-1}!)), read from this repo's own FactorialTable - the table
    // RoomWaysPrecomputedFactorialAlgebra reads for LC 1916.
    public static int SumOfProductsByCarryDigitDp(int sequenceLength, int requiredSetBits, int[] nums)
    {
        var maxBit = nums.Length + CarryDrainSteps(sequenceLength);
        var digits = (
            Nums: nums,
            Table: FactorialTable.Build(sequenceLength),
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

    // Past the last bit position the sweep only succeeded if every slot was placed,
    // every set bit asked for was produced, and nothing was left to carry.
    private static bool IsExactSolution(int remaining, int need, int carry)
        => remaining == 0 && carry == 0 && need == 0;

    // One node of the sequence tree, as the sum modulo 1e9+7 of what every completion of
    // it contributes: a complete sequence contributes its own matching product, a
    // shorter one appends each index in turn, recurses, and removes it again.
    private static long SumOverSequences(SequenceState state, int sequenceLength, int requiredSetBits, int[] nums)
    {
        if (state.Chosen.Count == sequenceLength)
        {
            return MatchingProduct(state, requiredSetBits, nums);
        }

        var total = 0L;

        for (var index = 0; index < nums.Length; index++)
        {
            AddChoice(state, index);
            var completions = SumOverSequences(state, sequenceLength, requiredSetBits, nums);
            total = (total + completions) % LeetCodeModulus.Value;
            RemoveChoice(state, index);
        }

        return total;
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
            product = product * nums[index] % LeetCodeModulus.Value;
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

    /// <summary>
    /// The recurrence, named: one DP step at bit position <c>Bit</c>, handing
    /// <c>Remaining</c> slots to that position's nums entry and letting the carry absorb
    /// what overflows into the next position - and, past the last position, succeeding
    /// only when every slot was placed, every set bit produced, and nothing left to carry.
    /// </summary>
    private sealed class CarryDigitSweep(
        (int[] Nums, FactorialTable Table, int MaxBit) digits)
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
                result = (result + CountTerm(state, count, rest)) % LeetCodeModulus.Value;
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

            var ways = digits.Table.Choose(remaining, count);
            var valuePower = bit < digits.Nums.Length ? ModularArithmetic.Power(digits.Nums[bit], count) : 1;
            var weight = ways * valuePower % LeetCodeModulus.Value;
            var sub = rest.Replay((bit + 1, remaining - count, nextNeed, total >> 1), rest);

            return weight * sub % LeetCodeModulus.Value;
        }
    }
}
