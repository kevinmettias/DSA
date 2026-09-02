using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfOperationsToMakeStringSorted;

// LeetCode 1830. Minimum Number of Operations to Make String Sorted: each described
// operation is exactly the "previous permutation" step, so the answer is s's rank
// (0-indexed, mod 1e9+7) among the distinct permutations of its own multiset of
// characters, sorted ascending. Sweeping left to right, a FenwickTree<int,
// SumOperation<int>> (this repo's own Binary Indexed Tree, initialized from the
// 26-letter frequency array) gives, at every position, how many not-yet-placed
// letters are smaller than the current one via PrefixQuery - the same
// value-indexed-Fenwick shape MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits
// already uses, generalized from digits to letters. Each smaller letter contributes
// (remaining-1)! / prod(remainingFreq!) permutations, folded via a running modular
// inverse-factorial product (updated with one multiplication per step, no
// recomputation) and modular exponentiation for the initial inverse - the same
// inline ModPow-by-squaring shape SuperPowTests already uses, since this repo has no
// dedicated modular-arithmetic primitive and none is needed beyond a few running
// scalars.
public sealed partial class MinimumNumberOfOperationsToMakeStringSortedTests
{
    private const long Mod = 1_000_000_007;

    [Theory]
    [InlineData("cba", 5)]
    [InlineData("aabaa", 2)]
    [InlineData("zyx", 5)]
    public void MakeStringSorted_LeetCodeExamples_ReturnsPermutationRank(string s, int expected)
        => Assert.Equal(expected, MakeStringSorted(s));

    [Fact]
    public void MakeStringSorted_AlreadySorted_ReturnsZero()
        => Assert.Equal(0, MakeStringSorted("abc"));

    [Fact]
    public void MakeStringSorted_SingleCharacter_ReturnsZero()
        => Assert.Equal(0, MakeStringSorted("q"));

    private static int MakeStringSorted(string s)
    {
        var n = s.Length;
        var (fact, invFact) = BuildFactorialTables(n);
        var freq = BuildFrequencyTable(s);
        var counts = new FenwickTree<int, SumOperation<int>>(freq);
        var invFactProduct = ComputeInitialInverseFactorialProduct(freq, invFact);

        var context = new RankAccumulatorContext(freq, fact, counts);
        var state = new RankState(0L, invFactProduct, n);

        foreach (var ch in s)
        {
            state = AccumulateRank(ch, state, context);
        }

        return (int)state.Ans;
    }

    private static (long[] Fact, long[] InvFact) BuildFactorialTables(int n)
    {
        var fact = new long[n + 1];
        var invFact = new long[n + 1];
        fact[0] = 1;

        for (var i = 1; i <= n; i++)
        {
            fact[i] = fact[i - 1] * i % Mod;
        }

        invFact[n] = ModPow(fact[n], Mod - 2);

        for (var i = n; i > 0; i--)
        {
            invFact[i - 1] = invFact[i] * i % Mod;
        }

        return (fact, invFact);
    }

    private static int[] BuildFrequencyTable(string s)
    {
        var freq = new int[26];

        foreach (var ch in s)
        {
            freq[ch - 'a']++;
        }

        return freq;
    }

    private static long ComputeInitialInverseFactorialProduct(int[] freq, long[] invFact)
    {
        var invFactProduct = 1L;

        foreach (var count in freq)
        {
            invFactProduct = invFactProduct * invFact[count] % Mod;
        }

        return invFactProduct;
    }

    private static RankState AccumulateRank(char ch, RankState state, RankAccumulatorContext context)
    {
        var c = ch - 'a';
        var smaller = c == 0 ? 0 : context.Counts.PrefixQuery(c - 1);
        var ans = state.Ans;

        if (smaller > 0)
        {
            ans = (ans + ((long)smaller * context.Fact[state.Remaining - 1] % Mod * state.InvFactProduct)) % Mod;
        }

        var invFactProduct = state.InvFactProduct * context.Freq[c] % Mod;
        context.Freq[c]--;
        context.Counts.Add(c, -1);

        return new RankState(ans, invFactProduct, state.Remaining - 1);
    }

    private readonly record struct RankAccumulatorContext(int[] Freq, long[] Fact, FenwickTree<int, SumOperation<int>> Counts);

    private readonly record struct RankState(long Ans, long InvFactProduct, int Remaining);

    private static long ModPow(long value, long exponent)
    {
        var result = 1L;
        value %= Mod;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % Mod;
            }

            value = value * value % Mod;
            exponent >>= 1;
        }

        return result;
    }
}
