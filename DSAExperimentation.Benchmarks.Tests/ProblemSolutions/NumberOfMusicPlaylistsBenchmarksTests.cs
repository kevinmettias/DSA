using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfMusicPlaylistsBenchmarks (ARCHITECTURE 17.9): a property of the
// workload's shape across sizes, which the generic check - run at the smallest SongCount only -
// cannot see. This class has no Setup: its workload is entirely the [Params] axis plus the derived
// Goal property.
public sealed partial class NumberOfMusicPlaylistsBenchmarksTests
{
    private const int SmallestSongCount = 20;
    private const int LargestSongCount = 100;

    // Goal is derived rather than tuned, so what needs checking is that it still tracks the tuned
    // axis: both arms benchmark (SongCount, Goal, ReplayGap), and a Goal pinned to one value while
    // SongCount varies would silently benchmark playlists of the wrong length.
    [Fact]
    public void Goal_FollowsSongCount_SoBothArmsMeasureTheFullLengthPlaylist()
    {
        var harness = BuildHarness();

        harness.SongCount = SmallestSongCount;
        var smallestGoal = harness.Goal;
        harness.SongCount = LargestSongCount;

        Assert.Equal(SmallestSongCount, smallestGoal);
        Assert.Equal(LargestSongCount, harness.Goal);
    }

    private static NumberOfMusicPlaylistsBenchmarks BuildHarness() =>
        new() { SongCount = SmallestSongCount };
}
