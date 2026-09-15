using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RotateString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RotateStringSolution's, the same methods
// RotateStringTests proves correct. _s/_goal are both runs of 'a' with one
// differing trailing character, so nearly every scan position inside s+s is a long
// near-miss - the worst case for the restart-on-mismatch scan and exactly what
// KMP's failure function is built to skip.
[MemoryDiagnoser]
public class RotateStringBenchmarks
{
    private string _s = "";

    private string _goal = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _s = new string('a', Length - 1) + 'b';
        _goal = new string('a', Length - 1) + 'c';
    }

    [Benchmark(Baseline = true)]
    public bool NaiveSubstringScan() =>
        RotateStringSolution.CanRotateByNaiveScan(
            new RotateStringSolution.RotationSource(_s),
            new RotateStringSolution.RotationGoal(_goal));

    [Benchmark]
    public bool KmpSearch() =>
        RotateStringSolution.CanRotateByPrefixFunction(
            new RotateStringSolution.RotationSource(_s),
            new RotateStringSolution.RotationGoal(_goal));
}
