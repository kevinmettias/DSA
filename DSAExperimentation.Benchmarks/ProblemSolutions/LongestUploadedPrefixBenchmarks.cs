using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Uploaded Prefix (LC 2424): recomputing the longest contiguous prefix from
// scratch after every upload (a bool[] rescanned from 1 each call, O(n) per call,
// O(n^2) total) vs. this repo's own Set<int> (backed by HashMap) tracking uploaded
// videos with a frontier pointer that only ever advances - O(1) amortized per
// upload, O(n) total, since the pointer never revisits a video number twice.
[MemoryDiagnoser]
public class LongestUploadedPrefixBenchmarks
{
    [Params(200, 5_000)]
    public int VideoCount;

    private int[] _uploadOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(2424); // LeetCode problem number
        _uploadOrder = Enumerable.Range(1, VideoCount).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NaiveRescanPerUpload()
    {
        var uploaded = new bool[VideoCount + 1];
        var longest = 0;

        foreach (var video in _uploadOrder)
        {
            uploaded[video - 1] = true;
            longest = 0;

            while (longest < VideoCount && uploaded[longest])
            {
                longest++;
            }
        }

        return longest;
    }

    [Benchmark]
    public int SetFrontierAdvance()
    {
        var uploaded = new Set<int>();
        var longest = 0;

        foreach (var video in _uploadOrder)
        {
            uploaded.TryAdd(video);

            while (uploaded.Has(longest + 1))
            {
                longest++;
            }
        }

        return longest;
    }
}
