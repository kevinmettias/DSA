using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using Stone = (int Alice, int Bob);

namespace DSAExperimentation.LeetCode.StoneGameVI;

// LeetCode 1686. Stone Game VI: Alice and Bob alternately take stones - Alice
// first - where stone i is worth aliceValues[i] to Alice and bobValues[i] to Bob.
// Both play optimally; the answer is 1 if Alice finishes ahead, -1 if Bob does,
// and 0 on a tie.
//
// The classic greedy proof is what both strategies run: what a player actually
// fights over is a stone's total "swing", aliceValues[i] + bobValues[i] - taking
// it earns you your own value AND denies the opponent theirs - so sorting stones
// by swing descending and alternating picks is optimal, and the game-theory DP the
// title suggests is never needed. The whole score difference then collapses to
// "every alice value, minus the swings that land on Bob's turns", which is why the
// order within a group of equal swings cannot change the answer.
//
// The two strategies differ only in the sort primitive: the BCL's Array.Sort, or
// this repo's own MergeSort over an ArrayIndexedSequence - the same "same
// algorithm, different sort primitive" pairing TwoCityScheduling uses.
internal static class StoneGameVISolution
{
    // Picks alternate from the front of the sorted stones: even index is Alice's
    // turn, odd index is Bob's.
    private const int TurnParityDivisor = 2;

    private static readonly IComparer<Stone> BySwingDescending =
        Comparer<Stone>.Create((x, y) => Swing(y).CompareTo(Swing(x)));

    // The textbook arm: pair the values up and hand them to the BCL's own sort.
    // Deliberately written without this repo's primitives - it is what the composed
    // arm below has to justify itself against.
    public static int WinnerByArraySortGreedy(int[] aliceValues, int[] bobValues)
    {
        var stones = PairBySwing(aliceValues, bobValues);

        Array.Sort(stones, BySwingDescending);

        return OutcomeOfAlternatingPicks(stones);
    }

    // The composed arm: the identical greedy over this repo's own
    // MergeSort.Sort<Element, TSequence>, viewing the pair array through an
    // ArrayIndexedSequence so the sort never sees an array at all.
    public static int WinnerByMergeSortGreedy(int[] aliceValues, int[] bobValues)
    {
        var stones = PairBySwing(aliceValues, bobValues);

        MergeSort.Sort<Stone, ArrayIndexedSequence<Stone>>(
            new ArrayIndexedSequence<Stone>(stones), BySwingDescending);

        return OutcomeOfAlternatingPicks(stones);
    }

    // What both players are really bidding for on stone i.
    private static int Swing(Stone stone) => stone.Alice + stone.Bob;

    private static Stone[] PairBySwing(int[] aliceValues, int[] bobValues)
    {
        var stones = new Stone[aliceValues.Length];

        for (var i = 0; i < stones.Length; i++)
        {
            stones[i] = (aliceValues[i], bobValues[i]);
        }

        return stones;
    }

    // Walk the sorted stones, crediting each to whoever's turn it is, and report
    // the outcome in LeetCode's 1 / 0 / -1 convention.
    private static int OutcomeOfAlternatingPicks(Stone[] stones)
    {
        var aliceScore = 0;
        var bobScore = 0;

        for (var i = 0; i < stones.Length; i++)
        {
            if (i % TurnParityDivisor == 0)
            {
                aliceScore += stones[i].Alice;
            }
            else
            {
                bobScore += stones[i].Bob;
            }
        }

        return Math.Sign(aliceScore - bobScore);
    }
}
