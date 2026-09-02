using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameVI;

// LeetCode 1686. Stone Game VI: the classic greedy proof - a stone's total "swing"
// value (aliceValue[i] + bobValue[i]) is what an optimal player fights over, so
// sorting stones by that swing descending and alternating picks (Alice first) is
// optimal. Same TwoCitySchedulingTests shape: this repo's own
// MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence with a custom
// descending comparer, not the textbook game-theory DP this problem doesn't
// actually need.
public sealed class StoneGameVITests
{
    [Fact]
    public void Winner_LeetCodeExampleOne_AliceWins()
    {
        int[] aliceValues = [1, 3];
        int[] bobValues = [2, 1];

        var winner = Winner(aliceValues, bobValues);
        Assert.Equal("Alice", winner);
    }

    [Fact]
    public void Winner_LeetCodeExampleTwo_TieWhenSwingsAreEqual()
    {
        int[] aliceValues = [1, 2];
        int[] bobValues = [3, 1];

        var winner = Winner(aliceValues, bobValues);
        Assert.Equal("Tie", winner);
    }

    [Fact]
    public void Winner_LeetCodeExampleThree_BobWins()
    {
        int[] aliceValues = [2, 4, 3];
        int[] bobValues = [1, 6, 7];

        var winner = Winner(aliceValues, bobValues);
        Assert.Equal("Bob", winner);
    }

    private static string Winner(int[] aliceValues, int[] bobValues)
    {
        var stones = BuildStonesBySwing(aliceValues, bobValues);
        SortBySwingDescending(stones);
        var (aliceScore, bobScore) = TallyAlternatingScores(stones);

        return DetermineWinner(aliceScore, bobScore);
    }

    private static (int Alice, int Bob)[] BuildStonesBySwing(int[] aliceValues, int[] bobValues)
        => aliceValues.Select((a, i) => (Alice: a, Bob: bobValues[i])).ToArray();

    private static void SortBySwingDescending((int Alice, int Bob)[] stones)
    {
        var bySwingDescending = Comparer<(int Alice, int Bob)>.Create(
            (x, y) => (y.Alice + y.Bob).CompareTo(x.Alice + x.Bob));

        MergeSort.Sort<(int Alice, int Bob), ArrayIndexedSequence<(int Alice, int Bob)>>(
            new ArrayIndexedSequence<(int Alice, int Bob)>(stones), bySwingDescending);
    }

    private static (int AliceScore, int BobScore) TallyAlternatingScores((int Alice, int Bob)[] stones)
    {
        var aliceScore = 0;
        var bobScore = 0;

        for (var i = 0; i < stones.Length; i++)
        {
            if (i % 2 == 0)
            {
                aliceScore += stones[i].Alice;
            }
            else
            {
                bobScore += stones[i].Bob;
            }
        }

        return (aliceScore, bobScore);
    }

    private static string DetermineWinner(int aliceScore, int bobScore)
        => aliceScore == bobScore ? "Tie" : aliceScore > bobScore ? "Alice" : "Bob";
}
