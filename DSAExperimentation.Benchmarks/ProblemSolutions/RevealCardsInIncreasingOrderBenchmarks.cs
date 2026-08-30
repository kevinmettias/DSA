using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reveal Cards In Increasing Order (LC 950): the naive simulation most people reach for
// first - a List<int> of indices with RemoveAt(0) standing in for "pop the front" - is
// O(n) per removal, O(n^2) total, vs. this repo's own Queue<int> (Deque-backed, O(1)
// amortized push/pop at both ends) driving the identical reveal/move-to-bottom
// simulation in O(n). Both pay the same O(n log n) sort up front, so the asymmetry shows
// up entirely in the simulation loop itself.
[MemoryDiagnoser]
public class RevealCardsInIncreasingOrderBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _deck = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(950);
        _deck = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1_000_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] ListRemoveAtSimulation()
    {
        var sorted = (int[])_deck.Clone();
        Array.Sort(sorted);

        var indices = new List<int>(_deck.Length);

        for (var i = 0; i < _deck.Length; i++)
        {
            indices.Add(i);
        }

        var result = new int[_deck.Length];

        foreach (var value in sorted)
        {
            var revealIndex = indices[0];
            indices.RemoveAt(0);
            result[revealIndex] = value;

            if (indices.Count > 0)
            {
                var moveToBottom = indices[0];
                indices.RemoveAt(0);
                indices.Add(moveToBottom);
            }
        }

        return result;
    }

    [Benchmark]
    public int[] QueueSimulation()
    {
        var sorted = (int[])_deck.Clone();
        Array.Sort(sorted);

        var indices = new RepoQueue();

        for (var i = 0; i < _deck.Length; i++)
        {
            indices.Enqueue(i);
        }

        var result = new int[_deck.Length];

        foreach (var value in sorted)
        {
            indices.TryDequeue(out var revealIndex);
            result[revealIndex] = value;

            if (indices.TryDequeue(out var moveToBottom))
            {
                indices.Enqueue(moveToBottom);
            }
        }

        return result;
    }
}
