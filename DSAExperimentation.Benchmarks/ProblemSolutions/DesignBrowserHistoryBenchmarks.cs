using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignBrowserHistory.DesignBrowserHistorySolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignBrowserHistorySolution's, the same classes
// DesignBrowserHistoryTests proves correct - the textbook List<string> + cursor
// baseline against this repo's own DynamicArray<string> doing the same
// truncate-then-append, the "array Representation primitive vs. the BCL
// equivalent" comparison DesignCircularQueueBenchmarks already makes for
// Deque<int>. [GlobalSetup] materializes the urls so string formatting is charged
// to setup rather than to the replay; the replay itself is the pre-migration
// Visit/Back/Visit cycle unchanged, so every iteration exercises both the
// truncation path (discarding forward history) and plain append growth.
[MemoryDiagnoser]
public class DesignBrowserHistoryBenchmarks
{
    private const string HomePageUrl = "home.com";
    private const int BackSteps = 2;

    [Params(200, 5_000)]
    public int OperationCount;

    private string[] _visitUrls = null!;
    private string[] _branchUrls = null!;

    [GlobalSetup]
    public void Setup()
    {
        _visitUrls = BuildUrls("url", OperationCount);
        _branchUrls = BuildUrls("branch", OperationCount);
    }

    [Benchmark(Baseline = true)]
    public string ListBacked() => Replay(new BrowserHistoryByListBacked(HomePageUrl));

    [Benchmark]
    public string DynamicArrayBacked() => Replay(new BrowserHistoryByDynamicArrayBacked(HomePageUrl));

    private string Replay(IBrowserHistory history)
    {
        var last = HomePageUrl;

        for (var i = 0; i < OperationCount; i++)
        {
            history.Visit(_visitUrls[i]);
            last = history.Back(BackSteps);
            history.Visit(_branchUrls[i]);
        }

        return last;
    }

    private static string[] BuildUrls(string prefix, int count)
    {
        var urls = new string[count];

        for (var i = 0; i < count; i++)
        {
            urls[i] = $"{prefix}{i}.com";
        }

        return urls;
    }
}
