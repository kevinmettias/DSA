using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RobotCollisions;

// LeetCode 2751. Robot Collisions: sort (Position, Index) pairs ascending via this
// repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence - the same
// custom-comparer shape QueueReconstructionByHeightTests.cs already exercises - then
// sweep left to right, pushing every right-mover's original index onto this repo's own
// Stack<int>. This is AsteroidCollisionTests' single-pass stack simulation generalized
// from pure destroy/survive to health-bearing collisions: whichever robot has lower
// health is zeroed out, the survivor's health drops by one, and (on a tie) both are
// zeroed - repeating against the next stack entry until the left-mover dies, wins
// outright, or the stack empties. Left-movers are never pushed: once one clears every
// right-mover behind it, nothing later in position order can ever catch up to it again.
public sealed class RobotCollisionsTests
{
    [Fact]
    public void Simulate_AllRobotsMovingSameDirection_NoCollisionsSurviveUnchanged()
    {
        int[] positions = [5, 4, 3, 2, 1];
        int[] healths = [2, 17, 9, 15, 10];

        var result = Simulate(positions, healths, "RRRRR");

        Assert.Equal([2, 17, 9, 15, 10], result);
    }

    [Fact]
    public void Simulate_LeetCodeExample_OnlyStrongestRightMoverSurvives()
    {
        int[] positions = [3, 5, 2, 6];
        int[] healths = [10, 10, 15, 12];

        var result = Simulate(positions, healths, "RLRL");

        Assert.Equal([14], result);
    }

    [Fact]
    public void Simulate_SingleLeftMoverCascadesThroughMultipleRightMovers_LosesOneHealthPerKill()
    {
        int[] positions = [1, 2, 3, 4];
        int[] healths = [5, 3, 1, 10];

        var result = Simulate(positions, healths, "RRRL");

        Assert.Equal([7], result);
    }

    private static int[] Simulate(int[] positions, int[] healths, string directions)
    {
        var n = positions.Length;
        var byPosition = positions.Select((position, index) => (Position: position, Index: index)).ToArray();
        var ascendingByPosition = Comparer<(int Position, int Index)>.Create((a, b) => a.Position.CompareTo(b.Position));

        MergeSort.Sort<(int Position, int Index), ArrayIndexedSequence<(int Position, int Index)>>(
            new ArrayIndexedSequence<(int Position, int Index)>(byPosition), ascendingByPosition);

        var health = (int[])healths.Clone();
        var rightMovers = new RepoStack();

        foreach (var (_, index) in byPosition)
        {
            if (directions[index] == 'R')
            {
                rightMovers.Push(index);
            }
            else
            {
                ResolveLeftMover(rightMovers, health, index);
            }
        }

        return [.. Enumerable.Range(0, n).Where(i => health[i] > 0).Select(i => health[i])];
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
}
