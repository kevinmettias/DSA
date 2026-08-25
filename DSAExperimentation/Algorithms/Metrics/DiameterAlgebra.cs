using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Algorithms.Metrics;

// The binary-tree diameter algorithm generalized to arbitrary arity: a path through
// a node can only descend into at most two distinct children, so "diameter through
// this node" sums the two tallest child heights rather than a fixed left/right pair.
internal readonly struct DiameterAlgebra<TNode> : IFoldAlgebra<TNode, HeightDiameterState>
{
    public static HeightDiameterState Empty
        => new(Height: 0, Diameter: 0);

    public static HeightDiameterState Combine(TNode node, IReadOnlyList<HeightDiameterState> children)
    {
        var scan = ScanChildren(children);
        var height = 1 + scan.LargestHeight;
        var diameterThroughNode = scan.LargestHeight + scan.SecondLargestHeight;

        return new(height, Math.Max(diameterThroughNode, scan.LargestChildDiameter));
    }

    // Single linear scan for the top two child heights (needed for the path that passes
    // through this node) and the largest child diameter (needed for the path that
    // doesn't) - avoids sorting all of children just to read off the top two.
    private static ChildScanResult ScanChildren(IReadOnlyList<HeightDiameterState> children)
    {
        var largestHeight = 0;
        var secondLargestHeight = 0;
        var largestChildDiameter = 0;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];

            if (child.Height > largestHeight)
            {
                secondLargestHeight = largestHeight;
                largestHeight = child.Height;
            }
            else if (child.Height > secondLargestHeight)
            {
                secondLargestHeight = child.Height;
            }

            if (child.Diameter > largestChildDiameter)
            {
                largestChildDiameter = child.Diameter;
            }
        }
        return new ChildScanResult(largestHeight, secondLargestHeight, largestChildDiameter);
    }

    private readonly record struct ChildScanResult(int LargestHeight, int SecondLargestHeight, int LargestChildDiameter);
}
