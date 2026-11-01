namespace DSAExperimentation.Algorithms.Traversal.TopDown;

/// <summary>
/// Whether a visited node has no children (<c>Leaf</c>) or some (<c>Interior</c>), even ones a graph walk
/// skips as seen.
/// </summary>
public enum NodePosition
{
    Interior,
    Leaf,
}
