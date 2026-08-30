using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameIII;

// LeetCode 1406. Stone Game III: the player to move from stoneValue[index:]
// picks a prefix of 1, 2, or 3 stones, scoring that prefix's sum minus
// whatever the opponent can force from the state that follows - the same
// minimax-recurrence shape StoneGameTests/StoneGameIITests already use, just
// keyed on the single index (with a 3-way inner loop instead of a bound-M
// window) and memoized by this repo's own Memoizer<TState,TResult>.
public sealed partial class StoneGameIIITests
{
    [Fact]
    public void StoneGameIII_LeetCodeExampleOne_ReturnsBob()
        => Assert.Equal("Bob", Winner([1, 2, 3, 7]));

    [Fact]
    public void StoneGameIII_LeetCodeExampleTwo_ReturnsAlice()
        => Assert.Equal("Alice", Winner([1, 2, 3, -9]));

    [Fact]
    public void StoneGameIII_LeetCodeExampleThree_ReturnsTie()
        => Assert.Equal("Tie", Winner([1, 2, 3, 6]));

    [Fact]
    public void StoneGameIII_SinglePile_AliceTakesItAndWins()
        => Assert.Equal("Alice", Winner([5]));

    private static string Winner(int[] stoneValue)
    {
        var n = stoneValue.Length;
        var diff = Memoizer.Memoize<int, int>(0, Best);

        return diff switch
        {
            > 0 => "Alice",
            < 0 => "Bob",
            _ => "Tie",
        };

        int Best(int index, Func<int, int> bestFrom)
        {
            if (index >= n)
            {
                return 0;
            }

            var result = int.MinValue;
            var takenSum = 0;
            for (var take = 1; take <= 3 && index + take <= n; take++)
            {
                takenSum += stoneValue[index + take - 1];
                result = Math.Max(result, takenSum - bestFrom(index + take));
            }

            return result;
        }
    }
}
