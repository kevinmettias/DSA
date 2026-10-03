using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestUploadedPrefixBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins every answer the replay reports, from the workload's
// construction alone.
//
// Setup shuffles 1..VideoCount from a fixed seed, so the order can be restated here, and the answer
// after each upload is the longest run 1..k already uploaded - read off by keeping the highest
// upload time over each prefix, with no frontier or rescan. Every video is uploaded, so the last
// answer is VideoCount.
public sealed partial class LongestUploadedPrefixBenchmarksTests
{
    private const int SmallestVideoCount = 200;
    private const int RandomSeed = 2424;

    [Fact]
    public void RescanArray_ShuffledUploads_ReportsTheLongestPrefixAfterEveryUpload() =>
        Assert.Equal(ExpectedLongest(), BuildHarness().RescanArray());

    [Fact]
    public void SetFrontierAdvance_ShuffledUploads_ReportsTheLongestPrefixAfterEveryUpload() =>
        Assert.Equal(ExpectedLongest(), BuildHarness().SetFrontierAdvance());

    [Fact]
    public void Setup_ShuffledUploads_EndsWithEveryVideoInThePrefix() =>
        Assert.Equal(SmallestVideoCount, BuildHarness().SetFrontierAdvance()[^1]);

    // Prefix 1..k is complete once its last-uploaded video arrives, at the latest upload time
    // among videos 1..k; so the answer after upload t is the largest k whose latest time is <= t.
    private static int[] ExpectedLongest()
    {
        var random = new Random(RandomSeed);
        var order = Enumerable.Range(1, SmallestVideoCount).OrderBy(_ => random.Next()).ToArray();
        var uploadTime = new int[SmallestVideoCount + 1];

        for (var time = 0; time < order.Length; time++)
        {
            uploadTime[order[time]] = time;
        }

        var answers = new int[order.Length];
        var latest = -1;

        for (var k = 1; k <= SmallestVideoCount; k++)
        {
            latest = Math.Max(latest, uploadTime[k]);

            for (var time = latest; time < order.Length; time++)
            {
                answers[time] = k;
            }
        }

        return answers;
    }

    private static LongestUploadedPrefixBenchmarks BuildHarness()
    {
        var harness = new LongestUploadedPrefixBenchmarks { VideoCount = SmallestVideoCount };
        harness.Setup();

        return harness;
    }
}
