using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AssignCookies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AssignCookiesSolution's, the same methods
// AssignCookiesTests proves correct. Both arrays are drawn from the same range so
// most children have several candidate cookies, forcing brute force through a long
// rescan per child instead of matching on the first cookie it looks at.
[MemoryDiagnoser]
public class AssignCookiesBenchmarks
{
    private const int RandomSeed = 455; // LC problem number
    private const int RandomValueUpperBound = 1_000;

    private int[] _greed = [];

    private int[] _sizes = [];
    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _greed = Enumerable.Range(0, Length).Select(_ => random.Next(1, RandomValueUpperBound)).ToArray();
        _sizes = Enumerable.Range(0, Length).Select(_ => random.Next(1, RandomValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceScan() =>
        AssignCookiesSolution.FindContentChildrenByBruteForceScan(_greed, _sizes);

    [Benchmark]
    public int SortThenTwoPointer() =>
        AssignCookiesSolution.FindContentChildrenBySortThenTwoPointer(_greed, _sizes);
}
