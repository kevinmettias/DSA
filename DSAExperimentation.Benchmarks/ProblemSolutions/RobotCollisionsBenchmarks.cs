using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Robot Collisions (LC 2751): both arms sort positions the same way, via this repo's
// own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence (RobotCollisionsTests
// precedent) - the difference is entirely in how collisions get resolved afterward.
// RepeatedScan resolves ONE adjacent right-then-left collision at a time over a plain
// List<T>, restarting the scan from the front after every single resolution - the same
// AsteroidCollisionBenchmarks.RepeatedScan cascading-restart shape, generalized from
// pure destroy/survive to health-bearing collisions. StackSimulation instead sweeps left
// to right exactly once, pushing every right-mover's original index onto this repo's own
// Stack<int> and resolving each left-mover against the stack until it dies, wins
// outright, or the stack empties, so each index is pushed/popped at most once for O(n)
// total collision resolution instead of an O(n) rescan per collision.
[MemoryDiagnoser]
public class RobotCollisionsBenchmarks
{
    private const int MaxHealthExclusive = 1_000;
    private const int Seed = 2751;

    [Params(200, 3_000)]
    public int Length;

    private int[] _positions = null!;
    private int[] _healths = null!;
    private string _directions = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);

        var shuffledPositions = Enumerable.Range(1, Length).ToArray();
        for (var i = shuffledPositions.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (shuffledPositions[i], shuffledPositions[j]) = (shuffledPositions[j], shuffledPositions[i]);
        }

        _positions = shuffledPositions;
        _healths = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxHealthExclusive)).ToArray();
        _directions = new string([.. Enumerable.Range(0, Length).Select(_ => random.Next(2) == 0 ? 'L' : 'R')]);
    }

    [Benchmark(Baseline = true)]
    public int[] RepeatedScan()
    {
        var items = SortedByPosition().Select(i => (Index: i, Health: _healths[i], Dir: _directions[i])).ToList();
        var collisionFound = true;

        while (collisionFound)
        {
            collisionFound = false;

            for (var i = 0; i < items.Count - 1; i++)
            {
                if (items[i].Dir == 'R' && items[i + 1].Dir == 'L')
                {
                    ResolvePair(items, i);
                    collisionFound = true;
                    break;
                }
            }
        }

        return [.. items.OrderBy(item => item.Index).Select(item => item.Health)];
    }

    private static void ResolvePair(List<(int Index, int Health, char Dir)> items, int i)
    {
        var (leftIndex, leftHealth, leftDir) = items[i];
        var (rightIndex, rightHealth, rightDir) = items[i + 1];

        if (leftHealth > rightHealth)
        {
            items[i] = (leftIndex, leftHealth - 1, leftDir);
            items.RemoveAt(i + 1);
        }
        else if (leftHealth < rightHealth)
        {
            items[i + 1] = (rightIndex, rightHealth - 1, rightDir);
            items.RemoveAt(i);
        }
        else
        {
            items.RemoveAt(i + 1);
            items.RemoveAt(i);
        }
    }

    [Benchmark]
    public int[] StackSimulation()
    {
        var order = SortedByPosition();
        var health = (int[])_healths.Clone();
        var rightMovers = new RepoStack();

        foreach (var index in order)
        {
            if (_directions[index] == 'R')
            {
                rightMovers.Push(index);
            }
            else
            {
                ResolveLeftMover(rightMovers, health, index);
            }
        }

        return [.. Enumerable.Range(0, Length).Where(i => health[i] > 0).Select(i => health[i])];
    }

    private static void ResolveLeftMover(RepoStack rightMovers, int[] health, int leftIndex)
    {
        var alive = true;

        while (alive && rightMovers.TryPeek(out var topIndex))
        {
            if (health[topIndex] > health[leftIndex])
            {
                health[topIndex]--;
                health[leftIndex] = 0;
                alive = false;
            }
            else if (health[topIndex] < health[leftIndex])
            {
                health[leftIndex]--;
                health[topIndex] = 0;
                rightMovers.TryPop(out _);
            }
            else
            {
                health[topIndex] = 0;
                health[leftIndex] = 0;
                rightMovers.TryPop(out _);
                alive = false;
            }
        }
    }

    private int[] SortedByPosition()
    {
        var byPosition = _positions.Select((position, index) => (Position: position, Index: index)).ToArray();
        var ascendingByPosition = Comparer<(int Position, int Index)>.Create((a, b) => a.Position.CompareTo(b.Position));

        MergeSort.Sort<(int Position, int Index), ArrayIndexedSequence<(int Position, int Index)>>(
            new ArrayIndexedSequence<(int Position, int Index)>(byPosition), ascendingByPosition);

        return [.. byPosition.Select(item => item.Index)];
    }
}
