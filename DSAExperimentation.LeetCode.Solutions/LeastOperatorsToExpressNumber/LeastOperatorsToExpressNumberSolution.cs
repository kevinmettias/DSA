using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.LeetCode.Solutions.LeastOperatorsToExpressNumber;

// LeetCode 964. Least Operators to Express Number: the fewest operators in an
// expression built from the base alone that evaluates to target.
//
// No parentheses are allowed, so the optimal expression is a flat signed sum of
// x^k monomials and nothing more structured is even expressible. One occurrence of
// x^k costs (k-1) multiplications for k >= 1, or one division for x^0 (written
// x/x), and joining c same-sign occurrences at one exponent costs c-1 additions or
// subtractions. Folding the per-occurrence build cost together with its own join
// gives a flat "cost per unit at exponent k" weight: 2 at k = 0, k itself for
// k >= 1. A digit recursion in the base then picks, at each digit, either the digit
// itself (round down, carry nothing) or the base minus the digit (round up, carry
// +1 into the next exponent). The cross-exponent joins are one fewer than the total
// monomial count, so the recursion's raw total is corrected by -1 at the end.
//
// Both arms run the same digit recurrence. The unmemoized baseline writes it out as
// plain recursion - rather than through Memoizer's IRecurrence contract, because a
// baseline uses no library implementation (ARCHITECTURE 17.5) - so every (remaining, level)
// pair reachable by two different digit paths is re-explored from scratch and the
// recursion tree stays fully binary. LeastOpsExpressTargetByMemoizedRecursion runs
// it as the named recurrence DigitExpressionCost and caches on exactly that pair via
// this repo's own Memoizer<TState,TResult> - the same 2-tuple state shape
// BurstBalloonsSolution already uses - collapsing the tree to the handful of
// distinct states digit DP actually needs.
[LeetCodeQuestion("least-operators-to-express-number", LeetCodeAnswerOrder.AsWritten)]
internal static class LeastOperatorsToExpressNumberSolution
{
    // The units-digit (level 0) weight is always 2, independent of baseValue:
    // reaching 1 costs one x/x division, so the weight is fixed rather than derived
    // from the recursion level like every other digit's.
    private const int UnitsDigitWeight = 2;

    // The whole expression's monomials are joined by one fewer operator than there
    // are monomials, so the accumulated weight overcounts by exactly one.
    private const int JoinCorrection = 1;

    public static int LeastOpsExpressTargetByUnmemoizedRecursion(int baseValue, int target) =>
        UnmemoizedCost(baseValue, (target, 0)) - JoinCorrection;

    public static int LeastOpsExpressTargetByMemoizedRecursion(int baseValue, int target)
    {
        var recurrence = new DigitExpressionCost(baseValue);

        var totalWeight = Memoizer.Memoize<(int Remaining, int Level), int>((target, 0), recurrence);

        return totalWeight - JoinCorrection;
    }

    private static int RecursiveCost(
        DigitStep step, int remaining, IRecurrence<(int Remaining, int Level), int> cost)
    {
        var down = DownCost(step, cost);

        // quotient+1 fails to strictly decrease remaining only at baseValue == 2,
        // remaining == 2 (floor(2/2)+1 == 2): "round up" would recurse into this
        // exact same state forever without ever being cheaper than "down", so skip
        // it instead of calling cost again.
        if (step.Quotient + 1 >= remaining)
        {
            return down;
        }

        var up = UpCost(step, cost);
        return Math.Min(down, up);
    }

    private static int DownCost(DigitStep step, IRecurrence<(int Remaining, int Level), int> cost)
        => (step.Digit * step.Weight) + cost.Replay((step.Quotient, step.Level + 1), cost);

    private static int UpCost(DigitStep step, IRecurrence<(int Remaining, int Level), int> cost)
        => ((step.BaseValue - step.Digit) * step.Weight) + cost.Replay((step.Quotient + 1, step.Level + 1), cost);

    // The digit rule as plain recursion: the same round-down/round-up choice as
    // DigitExpressionCost, each next digit answered by calling straight back in.
    private static int UnmemoizedCost(int baseValue, (int Remaining, int Level) state)
    {
        var (remaining, level) = state;
        var weight = level == 0 ? UnitsDigitWeight : level;

        if (remaining < baseValue)
        {
            return BaseCaseCost(baseValue, remaining, level, weight);
        }

        var step = new DigitStep(baseValue, level, weight, remaining % baseValue, remaining / baseValue);

        return UnmemoizedRecursiveCost(step, remaining);
    }

    // The same self-loop guard as RecursiveCost: at baseValue == 2, remaining == 2,
    // "round up" would map the state onto itself, so only "down" is taken.
    private static int UnmemoizedRecursiveCost(DigitStep step, int remaining)
    {
        var down = UnmemoizedDownCost(step);

        if (step.Quotient + 1 >= remaining)
        {
            return down;
        }

        var up = UnmemoizedUpCost(step);
        return Math.Min(down, up);
    }

    private static int UnmemoizedDownCost(DigitStep step)
        => (step.Digit * step.Weight) + UnmemoizedCost(step.BaseValue, (step.Quotient, step.Level + 1));

    private static int UnmemoizedUpCost(DigitStep step)
        => ((step.BaseValue - step.Digit) * step.Weight)
            + UnmemoizedCost(step.BaseValue, (step.Quotient + 1, step.Level + 1));

    // remaining < baseValue is the base case, not just remaining == 0: once the
    // current digit already covers remaining outright, "round up" only ever needs
    // exactly one more level (quotient+1 == 1 there, itself already below baseValue).
    // The closed form here, instead of recursing, avoids descending into that next
    // level's OWN round-up option, which would map (1, level+1) back onto
    // (1, level+2) and loop forever without ever being the cheaper choice.
    private static int BaseCaseCost(int baseValue, int remaining, int level, int weight)
    {
        var roundDown = remaining * weight;
        var roundUp = (level + 1) + ((baseValue - remaining) * weight);
        return Math.Min(roundDown, roundUp);
    }

    // Bundles the pieces DownCost/UpCost need for one non-base-case digit step,
    // keeping each helper's parameter list to (step, cost) instead of five loose values.
    private readonly record struct DigitStep(int BaseValue, int Level, int Weight, int Digit, int Quotient);

    // The digit rule, named: a digit is read either as itself, rounding down and
    // carrying nothing, or as the base minus the digit, rounding up and carrying one
    // into the next exponent. Memoizing it is the caller's choice, not part of the rule.
    private sealed class DigitExpressionCost(int baseValue) : IRecurrence<(int Remaining, int Level), int>
    {
        /// <inheritdoc/>
        public int Replay((int Remaining, int Level) state, IRecurrence<(int Remaining, int Level), int> rest)
        {
            var (remaining, level) = state;
            var weight = level == 0 ? UnitsDigitWeight : level;

            if (remaining < baseValue)
            {
                return BaseCaseCost(baseValue, remaining, level, weight);
            }

            var step = new DigitStep(baseValue, level, weight, remaining % baseValue, remaining / baseValue);

            return RecursiveCost(step, remaining, rest);
        }
    }
}
