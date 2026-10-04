using System.Numerics;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Algorithms.Searching;

// Binary search on the answer: the boundary of a monotone rule over an integer interval, found in
// O(log(high - low)) calls to the rule. FirstTrue is the smallest candidate in [low, high] the rule
// holds for, and high + 1 when it holds for none; LastTrue is the largest, and low - 1 when it holds
// for none. "None" lands just outside the range for the same reason LowerBound answers Length: the
// caller gets a position, not a sentinel, and FirstTrue(0, n - 1, rule) reads exactly like LowerBound
// over n entries.
//
// Not a BinarySearch method, though the loop is the same ten lines: BinarySearch's laws are a sorted
// IRandomAccessSequence and a comparer, and this has neither - no sequence, only an interval stated
// by two numbers, and a rule whose one law is monotonicity (see IMonotonePredicate). LowerBound could
// be written as FirstTrue over a "not before target" rule, and would answer the same; it is not,
// because for a reference-type element that rule is shared generic code, and every probe would pay
// the runtime lookup §12.4 measured, where BoundSearch keeps the comparison inline.
//
// Generic over the integer type so a range that outgrows int - a time bound of 10^14, a budget summed
// over 10^5 stations - is searched in long rather than truncated to fit an index. Each value type
// closes the method separately, so int and long each run with their operators inlined.
//
// The bounds are checked on entry, at O(1): an answer of high + 1 or low - 1 has to be representable,
// and so does the width high - low + 1. A range that wraps would otherwise come back as a confident,
// wrong boundary; a range ending at int.MaxValue belongs in long.
internal static class MonotonePredicateSearch
{
    private const string EmptyRangeMessage = "low may exceed high by at most one, which states the empty range.";
    private const string NoRoomAboveMessage = "high must be below the type's MaxValue, so that high + 1 can report that no candidate holds.";
    private const string NoRoomBelowMessage = "low must be above the type's MinValue, so that low - 1 can report that no candidate holds.";
    private const string RangeTooWideMessage = "high - low + 1 must fit in the integer type the range is stated in.";

    public static Integer FirstTrue<Integer, TPredicate>(Integer low, Integer high, TPredicate predicate)
        where Integer : IBinaryInteger<Integer>, IMinMaxValue<Integer>
        where TPredicate : struct, IMonotonePredicate<Integer>
    {
        ValidateRange(low, high);
        return FirstWhere(low, high, predicate, Boundary.FirstHolding);
    }

    // The last candidate the rule holds for is one before the first it fails for, so this is the same
    // walk looking for the other answer.
    public static Integer LastTrue<Integer, TPredicate>(Integer low, Integer high, TPredicate predicate)
        where Integer : IBinaryInteger<Integer>, IMinMaxValue<Integer>
        where TPredicate : struct, IMonotonePredicate<Integer>
    {
        ValidateRange(low, high);

        if (low == Integer.MinValue)
        {
            throw new ArgumentOutOfRangeException(nameof(low), NoRoomBelowMessage);
        }

        return FirstWhere(low, high, predicate, Boundary.FirstFailing) - Integer.One;
    }

    private static void ValidateRange<Integer>(Integer low, Integer high)
        where Integer : IBinaryInteger<Integer>, IMinMaxValue<Integer>
    {
        if (high == Integer.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(high), NoRoomAboveMessage);
        }

        if (low > high + Integer.One)
        {
            throw new ArgumentOutOfRangeException(nameof(low), EmptyRangeMessage);
        }

        // high - low itself overflows only when low is negative; then the width fits exactly when
        // high stays below MaxValue + low, a sum that cannot overflow for a negative low.
        if (low < Integer.Zero && high >= Integer.MaxValue + low)
        {
            throw new ArgumentOutOfRangeException(nameof(high), RangeTooWideMessage);
        }
    }

    // The first candidate in [low, high] whose answer is the one sought, or high + 1. The window is
    // half-open, [first, pastLast), so it closes exactly when the two meet and no probe is wasted on
    // a candidate already ruled out.
    private static Integer FirstWhere<Integer, TPredicate>(
        Integer low, Integer high, TPredicate predicate, Boundary boundary)
        where Integer : IBinaryInteger<Integer>
        where TPredicate : struct, IMonotonePredicate<Integer>
    {
        var halving = Integer.CreateTruncating(AlgorithmConstants.HalvingFactor);
        var soughtAnswer = boundary is Boundary.FirstHolding;
        var first = low;
        var pastLast = high + Integer.One;

        while (first < pastLast)
        {
            var mid = first + ((pastLast - first) / halving);

            if (predicate.IsSatisfiedBy(mid) == soughtAnswer)
            {
                pastLast = mid;
            }
            else
            {
                first = mid + Integer.One;
            }
        }

        return first;
    }

    // Which answer the walk is looking for the first of. FirstTrue wants the first candidate the rule
    // holds for; LastTrue wants the first it fails for, one past its own answer.
    private enum Boundary
    {
        FirstHolding,
        FirstFailing,
    }
}
