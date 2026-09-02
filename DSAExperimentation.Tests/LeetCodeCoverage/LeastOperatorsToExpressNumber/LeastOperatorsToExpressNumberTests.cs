using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LeastOperatorsToExpressNumber;

// LeetCode 964. Least Operators to Express Number: the optimal expression is a
// flat signed sum of x^k monomials (no parentheses are allowed, so nothing more
// structured than that is even expressible) - building one occurrence of x^k
// costs (k-1) multiplications for k >= 1, or 1 division for x^0 (written x/x),
// and joining c same-sign occurrences at one exponent costs c-1 additions/
// subtractions. Folding the per-occurrence build cost and its own join together
// gives a flat "cost per unit at exponent k" weight: 2 at k=0, k itself for
// k >= 1. Standard base-x digit DP then picks, at each digit, either the digit
// itself (round down, carry nothing) or x-minus-the-digit (round up, carry +1
// into the next exponent) - this repo's own Memoizer<TState,TResult> supplies
// the cache, keyed by (remaining value, exponent level) the same 2-tuple-state
// shape BurstBalloonsTests/EditDistanceBenchmarks already use. The cross-exponent
// joins are one fewer than the total monomial count, so the recursion's raw total
// is corrected by -1 at the very end.
public sealed class LeastOperatorsToExpressNumberTests
{
    [Fact]
    public void LeastOpsExpressTarget_OfficialExampleOne_ReturnsFive()
    {
        var actual = LeastOpsExpressTarget(x: 3, target: 19);

        Assert.Equal(5, actual);
    }

    [Fact]
    public void LeastOpsExpressTarget_OfficialExampleTwo_ReturnsEight()
    {
        var actual = LeastOpsExpressTarget(x: 5, target: 501);

        Assert.Equal(8, actual);
    }

    [Fact]
    public void LeastOpsExpressTarget_TargetEqualsX_ReturnsZeroOperators()
    {
        var actual = LeastOpsExpressTarget(x: 5, target: 5);

        Assert.Equal(0, actual);
    }

    // x = 2 is the one value where quotient+1 can land back on the same
    // remaining (remaining == x == 2), the edge case the quotient+1 >= remaining
    // guard above exists for - "2 + 2/2" = 3 in 2 operators (+, /).
    [Fact]
    public void LeastOpsExpressTarget_BaseTwoRoundUpFixedPointEdgeCase_ReturnsTwo()
    {
        var actual = LeastOpsExpressTarget(x: 2, target: 3);

        Assert.Equal(2, actual);
    }

    private static int LeastOpsExpressTarget(int x, int target)
    {
        var totalWeight = Memoizer.Memoize<(int Remaining, int Level), int>(
            (target, 0), (state, cost) => Cost(x, state, cost));

        return totalWeight - 1;
    }

    private static int Cost(int x, (int Remaining, int Level) state, Func<(int Remaining, int Level), int> cost)
    {
        var (remaining, level) = state;
        var weight = level == 0 ? 2 : level;

        // remaining < x is the base case, not just remaining == 0: once the
        // current digit already covers remaining outright, "round up" only
        // ever needs exactly one more level (quotient+1 == 1 there, itself
        // already below x) - closed-form here instead of recursing avoids
        // recursing into that next level's OWN round-up option, which would
        // map (1, level+1) back onto (1, level+2) and loop forever without
        // ever being the cheaper choice.
        if (remaining < x)
        {
            return BaseCaseCost(x, remaining, level, weight);
        }

        var step = new DigitStep(x, level, weight, remaining % x, remaining / x);
        return RecursiveCost(step, remaining, cost);
    }

    private static int BaseCaseCost(int x, int remaining, int level, int weight)
    {
        var roundDown = remaining * weight;
        var roundUp = (level + 1) + ((x - remaining) * weight);
        return Math.Min(roundDown, roundUp);
    }

    private static int RecursiveCost(DigitStep step, int remaining, Func<(int Remaining, int Level), int> cost)
    {
        var down = DownCost(step, cost);

        // quotient+1 fails to strictly decrease remaining only at x == 2,
        // remaining == 2 (floor(2/2)+1 == 2): "round up" would recurse into
        // this exact same state forever without ever being cheaper than
        // "down", so skip it instead of calling cost again.
        if (step.Quotient + 1 >= remaining)
        {
            return down;
        }

        var up = UpCost(step, cost);
        return Math.Min(down, up);
    }

    // Bundles the pieces DownCost/UpCost need for one non-base-case digit step,
    // keeping each helper's parameter list to (step, cost) instead of five loose values.
    private readonly record struct DigitStep(int X, int Level, int Weight, int Digit, int Quotient);

    private static int DownCost(DigitStep step, Func<(int Remaining, int Level), int> cost)
        => (step.Digit * step.Weight) + cost((step.Quotient, step.Level + 1));

    private static int UpCost(DigitStep step, Func<(int Remaining, int Level), int> cost)
        => ((step.X - step.Digit) * step.Weight) + cost((step.Quotient + 1, step.Level + 1));
}
