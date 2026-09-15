namespace DSAExperimentation.LeetCode.RectangleArea;

// LeetCode 223 hands the two rectangles over as eight adjacent ints, and at that call site
// nothing but position says where one rectangle ends and the next begins. A rectangle is
// one concept, so it is one argument that says so.
internal readonly record struct Rectangle(int X1, int Y1, int X2, int Y2)
{
    public int Width => X2 - X1;

    public int Height => Y2 - Y1;

    // The closed-form arm multiplies a rectangle's own sides before subtracting the
    // overlap, so the long cast belongs to the area rather than to each call site.
    public long Area => (long)Width * Height;
}
