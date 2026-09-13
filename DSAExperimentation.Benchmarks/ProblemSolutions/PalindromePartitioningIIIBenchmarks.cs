using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PalindromePartitioningIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PalindromePartitioningIIISolution's, the same methods
// PalindromePartitioningIIITests proves correct. The workload is a random string
// over a small alphabet (so palindrome repairs are neither free nor uniformly
// expensive) split into half as many pieces as it has characters, which is where the
// two arms' shared decision tree is widest.
[MemoryDiagnoser]
public class PalindromePartitioningIIIBenchmarks
{
    private const int RandomSeed = 1278; // LC problem number
    private const int AlphabetSize = 4;
    private const int PartitionDivisor = 2;

    [Params(12, 18)]
    public int Length;

    private string _s = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
        _k = Length / PartitionDivisor;
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() => PalindromePartitioningIIISolution.MinChangesByNaiveRecursion(_s, _k);

    [Benchmark]
    public int MemoizedTopDown() => PalindromePartitioningIIISolution.MinChangesByMemoizedRecurrence(_s, _k);
}
