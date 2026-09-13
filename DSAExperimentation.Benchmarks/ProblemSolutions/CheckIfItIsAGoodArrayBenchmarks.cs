using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckIfItIsAGoodArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfItIsAGoodArraySolution's, the same methods
// CheckIfItIsAGoodArrayTests proves correct. Bezout's identity reduces LC 1250 to
// "is the array's gcd 1", so the performance question left is how each pairwise
// gcd step is computed - repeated subtraction (O(max(a,b)/min(a,b)) per pair) vs.
// the modulo-based Euclidean algorithm (O(log min(a,b)) per pair). _values are all
// multiples of 3, guaranteeing the running gcd never reaches 1 early, so both arms
// are forced through every element instead of one short-circuiting on the first
// pair.
[MemoryDiagnoser]
public class CheckIfItIsAGoodArrayBenchmarks
{
    private const int RandomSeed = 6;
    private const int MultipleFactor = 3; // keeps _values multiples of 3 so the running gcd never reaches 1 early
    private const int MaxRandomValueExclusive = 50_000;

    [Params(50, 200)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => MultipleFactor * random.Next(1, MaxRandomValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool IsGoodArrayBySubtractionGcd() =>
        CheckIfItIsAGoodArraySolution.IsGoodArrayBySubtractionGcd(_values);

    [Benchmark]
    public bool IsGoodArrayByEuclideanGcd() =>
        CheckIfItIsAGoodArraySolution.IsGoodArrayByEuclideanGcd(_values);
}
