using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EliminationGame;

// LeetCode 390. Elimination Game: closed-form O(log n) head/step-tracking arithmetic,
// cross-validated for a range of n against an O(n) DynamicArray<int> brute-force
// simulation of the actual left-to-right/right-to-left elimination passes. No repo
// primitive applies to the closed-form arithmetic itself - the same "lighter
// repo-primitive fit" case already accepted for Pow(x, n)/Rectangle Area - but
// DynamicArray<int> stands in for the simulation's surviving-numbers list the same
// way it already stands in for Count Primes' sieve array.
public sealed class EliminationGameTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    [InlineData(9, 6)]
    public void LastRemaining_KnownValues_MatchesExpected(int n, int expected)
        => Assert.Equal(expected, LastRemaining(n));

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(9)]
    [InlineData(17)]
    [InlineData(64)]
    [InlineData(100)]
    [InlineData(257)]
    public void LastRemaining_AgreesWithDynamicArraySimulation_ForVariousN(int n)
        => Assert.Equal(SimulateWithDynamicArray(n), LastRemaining(n));

    private static int LastRemaining(int n)
    {
        var head = 1;
        var step = 1;
        var leftToRight = true;
        var remaining = n;

        while (remaining > 1)
        {
            if (leftToRight || remaining % 2 == 1)
            {
                head += step;
            }

            remaining /= 2;
            step *= 2;
            leftToRight = !leftToRight;
        }

        return head;
    }

    // Every pass - regardless of direction - discards the numbers at even (0-indexed)
    // positions and keeps the ones at odd positions. A right-to-left pass is the same
    // "keep odd positions" rule applied to the reversed list, then reversed back -
    // verified directly against LeetCode's own worked example: [1..9] -> [2,4,6,8]
    // (left-to-right) -> [2,6] (right-to-left) -> [6].
    private static int SimulateWithDynamicArray(int n)
    {
        var current = new DynamicArray<int>();

        for (var i = 1; i <= n; i++)
        {
            current.Add(i);
        }

        var leftToRight = true;

        while (current.Count > 1)
        {
            if (!leftToRight)
            {
                current = Reverse(current);
            }

            current = KeepOddPositions(current);

            if (!leftToRight)
            {
                current = Reverse(current);
            }

            leftToRight = !leftToRight;
        }

        return current.Get(0);
    }

    private static DynamicArray<int> KeepOddPositions(DynamicArray<int> values)
    {
        var kept = new DynamicArray<int>();

        for (var i = 1; i < values.Count; i += 2)
        {
            kept.Add(values.Get(i));
        }

        return kept;
    }

    private static DynamicArray<int> Reverse(DynamicArray<int> values)
    {
        var reversed = new DynamicArray<int>();

        for (var i = values.Count - 1; i >= 0; i--)
        {
            reversed.Add(values.Get(i));
        }

        return reversed;
    }
}
