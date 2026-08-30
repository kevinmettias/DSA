using BenchmarkDotNet.Attributes;
using System.Text;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Ambiguous Coordinates (LC 816): no repo primitive applies (same category as
// GasStation/Candy/MaximumProductSubarray) - both variants are pure string
// enumeration over the digit run, differing only in technique. The naive baseline
// materializes each candidate number via StringBuilder and re-scans it end to end
// (IndexOf the dot, then re-walk both sides) to validate leading/trailing zeros.
// The optimized version already knows the split point from the loop that produced
// it, so it slices directly and checks only the two boundary characters that can
// possibly violate the rule - no rebuilt string, no re-scan.
[MemoryDiagnoser]
public class AmbiguousCoordinatesBenchmarks
{
    [Params(8, 16)]
    public int Length;

    private string _digits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(816);
        var chars = new char[Length];
        chars[0] = (char)('1' + random.Next(9));

        for (var i = 1; i < Length; i++)
        {
            chars[i] = (char)('0' + random.Next(10));
        }

        _digits = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int RebuildAndRescan()
    {
        var count = 0;

        for (var split = 1; split < _digits.Length; split++)
        {
            var left = _digits[..split];
            var right = _digits[split..];

            foreach (var _ in RebuiltValidNumbers(left))
            {
                foreach (var __ in RebuiltValidNumbers(right))
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int SliceAndCheckBoundary()
    {
        var count = 0;

        for (var split = 1; split < _digits.Length; split++)
        {
            var left = _digits[..split];
            var right = _digits[split..];

            foreach (var _ in SlicedValidNumbers(left))
            {
                foreach (var __ in SlicedValidNumbers(right))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static IEnumerable<string> RebuiltValidNumbers(string digits)
    {
        if (IsValidWhole(digits))
        {
            yield return digits;
        }

        for (var dot = 1; dot < digits.Length; dot++)
        {
            var builder = new StringBuilder(digits.Length + 1);
            builder.Append(digits, 0, dot).Append('.').Append(digits, dot, digits.Length - dot);
            var candidate = builder.ToString();

            if (IsValidWithDot(candidate))
            {
                yield return candidate;
            }
        }
    }

    private static bool IsValidWhole(string digits) => digits == "0" || digits[0] != '0';

    private static bool IsValidWithDot(string candidate)
    {
        var dotIndex = candidate.IndexOf('.');
        var intPart = candidate[..dotIndex];
        var fracPart = candidate[(dotIndex + 1)..];
        return (intPart == "0" || intPart[0] != '0') && fracPart[^1] != '0';
    }

    private static IEnumerable<string> SlicedValidNumbers(string digits)
    {
        if (digits == "0" || digits[0] != '0')
        {
            yield return digits;
        }

        for (var dot = 1; dot < digits.Length; dot++)
        {
            var intPart = digits[..dot];
            var fracPart = digits[dot..];

            if ((intPart == "0" || intPart[0] != '0') && fracPart[^1] != '0')
            {
                yield return $"{intPart}.{fracPart}";
            }
        }
    }
}
