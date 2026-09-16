using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.AsteroidCollision;

// LeetCode 735. Asteroid Collision: a single left-to-right pass resolving each
// incoming left-moving (negative) asteroid against whatever right-moving
// (positive) asteroids are still alive ahead of it, per LeetCode's collision
// rules - the larger magnitude survives, equal magnitudes destroy each other,
// and asteroids moving apart never interact.
//
// The two strategies differ only in what tracks "still alive ahead of it":
// a List<int> rescanned from the start after every collision, or this repo's
// own Stack<int> doing the textbook single sweep.
internal static class AsteroidCollisionSolution
{
    // The naive baseline: after resolving one collision, rescan the whole list
    // from the beginning rather than continuing from where the collision
    // happened. O(n) per collision, so O(n^2) overall once cascades happen.
    // Written without this repo's own Stack, as a baseline is meant to be.
    public static int[] SimulateByRepeatedScan(int[] asteroids)
    {
        var current = new List<int>(asteroids);
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

    private static bool TryResolveCollisionAt(List<int> current, int collisionIndex)
    {
        if (current[collisionIndex] <= 0 || current[collisionIndex + 1] >= 0)
        {
            return false;
        }

        RemoveCollidedAsteroids(current, collisionIndex);
        return true;
    }

    private static void RemoveCollidedAsteroids(List<int> current, int collisionIndex)
    {
        var left = current[collisionIndex];
        var right = current[collisionIndex + 1];

        if (left < -right)
        {
            current.RemoveAt(collisionIndex);
        }
        else if (left == -right)
        {
            current.RemoveAt(collisionIndex + 1);
            current.RemoveAt(collisionIndex);
        }
        else
        {
            current.RemoveAt(collisionIndex + 1);
        }
    }

    // This repo's own Stack<int>: every right-moving asteroid already on the
    // stack keeps getting popped and compared against an incoming left-mover
    // until the incoming asteroid is destroyed, both are destroyed, or the
    // stack empties/holds only left-movers - resolving every collision,
    // including cascades, in one O(n) sweep with no candidate ever revisited
    // once it survives.
    public static int[] SimulateByStackPass(int[] asteroids)
    {
        var stack = new RepoStack();

        foreach (var asteroid in asteroids)
        {
            ResolveIncomingAsteroid(stack, asteroid);
        }

        var result = new int[stack.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return result;
    }

    private static void ResolveIncomingAsteroid(RepoStack stack, int asteroid)
    {
        if (CanSurviveCollisions(stack, asteroid))
        {
            stack.Push(asteroid);
        }
    }

    private static bool CanSurviveCollisions(RepoStack stack, int asteroid)
    {
        var alive = true;

        while (alive)
        {
            var (isFacing, top) = GetMoverFacing(asteroid, stack);

            if (!isFacing)
            {
                break;
            }

            alive = CanAbsorbTopOfStack(stack, asteroid, top);
        }

        return alive;
    }

    // A left-moving asteroid is only in play while a right-moving one sits on top of
    // the stack; the peek hands that top back to the loop. Nothing reads it when the
    // answer is false, so it starts at zero.
    private static (bool IsFacing, int Top) GetMoverFacing(int current, RepoStack stack)
    {
        var top = 0;

        return (current < 0 && stack.TryPeek(out top) && top > 0, top);
    }

    // The incoming asteroid meets the right-moving one on top of the stack: a smaller
    // top is destroyed and the incoming asteroid keeps travelling, otherwise the
    // incoming asteroid dies - taking an equal-sized top with it.
    private static bool CanAbsorbTopOfStack(RepoStack stack, int asteroid, int top)
    {
        if (top < -asteroid)
        {
            stack.TryPop(out _);
            return true;
        }

        if (top == -asteroid)
        {
            stack.TryPop(out _);
        }

        return false;
    }
}
