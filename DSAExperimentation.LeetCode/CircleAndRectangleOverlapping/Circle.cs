namespace DSAExperimentation.LeetCode.CircleAndRectangleOverlapping;

// LC 1401 hands the circle over as radius, xCenter and yCenter - three adjacent
// ints at a call site where only position says which one is the radius. The circle
// is one concept, so it is one argument that says so.
internal readonly record struct Circle(int Radius, int X, int Y);
