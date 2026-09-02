using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CarFleetII;

// LeetCode 1776. Car Fleet II: a single right-to-left pass over this repo's own
// Stack<int> of candidate car indices ahead (CarFleet/DailyTemperatures/
// AsteroidCollision precedent for this repo's own Stack<T> instead of the CLR's
// System.Collections.Generic.Stack). For car i, a candidate j on top of the stack
// is popped for good whenever i can never catch it (i is no faster than j) or
// whenever i's own naive collision time with j would happen after j has already
// collided with something ahead of itself (t > answer[j]) - in both cases j can
// never be the actual next-blocking-car for i or for any car further behind, so
// it is safe to discard permanently rather than just skip for this one car.
public sealed partial class CarFleetIITests
{
    [Fact]
    public void GetCollisionTimes_LeetCodeExampleOne_ReturnsExpectedTimes()
    {
        int[][] cars = [[1, 2], [2, 1], [4, 3], [7, 2]];

        var times = GetCollisionTimes(cars);

        Assert.Equal([1.0, -1.0, 3.0, -1.0], times);
    }

    [Fact]
    public void GetCollisionTimes_LeetCodeExampleTwo_ReturnsChainedCollisionTimes()
    {
        int[][] cars = [[3, 4], [5, 4], [6, 3], [9, 1]];

        var times = GetCollisionTimes(cars);

        Assert.Equal([2.0, 1.0, 1.5, -1.0], times);
    }

    [Fact]
    public void GetCollisionTimes_EverySameSpeedCarNeverCatchesUp_ReturnsAllNegativeOne()
    {
        int[][] cars = [[1, 4], [5, 4], [9, 4]];

        var times = GetCollisionTimes(cars);

        Assert.Equal([-1.0, -1.0, -1.0], times);
    }

    private static double[] GetCollisionTimes(int[][] cars)
    {
        var answer = new double[cars.Length];
        var candidatesAhead = new RepoIndexStack();
        var state = new CollisionSearchState(cars, answer, candidatesAhead);

        for (var i = cars.Length - 1; i >= 0; i--)
        {
            answer[i] = -1.0;

            while (candidatesAhead.TryPeek(out var j))
            {
                if (TryResolveCollision(state, i, j))
                {
                    break;
                }
            }

            candidatesAhead.Push(i);
        }

        return answer;
    }

    // Resolves car i's collision against the current top-of-stack candidate j.
    // Returns true once answer[i] is settled; false means j can never be the
    // blocking car for i (or anyone behind i) and was popped for good, so the
    // caller should keep peeking the next candidate.
    private static bool TryResolveCollision(CollisionSearchState state, int i, int j)
    {
        var (cars, answer, candidatesAhead) = state;

        if (cars[i][1] <= cars[j][1])
        {
            candidatesAhead.TryPop(out _);
            return false;
        }

        var collisionTime = (double)(cars[j][0] - cars[i][0]) / (cars[i][1] - cars[j][1]);

        if (answer[j] < 0 || collisionTime <= answer[j])
        {
            answer[i] = collisionTime;
            return true;
        }

        candidatesAhead.TryPop(out _);
        return false;
    }

    private readonly record struct CollisionSearchState(int[][] Cars, double[] Answer, RepoIndexStack CandidatesAhead);
}
