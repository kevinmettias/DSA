using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.RobotCollisions;

// LeetCode 2751. Robot Collisions: robots at distinct positions move left or
// right; when two meet, the one with less health is removed and the survivor
// loses one health, and on a tie both are removed. Report the healths of the
// survivors in the order the robots were given.
//
// Both strategies establish position order the same way - this repo's own
// MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence of
// (Position, Index) pairs, the same custom-comparer shape
// QueueReconstructionByHeightSolution uses - so the only difference between
// them is how collisions are resolved afterward.
internal static class RobotCollisionsSolution
{
    private const char RightMoving = 'R';
    private const char LeftMoving = 'L';

    // The naive baseline: resolve ONE adjacent right-then-left collision at a
    // time over a plain List<T>, restarting the scan from the front after every
    // resolution - O(n) per collision, so O(n^2) once cascades happen. This is
    // AsteroidCollisionSolution.SimulateByRepeatedScan generalized from pure
    // destroy/survive to health-bearing collisions.
    public static int[] SurvivorHealthsByRepeatedScan(int[] positions, int[] healths, string directions)
    {
        var items = SortedIndicesByPosition(positions)
            .Select(index => (Index: index, Health: healths[index], Direction: directions[index]))
            .ToList();
        var collisionFound = true;

        while (collisionFound)
        {
            collisionFound = false;

            for (var i = 0; i < items.Count - 1; i++)
            {
                if (items[i].Direction == RightMoving && items[i + 1].Direction == LeftMoving)
                {
                    ResolvePair(items, i);
                    collisionFound = true;
                    break;
                }
            }
        }

        return [.. items.OrderBy(item => item.Index).Select(item => item.Health)];
    }

    private static void ResolvePair(List<(int Index, int Health, char Direction)> items, int i)
    {
        var (leftIndex, leftHealth, leftDirection) = items[i];
        var (rightIndex, rightHealth, rightDirection) = items[i + 1];

        if (leftHealth > rightHealth)
        {
            items[i] = (leftIndex, leftHealth - 1, leftDirection);
            items.RemoveAt(i + 1);
        }
        else if (leftHealth < rightHealth)
        {
            items[i + 1] = (rightIndex, rightHealth - 1, rightDirection);
            items.RemoveAt(i);
        }
        else
        {
            items.RemoveAt(i + 1);
            items.RemoveAt(i);
        }
    }

    // One left-to-right sweep, pushing every right-mover's original index onto
    // this repo's own Stack<int> and resolving each left-mover against the
    // stack until it dies, wins outright, or the stack empties. Left-movers are
    // never pushed: once one has cleared every right-mover behind it, nothing
    // later in position order can ever catch up to it. Each index is pushed and
    // popped at most once, so collision resolution is O(n) in total rather than
    // an O(n) rescan per collision.
    public static int[] SurvivorHealthsByStackSimulation(int[] positions, int[] healths, string directions)
    {
        var health = (int[])healths.Clone();
        var rightMovers = new RepoStack();

        foreach (var index in SortedIndicesByPosition(positions))
        {
            if (directions[index] == RightMoving)
            {
                rightMovers.Push(index);
            }
            else
            {
                ResolveLeftMover(rightMovers, health, index);
            }
        }

        return [.. Enumerable.Range(0, positions.Length).Where(i => health[i] > 0).Select(i => health[i])];
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
                DestroyBothRobots(rightMovers, health, topIndex, leftIndex);
                alive = false;
            }
        }
    }

    // A collision between two robots of equal health destroys both: the survivor
    // loses one health, and on a tie - which leaves no survivor - neither keeps any.
    private static void DestroyBothRobots(RepoStack rightMovers, int[] health, int topIndex, int leftIndex)
    {
        health[topIndex] = 0;
        health[leftIndex] = 0;
        rightMovers.TryPop(out _);
    }

    // Original indices, ordered by the position each robot starts at.
    private static int[] SortedIndicesByPosition(int[] positions)
    {
        var byPosition = positions.Select((position, index) => (Position: position, Index: index)).ToArray();
        var ascendingByPosition = Comparer<(int Position, int Index)>.Create(
            (a, b) => a.Position.CompareTo(b.Position));

        MergeSort.Sort<(int Position, int Index), ArrayIndexedSequence<(int Position, int Index)>>(
            new ArrayIndexedSequence<(int Position, int Index)>(byPosition), ascendingByPosition);

        return [.. byPosition.Select(item => item.Index)];
    }
}
