using DSAExperimentation.LeetCode.NumberOfMusicPlaylists;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfMusicPlaylists;

// Harness only: both strategies live in NumberOfMusicPlaylistsSolution and are
// asserted against the same examples - LeetCode's three, plus the degenerate
// single-song playlist and two alternating-playlist cases that pin down the
// "no replay within replayGap songs" factor.
public sealed partial class NumberOfMusicPlaylistsTests
{
    public static TheoryData<int, int, int, long> Examples =>
        new()
        {
            { 3, 3, 1, 6 },
            { 2, 3, 0, 6 },
            { 2, 3, 1, 2 },
            { 1, 1, 0, 1 },
            { 2, 2, 0, 2 },
            { 2, 4, 1, 2 },
            { 3, 4, 1, 18 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMusicPlaylistsByTabulation_LeetCodeExamples_ReturnsPlaylistCount(
        int songCount,
        int goal,
        int replayGap,
        long expected)
    {
        var actual = NumberOfMusicPlaylistsSolution.CountMusicPlaylistsByTabulation(songCount, goal, replayGap);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountMusicPlaylistsByMemoizedRecurrence_LeetCodeExamples_ReturnsPlaylistCount(
        int songCount,
        int goal,
        int replayGap,
        long expected)
    {
        var actual = NumberOfMusicPlaylistsSolution.CountMusicPlaylistsByMemoizedRecurrence(songCount, goal, replayGap);

        Assert.Equal(expected, actual);
    }
}
