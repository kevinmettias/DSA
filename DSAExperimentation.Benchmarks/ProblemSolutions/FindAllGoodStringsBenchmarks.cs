using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindAllGoodStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindAllGoodStringsSolution's, the same methods
// FindAllGoodStringsTests proves correct - enumerating every candidate and
// substring-checking it with the BCL against the KMP-automaton digit DP that walks
// the state space directly. s1/s2 span the full alphabet at every position so the
// enumeration baseline pays its full 26^Length cost.
[MemoryDiagnoser]
public class FindAllGoodStringsBenchmarks
{
    private const string EvilSubstring = "ab";

    [Params(3, 4)]
    public int Length;

    private string _s1 = null!;
    private string _s2 = null!;
    private string _evil = null!;

    [GlobalSetup]
    public void Setup()
    {
        _s1 = new string('a', Length);
        _s2 = new string('z', Length);
        _evil = EvilSubstring;
    }

    [Benchmark(Baseline = true)]
    public int EnumerationScan() =>
        FindAllGoodStringsSolution.CountGoodStringsByEnumeration(Length, _s1, _s2, _evil);

    [Benchmark]
    public int AutomatonDigitDp() =>
        FindAllGoodStringsSolution.CountGoodStringsByAutomatonDigitDp(Length, _s1, _s2, _evil);
}
