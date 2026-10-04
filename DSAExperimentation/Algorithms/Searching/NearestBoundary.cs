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
//
// The pending positions live in a BCL Stack<int>: it is the sweep's own scratch state,
// the bucket StronglyConnectedComponents' accumulation stack sits in, not a substitute
// for a call stack, which is what DataStructures/Stack is kept for.
internal static class NearestBoundary
{
    // The nearest position to the left whose value is strictly smaller than the value
    // at the position asked about; `none` when the value is a prefix minimum.
    public static int[] SmallerToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep<StrictlySmaller, ToTheLeft>(values, none);

    // The nearest position to the left whose value is smaller than or equal to the value
    // at the position asked about - so an equal neighbour is a boundary rather than
    // something the sweep pops on its way further left.
    public static int[] SmallerOrEqualToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep<SmallerOrEqual, ToTheLeft>(values, none);

    // The nearest position to the left whose value is strictly greater than the value at
    // the position asked about; `none` when the value is a prefix maximum.
    public static int[] GreaterToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep<StrictlyGreater, ToTheLeft>(values, none);

    // The nearest position to the left whose value is greater than or equal to the value
    // at the position asked about.
    public static int[] GreaterOrEqualToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep<GreaterOrEqual, ToTheLeft>(values, none);

    // The nearest position to the right whose value is strictly smaller than the value at
    // the position asked about; `none` when the value is a suffix minimum.
    public static int[] SmallerToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep<StrictlySmaller, ToTheRight>(values, none);

    // The nearest position to the right whose value is smaller than or equal to the value
    // at the position asked about - the mirror of SmallerOrEqualToTheLeft, and the side a
    // caller pairs with a strict SmallerToTheLeft to keep one of two equal minima.
    public static int[] SmallerOrEqualToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep<SmallerOrEqual, ToTheRight>(values, none);

    // The nearest position to the right whose value is strictly greater than the value at
    // the position asked about; `none` when the value is a suffix maximum.
    public static int[] GreaterToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep<StrictlyGreater, ToTheRight>(values, none);

    // The nearest position to the right whose value is greater than or equal to the value
    // at the position asked about.
    public static int[] GreaterOrEqualToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep<GreaterOrEqual, ToTheRight>(values, none);

    // `none` is the caller's to choose because the useful sentinel differs by caller: one
    // wants -1, so a range starting at a boundary reads as -1 + 1 == 0; another wants
    // values.Length, so a range ending at one reads as Length with no special case. A
    // scan that picked for them would push that choice back onto call sites as arithmetic.
    //
    // The relation and the direction are struct type arguments, so each of the eight public
    // forms is its own instantiation with the comparison and the position arithmetic
    // inlined: the pop loop runs no branch on which relation it is, where a switch over an
    // enum would have run one per comparison.
    private static int[] Sweep<TRelation, TDirection>(ReadOnlySpan<int> values, int none)
        where TRelation : struct, IBoundaryRelation
        where TDirection : struct, ISweepDirection
    {
        var boundaries = new int[values.Length];
        var pending = new Stack<int>();

        for (var step = 0; step < values.Length; step++)
        {
            var position = TDirection.PositionAt(step, values.Length);

            // Everything the relation rejects is popped: a rejected position can never be
            // a boundary for any later position either, which is what makes this O(n)
            // rather than a rescan.
            while (pending.TryPeek(out var top) && !TRelation.IsBoundaryFor(values[top], values[position]))
            {
                pending.TryPop(out _);
            }

            boundaries[position] = pending.TryPeek(out var boundary) ? boundary : none;
            pending.Push(position);
        }

        return boundaries;
    }

    // Whether the candidate value still stands in the named relation to the value being
    // asked about - so it is a boundary for that position rather than something the sweep
    // pops on its way further along. The relation arrives as a named witness rather than
    // as a lambda so the two values it is read against cannot be silently exchanged: for a
    // non-symmetric relation, `candidate < current` and `current < candidate` are
    // different algorithms, and the call site says which one it meant. The four below are
    // the whole set; whichever one a caller names is the whole of that caller's algorithm,
    // since the sweep itself is identical for all four.
    private interface IBoundaryRelation
    {
        static abstract bool IsBoundaryFor(int candidate, int current);
    }

    private readonly struct StrictlySmaller : IBoundaryRelation
    {
        public static bool IsBoundaryFor(int candidate, int current) => candidate < current;
    }

    private readonly struct SmallerOrEqual : IBoundaryRelation
    {
        public static bool IsBoundaryFor(int candidate, int current) => candidate <= current;
    }

    private readonly struct StrictlyGreater : IBoundaryRelation
    {
        public static bool IsBoundaryFor(int candidate, int current) => candidate > current;
    }

    private readonly struct GreaterOrEqual : IBoundaryRelation
    {
        public static bool IsBoundaryFor(int candidate, int current) => candidate >= current;
    }

    // Which side the boundary is looked for on, as the order positions are visited in: a
    // boundary to the left is found by sweeping left to right, so it has already been seen.
    private interface ISweepDirection
    {
        static abstract int PositionAt(int step, int length);
    }

    private readonly struct ToTheLeft : ISweepDirection
    {
        public static int PositionAt(int step, int length) => step;
    }

    private readonly struct ToTheRight : ISweepDirection
    {
        public static int PositionAt(int step, int length) => length - 1 - step;
    }
}
