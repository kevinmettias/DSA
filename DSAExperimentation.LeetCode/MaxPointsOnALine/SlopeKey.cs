namespace DSAExperimentation.LeetCode.MaxPointsOnALine;

// The canonical slope of the segment between two points, reduced by their GCD and
// sign-normalized so (dx, dy) and (-dx, -dy) - the same line, opposite direction -
// always compare equal. Both members are ints, so nothing but the names can say
// which of the two a value holds.
internal readonly record struct SlopeKey(int Dx, int Dy);
