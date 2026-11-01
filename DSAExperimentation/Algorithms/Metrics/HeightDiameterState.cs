namespace DSAExperimentation.Algorithms.Metrics;

/// <summary>A subtree's height and diameter, the state <see cref="DiameterAlgebra{TNode}"/> folds.</summary>
/// <param name="Height">The number of nodes on the longest downward path from the subtree's root.</param>
/// <param name="Diameter">The number of edges on the longest path between two of the subtree's nodes.</param>
public readonly record struct HeightDiameterState(
    int Height,
    int Diameter);
