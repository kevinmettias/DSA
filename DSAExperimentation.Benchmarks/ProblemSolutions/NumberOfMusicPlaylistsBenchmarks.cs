using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfMusicPlaylists;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfMusicPlaylistsSolution's, the same methods
// NumberOfMusicPlaylistsTests proves correct - bottom-up tabulation over the
// (length, unique) table vs. the Memoizer-driven top-down recursion over the same
// state, both O(Goal * N). k is fixed below Goal/N's params (LeetCode's own
// n/2 <= goal - k <= n constraint isn't required for either implementation to be
// correct, but staying inside it keeps every benchmarked case realistic).
[MemoryDiagnoser]
public class NumberOfMusicPlaylistsBenchmarks
{
    private const int K = 2;

    [Params(20, 100)]
    public int N { get; set; }

    public int Goal => N;

    [Benchmark(Baseline = true)]
    public long Tabulation() => NumberOfMusicPlaylistsSolution.NumMusicPlaylistsByTabulation(N, Goal, K);

    [Benchmark]
    public long Memoized() => NumberOfMusicPlaylistsSolution.NumMusicPlaylistsByMemoizedRecurrence(N, Goal, K);
}
