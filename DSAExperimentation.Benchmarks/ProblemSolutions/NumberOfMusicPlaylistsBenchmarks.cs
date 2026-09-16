using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfMusicPlaylists;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfMusicPlaylistsSolution's, the same methods
// NumberOfMusicPlaylistsTests proves correct - bottom-up tabulation over the
// (length, unique) table vs. the Memoizer-driven top-down recursion over the same
// state, both O(Goal * SongCount). replayGap is fixed below Goal/SongCount's params
// (LeetCode's own n/2 <= goal - k <= n constraint isn't required for either
// implementation to be correct, but staying inside it keeps every benchmarked case
// realistic).
[MemoryDiagnoser]
public class NumberOfMusicPlaylistsBenchmarks
{
    private const int ReplayGap = 2;

    [Params(20, 100)]
    public int SongCount { get; set; }

    public int Goal => SongCount;

    [Benchmark(Baseline = true)]
    public long Tabulation() =>
        NumberOfMusicPlaylistsSolution.CountMusicPlaylistsByTabulation(SongCount, Goal, ReplayGap);

    [Benchmark]
    public long Memoized() =>
        NumberOfMusicPlaylistsSolution.CountMusicPlaylistsByMemoizedRecurrence(SongCount, Goal, ReplayGap);
}
