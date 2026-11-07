using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.Solutions.FindPositiveIntegerSolutionForAGivenEquation;

// LeetCode 1237. Find Positive Integer Solution for a Given Equation: the hidden
// CustomFunction is a black box that is strictly increasing in both x and y, and the
// answer is every pair (x, y) in 1..1000 with f(x, y) == z.
//
// The function is an ICustomFunction rather than a delegate for the same reason LeetCode
// hides it behind an API object: a strategy is only allowed to know the monotonicity,
// never the formula, and the interface is where that contract gets written down - which a
// bare Func<int, int, int> parameter had nowhere to put. Every strategy has three
// overloads: one taking LeetCode's own inputs, function_id and z, which builds the
// function the id names as LeetCode's judge does; one taking the function, pinned to
// LeetCode's own 1..1000 range; and one taking an explicit bound, which is what lets a
// benchmark vary the search space without the range becoming part of the measured work.
[LeetCodeQuestion(
    "find-positive-integer-solution-for-a-given-equation",
    LeetCodeAnswerOrder.AnyOrder,
    Promise = typeof(FindPositiveIntegerSolutionForAGivenEquationPromise))]
internal static class FindPositiveIntegerSolutionForAGivenEquationSolution
{
    // LeetCode's shape (ARCHITECTURE 17.4): its judge takes function_id and z, and hands
    // findSolution the hidden CustomFunction that id names, which the statement states for
    // ids 1 and 2 only (StatedCustomFunctions); the pairs come back in any order.
    public static List<(int X, int Y)> FindSolutionsByBruteForce(int functionId, int targetValue)
    {
        var function = StatedCustomFunctions.Of(functionId);

        return FindSolutionsByBruteForce(function, targetValue);
    }

    // The textbook answer: evaluate the function at all bound^2 pairs and keep the
    // ones that hit targetValue. It assumes nothing about f beyond being callable, so
    // it is the arm the two monotonicity-exploiting strategies have to justify
    // themselves against. Deliberately BCL-only.
    public static List<(int X, int Y)> FindSolutionsByBruteForce(
        ICustomFunction function, int targetValue) =>
        FindSolutionsByBruteForce(function, targetValue, SearchRange.Bound);

    public static List<(int X, int Y)> FindSolutionsByBruteForce(
        ICustomFunction function, int targetValue, int bound)
    {
        var solutions = new List<(int X, int Y)>();

        for (var x = 1; x <= bound; x++)
        {
            for (var y = 1; y <= bound; y++)
            {
                if (function.Evaluate(x, y) == targetValue)
                {
                    solutions.Add((x, y));
                }
            }
        }

        return solutions;
    }

    // LeetCode's shape, as for the brute force above.
    public static List<(int X, int Y)> FindSolutionsByTwoPointer(int functionId, int targetValue)
    {
        var function = StatedCustomFunctions.Of(functionId);

        return FindSolutionsByTwoPointer(function, targetValue);
    }

    // The classic O(bound) walk: start at the bottom-left corner (smallest x, largest
    // y). Because f increases along both axes, a value below targetValue can only be
    // fixed by raising x and a value above targetValue only by lowering y, so each step
    // retires a whole row or column and neither pointer ever backtracks.
    public static List<(int X, int Y)> FindSolutionsByTwoPointer(
        ICustomFunction function, int targetValue) =>
        FindSolutionsByTwoPointer(function, targetValue, SearchRange.Bound);

    public static List<(int X, int Y)> FindSolutionsByTwoPointer(
        ICustomFunction function, int targetValue, int bound)
    {
        var solutions = new List<(int X, int Y)>();
        var x = 1;
        var y = bound;

        while (x <= bound && y >= 1)
        {
            (x, y) = ProbeAndStep(function, targetValue, solutions, (x, y));
        }

        return solutions;
    }

    // One step of the walk: probe (x, y) and move the pointer that the probe justifies - a
    // hit is recorded and retires the cell along the diagonal, a value below targetValue
    // can only be fixed by raising x, one above targetValue only by lowering y.
    private static (int X, int Y) ProbeAndStep(
        ICustomFunction function, int targetValue, List<(int X, int Y)> solutions,
        (int X, int Y) pointer)
    {
        var (x, y) = pointer;
        var value = function.Evaluate(x, y);

        if (value == targetValue)
        {
            solutions.Add((x, y));
            x++;
            y--;
        }
        else if (value < targetValue)
        {
            x++;
        }
        else
        {
            y--;
        }

        return (x, y);
    }

    // LeetCode's shape, as for the brute force above.
    public static List<(int X, int Y)> FindSolutionsByBinarySearchPerRow(int functionId, int targetValue)
    {
        var function = StatedCustomFunctions.Of(functionId);

        return FindSolutionsByBinarySearchPerRow(function, targetValue);
    }

    // For a fixed x the row f(x, 1..bound) is itself sorted ascending, so each row is
    // one BinarySearch.Find over this repo's own IRandomAccessSequence - the same
    // "binary search over a computed sequence" composition KokoEatingBananas and
    // CapacityToShipPackagesWithinDDays use, applied per row (Find for an exact
    // targetValue rather than LowerBound for a feasibility boundary).
    //
    // O(bound log bound) loses to the two-pointer walk here; the point of keeping it
    // is that the reusable primitive is available and correct on this shape, not that
    // it is the best tool for a function monotone on both axes.
    public static List<(int X, int Y)> FindSolutionsByBinarySearchPerRow(
        ICustomFunction function, int targetValue) =>
        FindSolutionsByBinarySearchPerRow(function, targetValue, SearchRange.Bound);

    public static List<(int X, int Y)> FindSolutionsByBinarySearchPerRow(
        ICustomFunction function, int targetValue, int bound)
    {
        var solutions = new List<(int X, int Y)>();

        for (var x = 1; x <= bound; x++)
        {
            var row = new FunctionRowSequence(function, x, bound);
            var index = BinarySearch.Find<int, FunctionRowSequence>(row, targetValue);

            if (index is not null)
            {
                solutions.Add((x, index.Value + 1));
            }
        }

        return solutions;
    }

    // One row of the function table, never materialized: index i stands for y = i + 1,
    // and xValue is the row this instance was built for.
    // A witness for this problem alone - "row of the hidden CustomFunction" is LC
    // 1237's own content, not a general sequence shape - so it lives beside the
    // solution, the same placement FirstBadVersion's VersionIsBad rule gets.
    private readonly struct FunctionRowSequence(ICustomFunction function, int xValue, int length)
        : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => function.Evaluate(xValue, index + 1);
    }
}
