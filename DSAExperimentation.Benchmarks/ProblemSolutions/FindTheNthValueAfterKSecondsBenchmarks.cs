using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheNthValueAfterKSeconds;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheNthValueAfterKSecondsSolution's, the same
// methods FindTheNthValueAfterKSecondsTests proves correct - literal O(n*k)
// simulation vs. the closed-form modular binomial coefficient. n and k grow
// together via the same [Params] axis so the O(n*k) arm's quadratic-ish blowup
// against the closed form's near-linear one shows up in the ratio.
[MemoryDiagnoser]
public class FindTheNthValueAfterKSecondsBenchmarks
{
    [Params(50, 300)]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public int BruteForce() => FindTheNthValueAfterKSecondsSolution.ValueAfterKSecondsByBruteForce(Size, Size);

    [Benchmark]
    public int ModularBinomial() => FindTheNthValueAfterKSecondsSolution.ValueAfterKSecondsByModularBinomial(Size, Size);
}
