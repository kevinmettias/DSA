using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CamelcaseMatching;
using System.Text.RegularExpressions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CamelcaseMatchingSolution's, the same methods
// CamelcaseMatchingTests proves correct - the regex engine's backtracking state
// machine (baseline) against a direct two-pointer scan per query. [GlobalSetup] sizes
// and seeds the queries and compiles the matcher once, so the regex arm is charged
// for matching only, not for building its own pattern.
[MemoryDiagnoser]
public class CamelcaseMatchingBenchmarks
{
    private const string Pattern = "FB";

    // LC problem number, used as the deterministic seed for query generation.
    private const int RandomSeed = 1023;

    [Params(500, 10_000)]
    public int QueryCount;

    private string[] _queries = null!;
    private Regex _matcher = null!;

    [GlobalSetup]
    public void Setup()
    {
        _queries = CamelcaseQueryWorkloads.BuildMatchingQueries(Pattern, QueryCount, seed: RandomSeed);
        _matcher = CamelcaseMatchingSolution.BuildMatcher(Pattern);
    }

    [Benchmark(Baseline = true)]
    public bool[] RegexPerQuery() => CamelcaseMatchingSolution.CamelMatchByRegex(_queries, _matcher);

    [Benchmark]
    public bool[] TwoPointerPerQuery() =>
        CamelcaseMatchingSolution.CamelMatchByTwoPointerScan(_queries, Pattern);
}
