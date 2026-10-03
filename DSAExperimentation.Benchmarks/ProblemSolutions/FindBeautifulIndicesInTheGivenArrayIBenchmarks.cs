using DSAExperimentation.LeetCode.FindBeautifulIndicesInTheGivenArrayI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindBeautifulIndicesInTheGivenArrayISolution's, the
// same methods FindBeautifulIndicesInTheGivenArrayISolutionTests proves correct. The anchor
// and nearby patterns each repeat the searched text's own character but mismatch only
// in their very last position - the classic worst case for a naive substring scan,
// where every start position is compared almost all the way through the pattern
// before failing, instead of an early first-character mismatch making brute force
// look artificially competitive. LC 3006 caps both patterns at 10 characters, so
// they stay at that cap at every Length and the brute force pays its O(n * 10)
// rather than a gap that widens with the text.
public class FindBeautifulIndicesInTheGivenArrayIBenchmarks
{
    private const int SearchWindow = 100;

    private const int PatternLength = 10;

    private string _searchedText = "";

    private string _anchorPattern = "";
    private string _nearbyPattern = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _searchedText = new string('a', Length);
        _anchorPattern = new string('a', PatternLength - 1) + 'b';
        _nearbyPattern = new string('a', PatternLength - 1) + 'c';
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        FindBeautifulIndicesInTheGivenArrayISolution.FindBeautifulIndicesByBruteForce(
            new FindBeautifulIndicesInTheGivenArrayISolution.SearchedText(_searchedText),
            new FindBeautifulIndicesInTheGivenArrayISolution.AnchorPattern(_anchorPattern),
            new FindBeautifulIndicesInTheGivenArrayISolution.NearbyPattern(_nearbyPattern),
            SearchWindow);

    [Benchmark]
    public int[] PrefixFunctionSearch() =>
        FindBeautifulIndicesInTheGivenArrayISolution.FindBeautifulIndicesByPrefixFunctionSearch(
            new FindBeautifulIndicesInTheGivenArrayISolution.SearchedText(_searchedText),
            new FindBeautifulIndicesInTheGivenArrayISolution.AnchorPattern(_anchorPattern),
            new FindBeautifulIndicesInTheGivenArrayISolution.NearbyPattern(_nearbyPattern),
            SearchWindow);
}
