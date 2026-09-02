using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Operations to Make String Sorted (LC 1830): a baseline that
// counts, at every position, how many not-yet-placed letters are smaller than the
// current one via a linear O(26) scan of the frequency array, vs. this repo's own
// FenwickTree<int, SumOperation<int>> (a Binary Indexed Tree over the 26-letter
// alphabet) answering the same "how many remaining letters are smaller" question via
// PrefixQuery - the same Fenwick-vs-linear-scan shape
// CreateSortedArrayThroughInstructionsBenchmarks already uses, here over a
// fixed-size alphabet instead of a value-compressed range. Both accumulate the same
// modular permutation-rank sum via an incrementally-updated inverse-factorial
// product, so the only thing genuinely being compared is the "smaller count" query
// itself.
[MemoryDiagnoser]
public class MinimumNumberOfOperationsToMakeStringSortedBenchmarks
{
    private const long Mod = 1_000_000_007;
    private const int RandomSeed = 1830; // LC problem number
    private const int AlphabetSize = 26;
    private const long FermatLittleTheoremOffset = 2; // a^(Mod-2) === a^-1 (mod Mod) since Mod is prime

    [Params(200, 2_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public long LinearFrequencyScan()
    {
        var (fact, invFact) = BuildFactorials(_s.Length);
        var freq = BuildFrequencyArray(_s);
        var state = new ScanState { InvFactProduct = ComputeInvFactProduct(freq, invFact), Remaining = _s.Length };
        var ans = 0L;

        foreach (var ch in _s)
        {
            ans = (ans + AccumulateLinearScanContribution(ch, freq, fact, ref state)) % Mod;
        }

        return ans;
    }

    [Benchmark]
    public long FenwickTreeSweep()
    {
        var (fact, invFact) = BuildFactorials(_s.Length);
        var freq = BuildFrequencyArray(_s);
        var counts = new FenwickTree<int, SumOperation<int>>(freq);
        var state = new ScanState { InvFactProduct = ComputeInvFactProduct(freq, invFact), Remaining = _s.Length };
        var ans = 0L;

        foreach (var ch in _s)
        {
            ans = (ans + AccumulateFenwickScanContribution(ch, new FrequencyCounter(freq, counts), fact, ref state)) % Mod;
        }

        return ans;
    }

    private static int[] BuildFrequencyArray(string s)
    {
        var freq = new int[AlphabetSize];

        foreach (var ch in s)
        {
            freq[ch - 'a']++;
        }

        return freq;
    }

    private static long ComputeInvFactProduct(int[] freq, long[] invFact)
    {
        var invFactProduct = 1L;

        foreach (var count in freq)
        {
            invFactProduct = invFactProduct * invFact[count] % Mod;
        }

        return invFactProduct;
    }

    private static long AccumulateLinearScanContribution(char ch, int[] freq, long[] fact, ref ScanState state)
    {
        var c = ch - 'a';
        var smaller = 0;

        for (var d = 0; d < c; d++)
        {
            smaller += freq[d];
        }

        var contribution = smaller > 0
            ? (long)smaller * fact[state.Remaining - 1] % Mod * state.InvFactProduct % Mod
            : 0L;

        state.InvFactProduct = state.InvFactProduct * freq[c] % Mod;
        freq[c]--;
        state.Remaining--;

        return contribution;
    }

    private static long AccumulateFenwickScanContribution(char ch, FrequencyCounter counter, long[] fact, ref ScanState state)
    {
        var c = ch - 'a';
        var smaller = c == 0 ? 0 : counter.Counts.PrefixQuery(c - 1);

        var contribution = smaller > 0
            ? (long)smaller * fact[state.Remaining - 1] % Mod * state.InvFactProduct % Mod
            : 0L;

        state.InvFactProduct = state.InvFactProduct * counter.Freq[c] % Mod;
        counter.Freq[c]--;
        counter.Counts.Add(c, -1);
        state.Remaining--;

        return contribution;
    }

    private static (long[] Fact, long[] InvFact) BuildFactorials(int n)
    {
        var fact = new long[n + 1];
        var invFact = new long[n + 1];
        fact[0] = 1;

        for (var i = 1; i <= n; i++)
        {
            fact[i] = fact[i - 1] * i % Mod;
        }

        invFact[n] = ModPow(fact[n], Mod - FermatLittleTheoremOffset);

        for (var i = n; i > 0; i--)
        {
            invFact[i - 1] = invFact[i] * i % Mod;
        }

        return (fact, invFact);
    }

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

    private struct ScanState
    {
        public long InvFactProduct;
        public int Remaining;
    }

    private readonly record struct FrequencyCounter(int[] Freq, FenwickTree<int, SumOperation<int>> Counts);
}
