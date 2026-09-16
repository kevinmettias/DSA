using DSAExperimentation.LeetCode.LongestUploadedPrefix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestUploadedPrefix;

// Harness only: both strategies live in LongestUploadedPrefixSolution. LeetCode's
// own shape here is a stateful object across a sequence of calls, so an example is
// a call script rather than a single argument tuple - the same shape
// SeatReservationManagerTests uses for its own instance-API problem. The expected
// sequence carries one more entry than the script's uploads: its first entry is
// what longest() reports before anything has been uploaded, which is the published
// example's own first query.
//
// The rescan baseline is asserted here too, which is the point of hoisting it into
// the solution class: before this migration it lived only in the benchmark's
// baseline arm and nothing checked that the arm the frontier is measured against
// was even right. It is also the one strategy that needs the stream capacity, which
// is why the script states it.
public sealed partial class LongestUploadedPrefixTests
{
    public static TheoryData<UploadScript, int[]> Examples =>
        new()
        {
            // LeetCode's published example: LUPrefix(4), then upload 3, 1, 2 with a
            // longest() after each - 0, then 1, then 3.
            { new UploadScript(VideoCount: 4, [3, 1, 2]), [0, 0, 1, 3] },

            // Nothing uploaded at all, the pre-section-17 test's second case.
            { new UploadScript(VideoCount: 1, []), [0] },

            // A gap left open and then filled: the frontier stalls at 2 while 4 sits
            // stranded, then jumps straight to 4 when 3 arrives.
            { new UploadScript(VideoCount: 4, [1, 2, 4, 3]), [0, 1, 2, 2, 4] },

            // The same video twice, which must not advance the frontier twice.
            { new UploadScript(VideoCount: 1, [1, 1]), [0, 1, 1] },

            // Arrivals in descending order: nothing is contiguous until the very
            // last upload completes the whole stream at once.
            { new UploadScript(VideoCount: 5, [5, 4, 3, 2, 1]), [0, 0, 0, 0, 0, 5] },

            // Arrivals already in order, so every upload extends the prefix by one.
            { new UploadScript(VideoCount: 3, [1, 2, 3]), [0, 1, 2, 3] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void UploadedPrefixByRescanArray_LeetCodeExamples_ReportsLongestPrefixAfterEachUpload(
        UploadScript script, int[] expected) =>
        AssertScript(
            new LongestUploadedPrefixSolution.UploadedPrefixByRescanArray(script.VideoCount),
            script.Uploads,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void UploadedPrefixBySetFrontier_LeetCodeExamples_ReportsLongestPrefixAfterEachUpload(
        UploadScript script, int[] expected) =>
        AssertScript(
            new LongestUploadedPrefixSolution.UploadedPrefixBySetFrontier(), script.Uploads, expected);

    private static void AssertScript(
        LongestUploadedPrefixSolution.IUploadedPrefix server, int[] uploads, int[] expected)
    {
        Assert.Equal(expected[0], server.Longest());

        for (var i = 0; i < uploads.Length; i++)
        {
            server.Upload(uploads[i]);

            Assert.Equal(expected[i + 1], server.Longest());
        }
    }

    // One LeetCode call script: the stream capacity LUPrefix's constructor was given,
    // and the videos uploaded after it. The two travel together because only one of
    // the strategies takes the capacity - the frontier never allocates per video, so it
    // answers the identical script without being told how many videos exist.
    //
    // Nested because it is only ever used inside this test class and has no independent
    // identity: it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct UploadScript(int VideoCount, int[] Uploads);
}
