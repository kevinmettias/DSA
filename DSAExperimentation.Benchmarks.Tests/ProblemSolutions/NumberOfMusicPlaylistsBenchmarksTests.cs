using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfMusicPlaylistsBenchmarks (ARCHITECTURE 17.9): both arms count the same
// playlists - bottom-up tabulation over the (length, unique) table against the Memoizer-driven
// top-down recursion over that table - so a harness whose arms disagree is timing two different
// questions. Both return the count modulo 1e9+7, the problem's whole answer rather than a proxy. This
// class has no Setup: its workload is entirely the [Params] axis plus the Goal property, so the same
// SongCount is the whole of what a rebuild can vary.
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

    [Fact]
    public void Tabulation_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    [Fact]
    public void Memoized_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    private static NumberOfMusicPlaylistsBenchmarks BuildHarness() =>
        new() { SongCount = SmallestSongCount };
}
