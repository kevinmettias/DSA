namespace DSAExperimentation.LeetCode.FindMaximumAreaOfATriangle;

// One axis of the point set compressed to its extent: the smallest and largest
// coordinate seen on that axis, so a row's x-span or a column's y-span. Every use
// was an unnamed (int, int) pair of the same type, where nothing but position said
// which end was the minimum.
internal readonly record struct AxisSpan(int Min, int Max);
