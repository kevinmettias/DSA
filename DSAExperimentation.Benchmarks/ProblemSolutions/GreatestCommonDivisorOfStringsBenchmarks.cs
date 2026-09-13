using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GreatestCommonDivisorOfStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GreatestCommonDivisorOfStringsSolution's, the same methods
// GreatestCommonDivisorOfStringsTests proves correct. The comparison is the textbook
// str1+str2 == str2+str1 identity (which materializes both concatenations, then runs a
// Euclidean gcd over the two lengths) against a single pass of this repo's own
// PrefixFunctionSearch.ComputeFailureFunction over str1+str2, whose failure-function tail
// yields the shared period without ever building the swapped concatenation. Both are O(n),
// so this is a technique comparison rather than an asymptotic-class split. The unit string
// is repeated a different, coprime number of times into each input so the two lengths are
// coprime multiples of a common divisor, forcing both strategies through their real work
// instead of a same-length shortcut.
[MemoryDiagnoser]
public class GreatestCommonDivisorOfStringsBenchmarks
{
    private const int LowercaseAlphabetSize = 26;
    private const int Str1RepeatCount = 13;
    private const int Str2RepeatCount = 7;

    [Params(20, 500)]
    public int UnitLength;

    private string _str1 = null!;
    private string _str2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var unit = new string(Enumerable.Range(0, UnitLength).Select(_ => (char)('a' + random.Next(LowercaseAlphabetSize))).ToArray());
        var str1Units = Enumerable.Repeat(unit, Str1RepeatCount);
        _str1 = string.Concat(str1Units);
        var str2Units = Enumerable.Repeat(unit, Str2RepeatCount);
        _str2 = string.Concat(str2Units);
    }

    [Benchmark(Baseline = true)]
    public string ConcatenationEqualityCheck() =>
        GreatestCommonDivisorOfStringsSolution.GcdOfStringsByConcatenationEquality(_str1, _str2);

    [Benchmark]
    public string PrefixFunctionPeriod() =>
        GreatestCommonDivisorOfStringsSolution.GcdOfStringsByPrefixFunctionPeriod(_str1, _str2);
}
