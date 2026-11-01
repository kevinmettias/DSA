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
// O(n) either direction: each position is placed once and passed over at most once, so
// the sweep costs one pass where the outward scan it replaces costs O(n^2) whenever
// values run long ties. Values are read through the span and never held, so the caller keeps
// ownership of its array, and the returned array is the caller's to keep or discard.
//
// No stack is allocated. The stack a monotonic sweep keeps is, after it has placed any
// position p, exactly the chain p, boundary(p), boundary(boundary(p)), ... - the top
// below p is the boundary p was given, and so on down - so the sweep follows those links
// in the boundary array it is filling instead. The comparisons are the stack's, in the
// same order, and the result array is the only allocation.
/// <summary>Nearest smaller or greater values to each position's left or right, in one O(n) sweep.</summary>
public static class NearestBoundary
{
    // What the sweep's chain ends at when the caller's `none` is itself a position.
    private const int NotAPosition = -1;

    // The nearest position to the left whose value is strictly smaller than the value
    // at the position asked about; `none` when the value is a prefix minimum.
    /// <summary>Finds each position's nearest strictly smaller value to its left.</summary>
    /// <param name="values">The values to sweep.</param>
    /// <param name="none">
    /// The answer for a position with no such value, usually -1 or the length of <paramref name="values"/>.
    /// </param>
    /// <returns>
    /// An array parallel to <paramref name="values"/> holding, at each position, the nearest position to its
    /// left with a strictly smaller value, or <paramref name="none"/> where there is none.
    /// </returns>
    public static int[] SmallerToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep<StrictlySmaller, ToTheLeft>(values, none);

    // The nearest position to the left whose value is smaller than or equal to the value
    // at the position asked about - so an equal neighbour is a boundary rather than
    // something the sweep pops on its way further left.
    /// <summary>Finds each position's nearest smaller-or-equal value to its left.</summary>
    /// <param name="values">The values to sweep.</param>
    /// <param name="none">
    /// The answer for a position with no such value, usually -1 or the length of <paramref name="values"/>.
    /// </param>
    /// <returns>
    /// An array parallel to <paramref name="values"/> holding, at each position, the nearest position to its
    /// left with a smaller or equal value, or <paramref name="none"/> where there is none.
    /// </returns>
    public static int[] SmallerOrEqualToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep<SmallerOrEqual, ToTheLeft>(values, none);

    // The nearest position to the left whose value is strictly greater than the value at
    // the position asked about; `none` when the value is a prefix maximum.
    /// <summary>Finds each position's nearest strictly greater value to its left.</summary>
    /// <param name="values">The values to sweep.</param>
    /// <param name="none">
    /// The answer for a position with no such value, usually -1 or the length of <paramref name="values"/>.
    /// </param>
    /// <returns>
    /// An array parallel to <paramref name="values"/> holding, at each position, the nearest position to its
    /// left with a strictly greater value, or <paramref name="none"/> where there is none.
    /// </returns>
    public static int[] GreaterToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep<StrictlyGreater, ToTheLeft>(values, none);

    // The nearest position to the left whose value is greater than or equal to the value
    // at the position asked about.
    /// <summary>Finds each position's nearest greater-or-equal value to its left.</summary>
    /// <param name="values">The values to sweep.</param>
    /// <param name="none">
    /// The answer for a position with no such value, usually -1 or the length of <paramref name="values"/>.
    /// </param>
    /// <returns>
    /// An array parallel to <paramref name="values"/> holding, at each position, the nearest position to its
    /// left with a greater or equal value, or <paramref name="none"/> where there is none.
    /// </returns>
    public static int[] GreaterOrEqualToTheLeft(ReadOnlySpan<int> values, int none) =>
        Sweep<GreaterOrEqual, ToTheLeft>(values, none);

    // The nearest position to the right whose value is strictly smaller than the value at
    // the position asked about; `none` when the value is a suffix minimum.
    /// <summary>Finds each position's nearest strictly smaller value to its right.</summary>
    /// <param name="values">The values to sweep.</param>
    /// <param name="none">
    /// The answer for a position with no such value, usually -1 or the length of <paramref name="values"/>.
    /// </param>
    /// <returns>
    /// An array parallel to <paramref name="values"/> holding, at each position, the nearest position to its
    /// right with a strictly smaller value, or <paramref name="none"/> where there is none.
    /// </returns>
    public static int[] SmallerToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep<StrictlySmaller, ToTheRight>(values, none);

