using System.Text;

using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check If a String Contains All Binary Codes of Size K (LC 1461): the textbook
// "search for each of the 2^k codes" brute force (string.Contains per code) vs.
// one O(text.Length) sliding-bitmask pass recording each window in this repo's
// own Set<int> (HashMap-backed, DataStructures/Set/Set.cs). _text is built by
// concatenating every length-k code once, in order - not random data - so it
// deterministically contains ALL 2^k codes: BruteForce is forced through its
// full worst-case 2^k searches (no early "missing code" exit) on every
// invocation, the same "force the full scan on both sides" intent
// TwoSumBenchmarks' unreachable target already establishes for that benchmark.
[MemoryDiagnoser]
public class CheckIfAStringContainsAllBinaryCodesOfSizeKBenchmarks
{
    [Params(8, 10)]
    public int K;

    private string _text = null!;

    [GlobalSetup]
    public void Setup() => _text = BuildCoveringText(K);

    [Benchmark(Baseline = true)]
    public bool BruteForceSubstringSearch()
    {
        var total = 1 << K;

        for (var code = 0; code < total; code++)
        {
            var binaryCode = ToBinaryString(code, K);
            if (!_text.Contains(binaryCode, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool SlidingBitmaskWithSet()
    {
        var total = 1 << K;
        var mask = total - 1;
        var seen = new Set<int>();
        var code = 0;

        for (var i = 0; i < _text.Length; i++)
        {
            code = ((code << 1) | (_text[i] - '0')) & mask;

            if (i >= K - 1)
            {
                seen.TryAdd(code);
            }
        }

        return seen.Count == total;
    }

    private static string BuildCoveringText(int k)
    {
        var total = 1 << k;
        var builder = new StringBuilder(total * k);

        for (var code = 0; code < total; code++)
        {
            var binaryCode = ToBinaryString(code, k);
            builder.Append(binaryCode);
        }

        return builder.ToString();
    }

    private static string ToBinaryString(int code, int k)
    {
        var chars = new char[k];

        for (var i = k - 1; i >= 0; i--)
        {
            chars[i] = (char)('0' + (code & 1));
            code >>= 1;
        }

        return new string(chars);
    }
}
