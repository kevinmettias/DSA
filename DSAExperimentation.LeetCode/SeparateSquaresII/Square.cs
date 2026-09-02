namespace DSAExperimentation.LeetCode.SeparateSquaresII;

// One axis-aligned square: bottom-left corner (X, Y), side length L, so it spans
// x in [X, X+L] and y in [Y, Y+L]. A plain record, not IEnumerable, so the
// LeetCode-shaped int[][] overload and the prepared IReadOnlyList<Square> overload
// can never be ambiguous (§17.4).
internal readonly record struct Square(long X, long Y, long L);
