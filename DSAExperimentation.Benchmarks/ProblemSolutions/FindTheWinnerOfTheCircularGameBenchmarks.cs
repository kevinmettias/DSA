using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheWinnerOfTheCircularGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheWinnerOfTheCircularGameSolution's, the same
// methods FindTheWinnerOfTheCircularGameTests proves correct. A List<int>-backed
// simulation whose RemoveAt re-shifts the remaining elements on every elimination
// (O(n) per round, O(n^2) total, independent of k) vs. this repo's own Queue<int>
// rotating k-1 friends from front to back per round (O(n*k) total) - faster whenever
// k is small relative to n, the common case this problem's constraints allow. Both
// arms build their own circle from the two integers LeetCode hands the problem, so
// there is nothing to hoist into a [GlobalSetup].
[MemoryDiagnoser]
public class FindTheWinnerOfTheCircularGameBenchmarks
{
    private const int K = 3;

    [Params(200, 2_000)]
    public int FriendCount { get; set; }

    [Benchmark(Baseline = true)]
    public int ByListRemoval() =>
        FindTheWinnerOfTheCircularGameSolution.FindTheWinnerByListRemoval(FriendCount, K);

    [Benchmark]
    public int ByQueueRotation() =>
        FindTheWinnerOfTheCircularGameSolution.FindTheWinnerByQueueRotation(FriendCount, K);
}
