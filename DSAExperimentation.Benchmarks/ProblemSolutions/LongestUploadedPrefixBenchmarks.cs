using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestUploadedPrefix;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestUploadedPrefixSolution's, the same classes
// LongestUploadedPrefixTests proves correct - the bool[] rescanned from video 1 on
// every query (O(n) per call, O(n^2) over the stream) against this repo's own
// Set<int> behind a frontier that only ever advances (O(1) amortized per upload,
// O(n) total). Both replay the identical shuffled arrival order, querying after
// every upload so the rescan really is charged once per call rather than once at
// the end. [GlobalSetup] shuffles that order so building it is not charged to
// either arm.
[MemoryDiagnoser]
public class LongestUploadedPrefixBenchmarks
{
    private const int RandomSeed = 2424; private int[] _uploadOrder = [];

    // LC problem number

    [Params(200, 5_000)]
    public int VideoCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _uploadOrder = Enumerable.Range(1, VideoCount).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RescanArray() => Replay(new LongestUploadedPrefixSolution.UploadedPrefixByRescanArray(VideoCount));

    [Benchmark]
    public int SetFrontierAdvance() => Replay(new LongestUploadedPrefixSolution.UploadedPrefixBySetFrontier());

    private int Replay(LongestUploadedPrefixSolution.IUploadedPrefix server)
    {
        var longest = 0;

        foreach (var video in _uploadOrder)
        {
            server.Upload(video);
            longest = server.Longest();
        }

        return longest;
    }
}
