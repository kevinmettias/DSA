using DSAExperimentation.LeetCode.LongestUploadedPrefix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestUploadedPrefixSolution's, the same classes
// LongestUploadedPrefixSolutionTests proves correct - the bool[] rescanned from video 1 on
// every query (O(n) per call, O(n^2) over the stream) against this repo's own
// Set<int> behind a frontier that only ever advances (O(1) amortized per upload,
// O(n) total). Both replay the identical shuffled arrival order, querying after
// every upload so the rescan really is charged once per call rather than once at
// the end. [GlobalSetup] shuffles that order so building it is not charged to
// either arm. Each arm returns every Longest answer, one per upload.
public class LongestUploadedPrefixBenchmarks
{
    private const int RandomSeed = 2424; // LC problem number

    private int[] _uploadOrder = [];

    private int[] _longest = [];

    [Params(200, 5_000)]
    public int VideoCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _uploadOrder = Enumerable.Range(1, VideoCount).OrderBy(_ => random.Next()).ToArray();
        _longest = new int[_uploadOrder.Length];
    }

    [Benchmark(Baseline = true)]
    public int[] RescanArray() => Replay(new LongestUploadedPrefixSolution.UploadedPrefixByRescanArray(VideoCount));

    [Benchmark]
    public int[] SetFrontierAdvance() => Replay(new LongestUploadedPrefixSolution.UploadedPrefixBySetFrontier());

    private int[] Replay(LongestUploadedPrefixSolution.IUploadedPrefix server)
    {
        for (var i = 0; i < _uploadOrder.Length; i++)
        {
            server.Upload(_uploadOrder[i]);
            _longest[i] = server.Longest();
        }

        return _longest;
    }
}
