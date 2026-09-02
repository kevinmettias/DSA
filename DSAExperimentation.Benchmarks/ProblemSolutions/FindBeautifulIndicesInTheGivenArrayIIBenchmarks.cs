using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindBeautifulIndicesInTheGivenArrayIISolution's, the
// same methods FindBeautifulIndicesInTheGivenArrayIITests proves correct. Same
// worst-case construction as Beautiful Indices I's benchmark (see there): a and b
// mismatch only at their very last character, forcing every naive scan attempt
// through almost the whole pattern before failing, with pattern length scaling
// alongside s so the O(n*m) vs O(n) gap widens with it - the gap #3008's larger
// published bound exists to make unavoidable.
[MemoryDiagnoser]
public class FindBeautifulIndicesInTheGivenArrayIIBenchmarks
{
    private const int SearchWindow = 100;

    [Params(200, 5_000)]
    public int Length;

    private string _s = null!;
    private string _a = null!;
    private string _b = null!;

    [GlobalSetup]
    public void Setup()
    {
        var patternLength = Math.Max(1, Length / 10);
        _s = new string('a', Length);
        _a = new string('a', patternLength - 1) + 'b';
        _b = new string('a', patternLength - 1) + 'c';
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByBruteForce(_s, _a, _b, SearchWindow);

    [Benchmark]
    public int[] ZFunction() =>
        FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByZFunction(_s, _a, _b, SearchWindow);
}
