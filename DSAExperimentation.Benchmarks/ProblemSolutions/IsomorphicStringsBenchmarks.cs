using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.IsomorphicStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IsomorphicStringsSolution's, the same methods
// IsomorphicStringsTests proves correct. The workload is a guaranteed-
// isomorphic pair (a random substitution cipher applied to the source) so
// neither strategy exits early on a mismatch.
[MemoryDiagnoser]
public class IsomorphicStringsBenchmarks
{
    private const int Seed = 205;

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private string _t = null!;

    [GlobalSetup]
    public void Setup() => (_s, _t) = IsomorphicStringWorkloads.BuildIsomorphicPair(Length, Seed);

    [Benchmark(Baseline = true)]
    public bool Dictionary() => IsomorphicStringsSolution.IsIsomorphicByDictionary(_s, _t);

    [Benchmark]
    public bool HashMap() => IsomorphicStringsSolution.IsIsomorphicByHashMap(_s, _t);
}
