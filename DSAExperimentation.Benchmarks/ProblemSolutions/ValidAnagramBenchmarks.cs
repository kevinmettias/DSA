using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidAnagram;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ValidAnagramSolution's, the same methods
// ValidAnagramTests proves correct. t is a rotation of s (same multiset, different
// order) so both strategies are forced through their full comparison instead of an
// early mismatch cutting brute force short.
[MemoryDiagnoser]
public class ValidAnagramBenchmarks
{
    private const int AlphabetSize = 26;

    private string _s = "";

    private string _t = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var letters = Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray();
        _s = new string(letters);
        _t = new string([.. letters[1..], letters[0]]);
    }

    [Benchmark(Baseline = true)]
    public bool BruteForce() => ValidAnagramSolution.IsAnagramByBruteForce(_s, _t);

    [Benchmark]
    public bool HashMapFrequencyCount() => ValidAnagramSolution.IsAnagramByHashMapFrequencyCount(_s, _t);
}
