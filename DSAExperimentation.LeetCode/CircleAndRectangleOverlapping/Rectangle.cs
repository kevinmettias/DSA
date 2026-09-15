namespace DSAExperimentation.LeetCode.CircleAndRectangleOverlapping;

// The rectangle arrives as x1, y1, x2, y2, and beside the circle's own arguments
// nothing but position says where one shape ends and the next begins. A rectangle is
// one concept, so it is one argument that says so.
internal readonly record struct Rectangle(int X1, int Y1, int X2, int Y2);
