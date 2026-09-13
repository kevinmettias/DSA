using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.LeastOperatorsToExpressNumber;

// LeetCode 964. Least Operators to Express Number: the fewest operators in an
// expression of x's alone that evaluates to target.
//
// No parentheses are allowed, so the optimal expression is a flat signed sum of
// x^k monomials and nothing more structured is even expressible. One occurrence of
// x^k costs (k-1) multiplications for k >= 1, or one division for x^0 (written
// x/x), and joining c same-sign occurrences at one exponent costs c-1 additions or
// subtractions. Folding the per-occurrence build cost together with its own join
// gives a flat "cost per unit at exponent k" weight: 2 at k = 0, k itself for
// k >= 1. A base-x digit recursion then picks, at each digit, either the digit
// itself (round down, carry nothing) or x-minus-the-digit (round up, carry +1 into
// the next exponent). The cross-exponent joins are one fewer than the total
// monomial count, so the recursion's raw total is corrected by -1 at the end.
//
// LeastOpsExpressTargetByUnmemoizedRecursion is the naive baseline: the same
// recursion with no cache, so every (remaining, level) pair reachable by two
// different digit paths is re-explored from scratch and the recursion tree stays
// fully binary. Written with nothing but BCL recursion.
//
// LeastOpsExpressTargetByMemoizedRecursion caches on exactly that pair via this
// repo's own Memoizer<TState,TResult> - the same 2-tuple state shape
// BurstBalloonsSolution already uses - collapsing the tree to the handful of
// distinct states digit DP actually needs.
internal static class LeastOperatorsToExpressNumberSolution
{
    // The units-digit (level 0) weight is always 2, independent of x: reaching 1
    // costs one x/x division, so the weight is fixed rather than derived from the
    // recursion level like every other digit's.
    private const int UnitsDigitWeight = 2;

    // The whole expression's monomials are joined by one fewer operator than there
    // are monomials, so the accumulated weight overcounts by exactly one.
    private const int JoinCorrection = 1;

    public static int LeastOpsExpressTargetByUnmemoizedRecursion(int x, int target) =>
        UnmemoizedCost(x, target, level: 0) - JoinCorrection;

    private static int UnmemoizedCost(int x, int remaining, int level)
    {
        var weight = level == 0 ? UnitsDigitWeight : level;

        if (remaining < x)
        {
            return BaseCaseCost(x, remaining, level, weight);
        }

        var step = new DigitStep(x, level, weight, remaining % x, remaining / x);
        return RecursiveCost(step, remaining, state => UnmemoizedCost(x, state.Remaining, state.Level));
    }

    public static int LeastOpsExpressTargetByMemoizedRecursion(int x, int target)
    {
        var totalWeight = Memoizer.Memoize<(int Remaining, int Level), int>(
            (target, 0), (state, cost) => MemoizedCost(x, state, cost));

        return totalWeight - JoinCorrection;
    }

    private static int MemoizedCost(int x, (int Remaining, int Level) state, Func<(int Remaining, int Level), int> cost)
    {
        var (remaining, level) = state;
        var weight = level == 0 ? UnitsDigitWeight : level;

        if (remaining < x)
        {
            return BaseCaseCost(x, remaining, level, weight);
        }

        var step = new DigitStep(x, level, weight, remaining % x, remaining / x);
        return RecursiveCost(step, remaining, cost);
    }

    // remaining < x is the base case, not just remaining == 0: once the current
    // digit already covers remaining outright, "round up" only ever needs exactly
    // one more level (quotient+1 == 1 there, itself already below x). The closed
    // form here, instead of recursing, avoids descending into that next level's OWN
    // round-up option, which would map (1, level+1) back onto (1, level+2) and loop
    // forever without ever being the cheaper choice.
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
        // remaining == 2 (floor(2/2)+1 == 2): "round up" would recurse into this
        // exact same state forever without ever being cheaper than "down", so skip
        // it instead of calling cost again.
        if (step.Quotient + 1 >= remaining)
        {
            return down;
        }

        return Math.Min(down, UpCost(step, cost));
    }

    // Bundles the pieces DownCost/UpCost need for one non-base-case digit step,
    // keeping each helper's parameter list to (step, cost) instead of five loose values.
    private readonly record struct DigitStep(int X, int Level, int Weight, int Digit, int Quotient);

    private static int DownCost(DigitStep step, Func<(int Remaining, int Level), int> cost)
        => (step.Digit * step.Weight) + cost((step.Quotient, step.Level + 1));

    private static int UpCost(DigitStep step, Func<(int Remaining, int Level), int> cost)
        => ((step.X - step.Digit) * step.Weight) + cost((step.Quotient + 1, step.Level + 1));
}
