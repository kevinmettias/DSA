namespace DSAExperimentation.Tests.DataStructures.LinkedList.Fixtures;

// Plain classes holding an int and one or two references, the size a hand-written list node of
// each shape takes, which a configuration of the one linked node must not exceed.
internal static class LinkShapes
{
    public static long BytesOfShapeWith(int links) => Allocations.BytesOf(() => Make(links));

    private static object Make(int links) => links == 1 ? new OneLinkShape() : new TwoLinkShape();

    private sealed class OneLinkShape
    {
        public int Value { get; set; }

        public object? First { get; set; }
    }

    private sealed class TwoLinkShape
    {
        public int Value { get; set; }

        public object? First { get; set; }

        public object? Second { get; set; }
    }
}