    // The nearest position to the right whose value is smaller than or equal to the value
    // at the position asked about - the mirror of SmallerOrEqualToTheLeft, and the side a
    // caller pairs with a strict SmallerToTheLeft to keep one of two equal minima.
    /// <summary>Finds each position's nearest smaller-or-equal value to its right.</summary>
    /// <param name="values">The values to sweep.</param>
    /// <param name="none">
    /// The answer for a position with no such value, usually -1 or the length of <paramref name="values"/>.
    /// </param>
    /// <returns>
    /// An array parallel to <paramref name="values"/> holding, at each position, the nearest position to its
    /// right with a smaller or equal value, or <paramref name="none"/> where there is none.
    /// </returns>
    public static int[] SmallerOrEqualToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep<SmallerOrEqual, ToTheRight>(values, none);

    // The nearest position to the right whose value is strictly greater than the value at
    // the position asked about; `none` when the value is a suffix maximum.
    /// <summary>Finds each position's nearest strictly greater value to its right.</summary>
    /// <param name="values">The values to sweep.</param>
    /// <param name="none">
    /// The answer for a position with no such value, usually -1 or the length of <paramref name="values"/>.
    /// </param>
    /// <returns>
    /// An array parallel to <paramref name="values"/> holding, at each position, the nearest position to its
    /// right with a strictly greater value, or <paramref name="none"/> where there is none.
    /// </returns>
    public static int[] GreaterToTheRight(ReadOnlySpan<int> values, int none) =>
        Sweep<StrictlyGreater, ToTheRight>(values, none);

    // The nearest position to the right whose value is greater than or equal to the value
    // at the position asked about.
    /// <summary>Finds each position's nearest greater-or-equal value to its right.</summary>
    /// <param name="values">The values to sweep.</param>
    /// <param name="none">
    /// The answer for a position with no such value, usually -1 or the length of <paramref name="values"/>.
    /// </param>
    /// <returns>
    /// An array parallel to <paramref name="values"/> holding, at each position, the nearest position to its
    /// right with a greater or equal value, or <paramref name="none"/> where there is none.
    /// </returns>
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

        // The chain ends at the "no boundary" mark, so the mark must not be a position. The
        // callers' -1 and values.Length never are, and the sweep writes them directly; a
        // `none` that names a real position is swept with -1 and swapped in afterwards.
        var endOfChain = IsPosition(none, values.Length) ? NotAPosition : none;

        for (var step = 0; step < values.Length; step++)
        {
            var position = TDirection.PositionAt(step, values.Length);
            var previous = step == 0 ? endOfChain : TDirection.PositionAt(step - 1, values.Length);

            boundaries[position] = NearestOnChain<TRelation>(values, boundaries, previous, position);
        }

        if (endOfChain != none)
        {
            ReplaceEndOfChain(boundaries, none);
        }

        return boundaries;
    }

    private static void ReplaceEndOfChain(int[] boundaries, int none)
    {
        for (var position = 0; position < boundaries.Length; position++)
        {
            if (boundaries[position] == NotAPosition)
            {
                boundaries[position] = none;
            }
        }
    }

    // Walks the chain from `candidate` - the position placed just before, then its boundary, then
    // that one's - and answers the first the relation accepts for `position`, or the chain's end,
    // which is never a position. Skipping a rejected candidate by jumping to its own boundary is the
    // stack pop in the chain's terms: a rejected position can never be a boundary for any later
    // position either, so no link is followed twice and the whole sweep stays O(n) amortized.
    private static int NearestOnChain<TRelation>(
        ReadOnlySpan<int> values, int[] boundaries, int candidate, int position)
        where TRelation : struct, IBoundaryRelation
    {
        while (IsPosition(candidate, values.Length) && !TRelation.IsBoundaryFor(values[candidate], values[position]))
        {
            candidate = boundaries[candidate];
        }

        return candidate;
    }

    private static bool IsPosition(int candidate, int length) => (uint)candidate < (uint)length;

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
