using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game III (LC 1306): a hand-rolled Stack<int>+bool[] reachability walk,
// written the textbook way, vs. this repo's own DepthFirstSearch.Traverse closed
// over a bare successor Func<int,IEnumerable<int>> - the same composition
// JumpGameIIITests uses. _arr deliberately never contains a 0 (values drawn from
// [1, Length)), the same "force the real worst case" intent TwoSumBenchmarks'
// _target uses: neither strategy can short-circuit on an early hit, so both are
// forced through the full reachable component from _start.
[MemoryDiagnoser]
public class JumpGameIIIBenchmarks
{
    // LC problem number, reused as the deterministic seed.
    private const int RandomSeed = 1306;

    [Params(200, 5_000)]
    public int Length;

    private int[] _arr = null!;
    private int _start;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _arr[i] = random.Next(1, Length);
        }

        _start = 0;
    }

    [Benchmark(Baseline = true)]
    public bool HandRolledStackWalk()
    {
        var visited = new bool[_arr.Length];
        var pending = new Stack<int>();
        pending.Push(_start);

        while (pending.Count > 0)
        {
            if (VisitNext(pending, visited))
            {
                return true;
            }
        }

        return false;
    }

    private bool VisitNext(Stack<int> pending, bool[] visited)
    {
        var index = pending.Pop();

        if (visited[index])
        {
            return false;
        }

        visited[index] = true;

        if (_arr[index] == 0)
        {
            return true;
        }

        EnqueueNeighbors(index, pending, visited);
        return false;
    }

    private void EnqueueNeighbors(int index, Stack<int> pending, bool[] visited)
    {
        var forward = index + _arr[index];
        var backward = index - _arr[index];

        if (forward < _arr.Length && !visited[forward])
        {
            pending.Push(forward);
        }

        if (backward >= 0 && !visited[backward])
        {
            pending.Push(backward);
        }
    }

    [Benchmark]
    public bool DepthFirstSearchTraverse()
    {
        var visited = DepthFirstSearch.Traverse(_start, Successors);
        return visited.Exists(index => _arr[index] == 0);
    }

    private IEnumerable<int> Successors(int index)
    {
        var forward = index + _arr[index];
        var backward = index - _arr[index];

        if (forward < _arr.Length)
        {
            yield return forward;
        }

        if (backward >= 0)
        {
            yield return backward;
        }
    }
}
