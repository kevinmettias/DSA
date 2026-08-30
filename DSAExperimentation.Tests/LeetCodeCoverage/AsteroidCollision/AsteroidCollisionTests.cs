using RepoAsteroidStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AsteroidCollision;

// LeetCode 735. Asteroid Collision: a single left-to-right pass over this repo's
// own Stack<int> - the same "explicit repo Stack instead of the CLR's own
// System.Collections.Generic.Stack" move BasicCalculatorTests/DecodeStringTests
// already make. Every right-moving (positive) asteroid already on the stack
// keeps getting popped and compared against an incoming left-moving (negative)
// one until the incoming asteroid is destroyed, both are destroyed, or the
// stack empties/holds only left-movers - exactly LeetCode's collision rules,
// with no candidate ever revisited once it survives.
public sealed partial class AsteroidCollisionTests
{
    [Fact]
    public void Simulate_SmallerLeftMoverDestroyed_LargerSurvives()
        => Assert.Equal([5, 10], Simulate([5, 10, -5]));

    [Fact]
    public void Simulate_EqualSizedOpposingAsteroids_BothDestroyed()
        => Assert.Equal([], Simulate([8, -8]));

    [Fact]
    public void Simulate_LargerLeftMoverDestroysSmallerRightMovers()
        => Assert.Equal([10], Simulate([10, 2, -5]));

    [Fact]
    public void Simulate_AsteroidsMovingApart_NeverCollide()
        => Assert.Equal([-2, -1, 1, 2], Simulate([-2, -1, 1, 2]));

    private static int[] Simulate(int[] asteroids)
    {
        var stack = new RepoAsteroidStack();

        foreach (var asteroid in asteroids)
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

            if (alive)
            {
                stack.Push(current);
            }
        }

        var result = new int[stack.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return result;
    }
}
