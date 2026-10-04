using DSAExperimentation.LeetCode.FirstBadVersion;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FirstBadVersionSolution's, the same methods
// FirstBadVersionSolutionTests proves correct. FirstBad sits at 70% of VersionCount so the
// linear scan pays close to its full O(n) worst case every call.
public class FirstBadVersionBenchmarks
{
    private const double FirstBadFraction = 0.7;

    private int _firstBad;

    [Params(1_000, 1_000_000)]
    public int VersionCount { get; set; }

    [GlobalSetup]
    public void Setup() => _firstBad = (int)(VersionCount * FirstBadFraction);

    [Benchmark(Baseline = true)]
    public int LinearScan() => FirstBadVersionSolution.FirstBadVersionByLinearScan(VersionCount, _firstBad);

    [Benchmark]
    public int PredicateSearch() =>
        FirstBadVersionSolution.FirstBadVersionByPredicateSearch(VersionCount, _firstBad);
}
