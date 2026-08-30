using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Greatest Common Divisor of Strings (LC 1071): the textbook str1+str2==str2+str1
// concatenation-equality check (building both concatenations up front, then an
// Euclidean gcd over the two lengths) vs. a single pass of this repo's own
// PrefixFunctionSearch.ComputeFailureFunction over str1+str2, whose failure-function
// tail directly yields the shared period without ever materializing the swapped
// concatenation str2+str1. Both are O(n) - this is a genuine technique difference
// (algebraic identity vs. failure-function period), not an asymptotic-class split,
// the same "constant-factor/technique comparison" shape this repo's benchmarks
// already use when two correct approaches share a complexity class. _unit is
// repeated a different number of times into each string so the two lengths are
// coprime multiples of a common divisor, forcing both strategies through their real
// matching work instead of a same-length shortcut.
[MemoryDiagnoser]
public class GreatestCommonDivisorOfStringsBenchmarks
{
    [Params(20, 500)]
    public int UnitLength;

    private string _str1 = null!;
    private string _str2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var unit = new string(Enumerable.Range(0, UnitLength).Select(_ => (char)('a' + random.Next(26))).ToArray());
        _str1 = string.Concat(Enumerable.Repeat(unit, 13));
        _str2 = string.Concat(Enumerable.Repeat(unit, 7));
    }

    [Benchmark(Baseline = true)]
    public string ConcatenationEqualityCheck()
    {
        if (_str1 + _str2 != _str2 + _str1)
        {
            return string.Empty;
        }

        var length = Gcd(_str1.Length, _str2.Length);
        return _str1[..length];
    }

    [Benchmark]
    public string PrefixFunctionPeriod()
    {
        var concatenated = _str1 + _str2;
        var failure = PrefixFunctionSearch.ComputeFailureFunction(concatenated);
        var period = concatenated.Length - failure[^1];

        if (_str1.Length % period != 0 || _str2.Length % period != 0)
        {
            return string.Empty;
        }

        return _str1[..period];
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
