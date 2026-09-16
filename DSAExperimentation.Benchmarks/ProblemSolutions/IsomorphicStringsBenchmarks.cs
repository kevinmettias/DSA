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

    private string _source = "";

    private string _target = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        (_source, _target) = IsomorphicStringWorkloads.BuildIsomorphicPair(Length, Seed);

    [Benchmark(Baseline = true)]
    public bool IsIsomorphicByDictionary() => IsomorphicStringsSolution.IsIsomorphicByDictionary(_source, _target);

    [Benchmark]
    public bool IsIsomorphicByHashMap() => IsomorphicStringsSolution.IsIsomorphicByHashMap(_source, _target);
}
