namespace DSAExperimentation.Algorithms.Searching;

// The boundary array a monotonic-stack sweep produces: for every position, the nearest
// position on one side whose value stands in a named relation to the value at the
// position being asked about, or the caller's `none` when no such position exists.
//
// Which relation a caller names is not a detail it is free to get wrong: a problem that
// needs a tie to belong to one of two equal positions rather than both asks for the
// strict relation on one side and the or-equal one on the other, and that asymmetry -
// not the scan - is what ApplyOperationsToMaximizeScore's (i - left[i]) * (right[i] - i)
// is built on. All four relations are here because that pairing is the reason the sweep
// is worth writing once; a caller reaching for a fifth is describing a different
// algorithm.
//
// O(n) either direction: each position is pushed once and popped at most once, so the
// sweep costs one pass where the outward scan it replaces costs O(n^2) whenever values
// run long ties. Values are read through the span and never held, so the caller keeps
// ownership of its array, and the returned array is the caller's to keep or discard.
internal static class NearestBoundary
{
    // The nearest position to the left whose value is strictly smaller than the value
    // at the position asked about; `none` when the value is a prefix minimum.
    public static int[] SmallerToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep(values, static (candidate, current) => candidate < current, none, BoundaryDirection.ToTheLeft);

    // The nearest position to the left whose value is smaller than or equal to the value
    // at the position asked about - so an equal neighbour is a boundary rather than
    // something the sweep pops on its way further left.
    public static int[] SmallerOrEqualToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep(values, static (candidate, current) => candidate <= current, none, BoundaryDirection.ToTheLeft);

    // The nearest position to the left whose value is strictly greater than the value at
    // the position asked about; `none` when the value is a prefix maximum.
    public static int[] GreaterToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep(values, static (candidate, current) => candidate > current, none, BoundaryDirection.ToTheLeft);

    // The nearest position to the left whose value is greater than or equal to the value
    // at the position asked about.
    public static int[] GreaterOrEqualToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep(values, static (candidate, current) => candidate >= current, none, BoundaryDirection.ToTheLeft);

    // The nearest position to the right whose value is strictly smaller than the value at
    // the position asked about; `none` when the value is a suffix minimum.
    public static int[] SmallerToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep(values, static (candidate, current) => candidate < current, none, BoundaryDirection.ToTheRight);

    // The nearest position to the right whose value is smaller than or equal to the value
    // at the position asked about - the mirror of SmallerOrEqualToTheLeft, and the side a
    // caller pairs with a strict SmallerToTheLeft to keep one of two equal minima.
    public static int[] SmallerOrEqualToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep(values, static (candidate, current) => candidate <= current, none, BoundaryDirection.ToTheRight);

    // The nearest position to the right whose value is strictly greater than the value at
    // the position asked about; `none` when the value is a suffix maximum.
    public static int[] GreaterToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep(values, static (candidate, current) => candidate > current, none, BoundaryDirection.ToTheRight);

    // The nearest position to the right whose value is greater than or equal to the value
    // at the position asked about.
    public static int[] GreaterOrEqualToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep(values, static (candidate, current) => candidate >= current, none, BoundaryDirection.ToTheRight);

    // `none` is the caller's to choose because the useful sentinel differs by caller: one
    // wants -1, so a range starting at a boundary reads as -1 + 1 == 0; another wants
    // values.Length, so a range ending at one reads as Length with no special case. A
    // scan that picked for them would push that choice back onto call sites as arithmetic.
    private static int[] Sweep(
        ReadOnlySpan<int> values, Func<int, int, bool> relation, int none, BoundaryDirection direction)
    {
        var boundaries = new int[values.Length];
        var pending = new Stack<int>();

        for (var step = 0; step < values.Length; step++)
        {
            var position = direction is BoundaryDirection.ToTheLeft ? step : values.Length - 1 - step;

            // Everything the relation rejects is popped: a rejected position can never be
            // a boundary for any later position either, which is what makes this O(n)
            // rather than a rescan.
            while (pending.TryPeek(out var top) && !relation(values[top], values[position]))
            {
                pending.TryPop(out _);
            }

            boundaries[position] = pending.TryPeek(out var boundary) ? boundary : none;
            pending.Push(position);
        }

        return boundaries;
    }

    private enum BoundaryDirection
    {
        ToTheLeft,
        ToTheRight,
    }
}
