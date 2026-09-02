using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CreateGridWithExactlyKPathsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CreateGridWithExactlyKPathsISolution's, the same
// methods CreateGridWithExactlyKPathsITests proves correct. k = 4 forces the
// widest rectangle search (up to a 2x4/4x2 candidate, or the 3x3 double-chain
// fallback), so it is the Params value that makes the two path-count strategies'
// per-candidate cost actually show up.
[MemoryDiagnoser]
public class CreateGridWithExactlyKPathsIBenchmarks
{
    private const int K = 4;

    [Params(4, 10)]
    public int Side;

    [Benchmark(Baseline = true)]
    public string[] PathCountDp() => CreateGridWithExactlyKPathsISolution.CreateGridByPathCountDp(Side, Side, K);

    [Benchmark]
    public string[] BinomialFormula() => CreateGridWithExactlyKPathsISolution.CreateGridByBinomialFormula(Side, Side, K);
}
