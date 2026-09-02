using BenchmarkDotNet.Attributes;
using RepoAsteroidStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Asteroid Collision (LC 735): a naive repeated-scan baseline that restarts
// from the beginning of a plain List<int> after every single collision it
// resolves (a fresh O(n) scan per collision, so O(n^2) overall once cascades
// happen) vs. this repo's own Stack<int> doing the textbook single
// left-to-right pass - the same "explicit repo Stack" move
// AsteroidCollisionTests itself makes - resolving every collision, including
// cascades, in one O(n) sweep.
[MemoryDiagnoser]
public class AsteroidCollisionBenchmarks
{
    // LC problem number, used as the deterministic seed for asteroid generation.
    private const int RandomSeed = 735;

    // Exclusive upper bound for an asteroid's magnitude.
    private const int MaxMagnitude = 1_000;

    // Coin-flip range: half the asteroids move left (negative), half move right.
    private const int SignCoinFlipRange = 2;

    [Params(200, 3_000)]
    public int Length;

    private int[] _asteroids = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _asteroids = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var magnitude = random.Next(1, MaxMagnitude);
                return random.Next(SignCoinFlipRange) == 0 ? magnitude : -magnitude;
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] RepeatedScan()
    {
        var current = new List<int>(_asteroids);
        var collisionFound = true;

        while (collisionFound)
        {
            collisionFound = false;

            for (var i = 0; i < current.Count - 1; i++)
            {
                if (TryResolveCollisionAt(current, i))
                {
                    collisionFound = true;
                    break;
                }
            }
        }

        return [.. current];
    }

    private static bool TryResolveCollisionAt(List<int> current, int i)
    {
        if (current[i] <= 0 || current[i + 1] >= 0)
        {
            return false;
        }

        RemoveCollidedAsteroids(current, i);
        return true;
    }

    private static void RemoveCollidedAsteroids(List<int> current, int i)
    {
        var left = current[i];
        var right = current[i + 1];

        if (left < -right)
        {
            current.RemoveAt(i);
        }
        else if (left == -right)
        {
            current.RemoveAt(i + 1);
            current.RemoveAt(i);
        }
        else
        {
            current.RemoveAt(i + 1);
        }
    }

    [Benchmark]
    public int[] StackSimulation()
    {
        var stack = new RepoAsteroidStack();

        foreach (var asteroid in _asteroids)
        {
            ProcessAsteroid(stack, asteroid);
        }

        var result = new int[stack.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return result;
    }

    private static void ProcessAsteroid(RepoAsteroidStack stack, int asteroid)
    {
        if (SurvivesCollisions(stack, asteroid))
        {
            stack.Push(asteroid);
        }
    }

    private static bool SurvivesCollisions(RepoAsteroidStack stack, int asteroid)
    {
        var current = asteroid;
        var alive = true;

        while (alive && current < 0 && stack.TryPeek(out var top) && top > 0)
        {
            if (top < -current)
            {
                stack.TryPop(out _);
                continue;
            }

            if (top == -current)
            {
                stack.TryPop(out _);
            }

            alive = false;
        }

        return alive;
    }
}
