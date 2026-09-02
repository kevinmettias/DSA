using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find the Winner of the Circular Game (LC 1823): a List<int>-backed simulation whose
// RemoveAt re-shifts the remaining elements on every elimination (O(n) per round,
// O(n^2) total, independent of k) vs. this repo's own Queue<int> rotating k-1 friends
// from front to back per round (O(n*k) total) - faster whenever k is small relative
// to n, the common case this problem's constraints allow.
[MemoryDiagnoser]
public class FindTheWinnerOfTheCircularGameBenchmarks
{
    private const int K = 3;

    [Params(200, 2_000)]
    public int FriendCount;

    [Benchmark(Baseline = true)]
    public int ListRemoveAt()
    {
        var friends = new List<int>(FriendCount);

        for (var friend = 1; friend <= FriendCount; friend++)
        {
            friends.Add(friend);
        }

        var index = 0;

        while (friends.Count > 1)
        {
            index = (index + K - 1) % friends.Count;
            friends.RemoveAt(index);
        }

        return friends[0];
    }

    [Benchmark]
    public int QueueRotation()
    {
        var friends = new RepoQueue();

        for (var friend = 1; friend <= FriendCount; friend++)
        {
            friends.Enqueue(friend);
        }

        while (friends.Count > 1)
        {
            for (var step = 0; step < K - 1; step++)
            {
                friends.TryDequeue(out var rotated);
                friends.Enqueue(rotated);
            }

            friends.TryDequeue(out _);
        }

        friends.TryDequeue(out var winner);
        return winner;
    }
}
