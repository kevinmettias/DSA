using DSAExperimentation.LeetCode.LongestCommonPrefix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestCommonPrefixSolution's, the same methods
// LongestCommonPrefixSolutionTests proves correct. Linear shrink-and-compare vs.
// BinarySearch over the monotone predicate "prefix length n is shared by
// every string". Each string is the shared prefix plus one diverging letter, so
// PrefixLength stops at 199: strings of 200 characters, LC 14's cap.
public class LongestCommonPrefixBenchmarks
{
    private static readonly string[] DivergingSuffixes = ["a", "b", "c", "d"];

    private string[] _values = [];

    [Params(64, 199)]
    public int PrefixLength { get; set; }

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
