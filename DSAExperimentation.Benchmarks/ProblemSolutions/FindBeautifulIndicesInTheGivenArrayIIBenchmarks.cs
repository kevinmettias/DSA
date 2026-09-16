using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindBeautifulIndicesInTheGivenArrayIISolution's, the
// same methods FindBeautifulIndicesInTheGivenArrayIITests proves correct. Same
// worst-case construction as Beautiful Indices I's benchmark (see there): the prefix
// and nearby patterns mismatch only at their very last character, forcing every naive
// scan attempt through almost the whole pattern before failing, with pattern length
// scaling alongside the haystack so the O(n*m) vs O(n) gap widens with it - the gap
// #3008's larger published bound exists to make unavoidable.
[MemoryDiagnoser]
public class FindBeautifulIndicesInTheGivenArrayIIBenchmarks
{
    private const int SearchWindow = 100;

    private string _haystack = "";

    private string _prefixPattern = "";
    private string _nearbyPattern = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var patternLength = Math.Max(1, Length / 10);
        _haystack = new string('a', Length);
        _prefixPattern = new string('a', patternLength - 1) + 'b';
        _nearbyPattern = new string('a', patternLength - 1) + 'c';
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByBruteForce(
            new FindBeautifulIndicesInTheGivenArrayIISolution.Haystack(_haystack),
            new FindBeautifulIndicesInTheGivenArrayIISolution.PrefixPattern(_prefixPattern),
            new FindBeautifulIndicesInTheGivenArrayIISolution.NearbyPattern(_nearbyPattern),
            SearchWindow);

    [Benchmark]
    public int[] ZFunction() =>
        FindBeautifulIndicesInTheGivenArrayIISolution.FindBeautifulIndicesByZFunction(
            new FindBeautifulIndicesInTheGivenArrayIISolution.Haystack(_haystack),
            new FindBeautifulIndicesInTheGivenArrayIISolution.PrefixPattern(_prefixPattern),
            new FindBeautifulIndicesInTheGivenArrayIISolution.NearbyPattern(_nearbyPattern),
            SearchWindow);
}
