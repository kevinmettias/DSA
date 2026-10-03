using DSAExperimentation.LeetCode.FindAllGoodStrings;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindAllGoodStringsSolution's, the same methods
// FindAllGoodStringsSolutionTests proves correct - enumerating every candidate and
// substring-checking it with the BCL against the KMP-automaton digit DP that walks
// the state space directly. s1/s2 span the full alphabet at every position so the
// enumeration baseline pays its full 26^Length cost.
//
// Sizes are per arm. That 26^Length enumeration stops at 4 characters; the digit DP's
// (position, automaton state, tightness) states grow linearly in the length and run on
// to LC 1397's own bound of 500, and the two are compared at the lengths both run.
public class FindAllGoodStringsBenchmarks
{
    private const string EvilSubstring = "ab";

    private Dictionary<int, (string S1, string S2)> _boundsByLength = [];

    public static IEnumerable<int> EnumerationSizes => [3, 4];

    public static IEnumerable<int> AutomatonDigitDpSizes => [.. EnumerationSizes, 50, 500];

    // Every length any arm runs has its bounds built here, outside the timed region; an arm
    // looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _boundsByLength = AutomatonDigitDpSizes.ToDictionary(
            length => length,
            length => (new string('a', length), new string('z', length)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(EnumerationSizes))]
    public int EnumerationScan(int length)
    {
        var (s1, s2) = _boundsByLength[length];

        return FindAllGoodStringsSolution.CountGoodStringsByEnumeration(
            length,
            new FindAllGoodStringsSolution.LowerBound(s1),
            new FindAllGoodStringsSolution.UpperBound(s2),
            new FindAllGoodStringsSolution.ForbiddenSubstring(EvilSubstring));
    }

    [Benchmark]
    [ArgumentsSource(nameof(AutomatonDigitDpSizes))]
    public int AutomatonDigitDp(int length)
    {
        var (s1, s2) = _boundsByLength[length];

        return FindAllGoodStringsSolution.CountGoodStringsByAutomatonDigitDp(
            length,
            new FindAllGoodStringsSolution.LowerBound(s1),
            new FindAllGoodStringsSolution.UpperBound(s2),
            new FindAllGoodStringsSolution.ForbiddenSubstring(EvilSubstring));
    }
}
