using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestCommonPrefix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestCommonPrefixSolution's, the same methods
// LongestCommonPrefixTests proves correct. Linear shrink-and-compare vs.
// BinarySearch over the monotone predicate "prefix length n is shared by
// every string".
[MemoryDiagnoser]
public class LongestCommonPrefixBenchmarks
{
    private static readonly string[] DivergingSuffixes = ["a", "b", "c", "d"];

    private string[] _values = null!;

    [Params(64, 512)]
    public int PrefixLength;

    [GlobalSetup]
    public void Setup()
    {
        var prefix = new string('x', PrefixLength);
        _values = DivergingSuffixes.Select(suffix => prefix + suffix).ToArray();
    }

    [Benchmark(Baseline = true)]
    public string LinearScan() => LongestCommonPrefixSolution.PrefixByLinearScan(_values);

    [Benchmark]
    public string BinarySearchPredicate() => LongestCommonPrefixSolution.PrefixByBinarySearch(_values);
}
