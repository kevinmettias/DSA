using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DesignBrowserHistory;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignBrowserHistorySolution's, the same classes
// DesignBrowserHistorySolutionTests proves correct - the textbook List<string> + cursor
// baseline against this repo's own DynamicArray<string> doing the same
// truncate-then-append, the "array Representation primitive vs. the BCL
// equivalent" comparison DesignCircularQueueBenchmarks already makes for
// Deque<int>. [GlobalSetup] materializes the urls so building them is charged to
// setup rather than to the replay; LC 1472 spells a url in '.' and lowercase
// letters only, so url i is its prefix, LowercaseNames.Of(i) and ".com". Every
// iteration visits a url, steps one page back and visits a branch url, so the
// branch truncates the url it stepped back from and the history grows by one page
// per iteration: each iteration exercises both the truncation path (discarding
// forward history) and append growth. Each arm returns every page Back landed on,
// in call order. LC 1472 allows 5000 calls in all and an iteration makes three, so
// the larger OperationCount is 1666.
public class DesignBrowserHistoryBenchmarks
{
    private const string HomePageUrl = "home.com";

    private const string UrlSuffix = ".com";

    private const int BackSteps = 1;

    private string[] _visitUrls = [];

    private string[] _branchUrls = [];

    private string[] _backPages = [];

    [Params(200, 1_666)]
    public int OperationCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _visitUrls = BuildUrls("url", OperationCount);
        _branchUrls = BuildUrls("branch", OperationCount);
        _backPages = new string[OperationCount];
    }

    private static string[] BuildUrls(string prefix, int count)
    {
        var urls = new string[count];

        for (var i = 0; i < count; i++)
        {
            urls[i] = prefix + LowercaseNames.Of(i) + UrlSuffix;
        }

        return urls;
    }

    [Benchmark(Baseline = true)]
    public string[] ListBacked() => Replay(new DesignBrowserHistorySolution.BrowserHistoryByListBacked(HomePageUrl));

    [Benchmark]
    public string[] DynamicArrayBacked() => Replay(new DesignBrowserHistorySolution.BrowserHistoryByDynamicArrayBacked(HomePageUrl));

    private string[] Replay(DesignBrowserHistorySolution.IBrowserHistory history)
    {
        for (var i = 0; i < OperationCount; i++)
        {
            history.Visit(_visitUrls[i]);
            _backPages[i] = history.Back(BackSteps);
            history.Visit(_branchUrls[i]);
        }

        return _backPages;
    }
}
