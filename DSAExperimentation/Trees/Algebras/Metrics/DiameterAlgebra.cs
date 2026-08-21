namespace DSAExperimentation.Trees;

public readonly struct DiameterAlgebra<TNode>
    : IBinaryDepthFirstFoldAlgebra<TNode, HeightDiameterState>
{
    public static HeightDiameterState Empty
        => new(Height: 0, Diameter: 0);

    public static HeightDiameterState Combine(
        TNode node,
        BinaryChildren<HeightDiameterState> children)
    {
        var height = 1 + Math.Max(
            children.Left.Height,
            children.Right.Height);

        var diameterThroughNode =
            children.Left.Height +
            children.Right.Height;

        var largestChildDiameter = Math.Max(
            children.Left.Diameter,
            children.Right.Diameter);

        var diameter = Math.Max(
            diameterThroughNode,
            largestChildDiameter);

        return new(height, diameter);
    }
}




