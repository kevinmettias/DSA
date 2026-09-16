using DSAExperimentation.LeetCode.NumberOfMusicPlaylists;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfMusicPlaylists;

// Harness only: both strategies live in NumberOfMusicPlaylistsSolution and are
// asserted against the same examples - LeetCode's three, plus the degenerate
// single-song playlist and two alternating-playlist cases that pin down the
// "no replay within k songs" factor.
public sealed class NumberOfMusicPlaylistsTests
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
    public void NumMusicPlaylistsByTabulation_LeetCodeExamples_ReturnsPlaylistCount(
        int n,
        int goal,
        int k,
        long expected)
    {
        var actual = NumberOfMusicPlaylistsSolution.NumMusicPlaylistsByTabulation(n, goal, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumMusicPlaylistsByMemoizedRecurrence_LeetCodeExamples_ReturnsPlaylistCount(
        int n,
        int goal,
        int k,
        long expected)
    {
        var actual = NumberOfMusicPlaylistsSolution.NumMusicPlaylistsByMemoizedRecurrence(n, goal, k);

        Assert.Equal(expected, actual);
    }
}
