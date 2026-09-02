namespace DSAExperimentation.LeetCode.Candy;

// LeetCode 135. Candy: give each child in a line at least one candy so that any
// child with a higher rating than an immediate neighbor gets strictly more candy
// than that neighbor, minimizing the total handed out.
//
// The two strategies differ in how they reach a stable assignment: repeatedly
// relaxing any violated neighbor constraint until a full pass makes no change, or
// a single forward pass enforcing "rises" followed by a single backward pass
// enforcing "falls." No repo primitive applies - a pure O(n) scan over the
// ratings array, the same shape already established for GasStation/JumpGame.
internal static class CandySolution
{
    // How many positions back from the end the backward pass starts (the last
    // index has no right neighbor to compare against).
    private const int SecondToLastIndexOffset = 2;

    // Textbook baseline: keep re-scanning left-to-right, bumping any candy count
    // that still violates a neighbor's rating constraint, until a full pass makes
    // no change. On a strictly monotonic ratings array each pass can only
    // propagate a violation one position further, forcing O(n) passes of O(n)
    // each - the arm the two-pass approach below has to justify itself against.
    public static int MinCandiesByRepeatedRelaxation(int[] ratings)
    {
        var candies = new int[ratings.Length];
        Array.Fill(candies, 1);

        bool changed;

        do
        {
            changed = false;

            for (var i = 0; i < ratings.Length; i++)
            {
                changed |= RelaxNeighbors(ratings, candies, i);
            }
        } while (changed);

        return SumCandies(candies);
    }

    private static bool RelaxNeighbors(int[] ratings, int[] candies, int i)
    {
        var changed = false;

        if (i > 0 && ratings[i] > ratings[i - 1] && candies[i] <= candies[i - 1])
        {
            candies[i] = candies[i - 1] + 1;
            changed = true;
        }

        if (i < ratings.Length - 1 && ratings[i] > ratings[i + 1] && candies[i] <= candies[i + 1])
        {
            candies[i] = candies[i + 1] + 1;
            changed = true;
        }

        return changed;
    }

    // O(n) two-pass slope-constraint approach: a forward pass enforcing "rises
    // must get more candy than their left neighbor," then a backward pass
    // enforcing the same for falls.
    public static int MinCandiesByTwoPassSlopeConstraints(int[] ratings)
    {
        var candies = new int[ratings.Length];
        Array.Fill(candies, 1);

        for (var i = 1; i < ratings.Length; i++)
        {
            if (ratings[i] > ratings[i - 1])
            {
                candies[i] = candies[i - 1] + 1;
            }
        }

        for (var i = ratings.Length - SecondToLastIndexOffset; i >= 0; i--)
        {
            if (ratings[i] > ratings[i + 1])
            {
                candies[i] = Math.Max(candies[i], candies[i + 1] + 1);
            }
        }

        return SumCandies(candies);
    }

    private static int SumCandies(int[] candies)
    {
        var total = 0;

        for (var i = 0; i < candies.Length; i++)
        {
            total += candies[i];
        }

        return total;
    }
}
