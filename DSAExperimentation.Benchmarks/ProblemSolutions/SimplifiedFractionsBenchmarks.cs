using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SimplifiedFractions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SimplifiedFractionsSolution's, the same methods
// SimplifiedFractionsTests proves correct. The naive/optimized split is within the
// one real algorithmic choice the problem has - trial division (O(min(a, b)) per
// pair) vs. the Euclidean algorithm (O(log min(a, b))).
//
// Deliberate change in what is measured (§17.8's precedent): both arms previously
// only counted coprime pairs to avoid materializing the result. They now return
// LeetCode's actual answer, so the formatted-string allocation is charged to both
// arms equally and the comparison is still about gcd cost.
[MemoryDiagnoser]
public class SimplifiedFractionsBenchmarks
{
    [Params(200, 2_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public List<string> TrialDivisionGcd() => SimplifiedFractionsSolution.ListFractionsByTrialDivisionGcd(N);

    [Benchmark]
    public List<string> EuclideanGcd() => SimplifiedFractionsSolution.ListFractionsByEuclideanGcd(N);
}
