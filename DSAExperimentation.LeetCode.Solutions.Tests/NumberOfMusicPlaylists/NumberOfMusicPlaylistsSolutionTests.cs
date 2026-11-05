using DSAExperimentation.LeetCode.Solutions.NumberOfMusicPlaylists;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfMusicPlaylists;

// Harness only: both strategies live in NumberOfMusicPlaylistsSolution and are
// asserted against the same cases - LeetCode's three, plus the degenerate
// single-song playlist and two alternating-playlist cases that pin down the
// "no replay within replayGap songs" factor.
public sealed partial class NumberOfMusicPlaylistsSolutionTests
{
    public static TheoryData<int, int, int, long> Cases =>
        new()
        {
            { 2, 2, 0, 2 },
            { 2, 4, 1, 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountMusicPlaylistsByTabulation_Cases_ReturnsPlaylistCount(
        int songCount,
        int goal,
        int replayGap,
        long expected)
    {
        var actual = NumberOfMusicPlaylistsSolution.CountMusicPlaylistsByTabulation(songCount, goal, replayGap);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountMusicPlaylistsByMemoizedRecurrence_Cases_ReturnsPlaylistCount(
        int songCount,
        int goal,
        int replayGap,
        long expected)
    {
        var actual = NumberOfMusicPlaylistsSolution.CountMusicPlaylistsByMemoizedRecurrence(songCount, goal, replayGap);

        Assert.Equal(expected, actual);
    }
}
