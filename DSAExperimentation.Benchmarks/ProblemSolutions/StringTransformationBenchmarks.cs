using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// String Transformation (LC 2851): both arms share the same closed-form modular
// combine step (O(log k) regardless of k, so k stays a large fixed constant here
// rather than a varying axis) - the isolated variable is how the single rotation
// -match count it needs gets computed: an O(n^2) brute-force window compare vs.
// this repo's O(n) ZFunction.FindAll, the same StringTransformationTests precedent.
[MemoryDiagnoser]
public class StringTransformationBenchmarks
{
    private const long Mod = 1_000_000_007;
    private const long K = 1_000_000_000_007;

    [Params(200, 2_000)]
    public int Length;

    private string _s = null!;
    private string _t = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var chars = Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(26))).ToArray();
        _s = new string(chars);
        // A genuine rotation of s (not s itself), so both strategies find a real,
        // non-trivial match instead of the degenerate all-zero R=0 case.
        _t = _s.Substring(Length / 2) + _s.Substring(0, Length / 2);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRotationCompare()
    {
        var relevantCount = CountRotationMatchesByBruteForce(_s, _t);
        return Combine(_s.Length, relevantCount, sameString: false, K);
    }

    [Benchmark]
    public int ZFunctionSearch()
    {
        var relevantCount = CountRotationMatchesByZFunction(_s, _t);
        return Combine(_s.Length, relevantCount, sameString: false, K);
    }

    private static int CountRotationMatchesByBruteForce(string s, string pattern)
    {
        var n = s.Length;
        var count = 0;

        for (var start = 0; start < n; start++)
        {
            var matches = true;

            for (var offset = 0; offset < n; offset++)
            {
                if (s[(start + offset) % n] != pattern[offset])
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                count++;
            }
        }

        return count;
    }

    private static int CountRotationMatchesByZFunction(string s, string pattern)
    {
        var text = s + s.Substring(0, s.Length - 1);
        return ZFunction.FindAll(text, pattern).Count;
    }

    private static int Combine(int n, int relevantCount, bool sameString, long k)
    {
        var sign = k % 2 == 0 ? 1L : Mod - 1;
        var pow = ModPow(n - 1, k, Mod);
        var inverseN = ModPow(n, Mod - 2, Mod);
        var g = ((pow - sign) % Mod + Mod) % Mod * inverseN % Mod;

        if (!sameString)
        {
            return (int)((long)relevantCount * g % Mod);
        }

        var f = (g + sign) % Mod;
        var extra = (long)(relevantCount - 1) * g % Mod;
        return (int)((f + extra) % Mod);
    }

    private static long ModPow(long value, long exponent, long modulus)
    {
        value %= modulus;
        var result = 1L;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result * value % modulus;
            }

            value = value * value % modulus;
            exponent >>= 1;
        }

        return result;
    }
}
