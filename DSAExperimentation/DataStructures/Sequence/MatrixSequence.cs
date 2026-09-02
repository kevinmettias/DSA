namespace DSAExperimentation.DataStructures.Sequence;

// Adapts a row-major rectangular matrix to IRandomAccessSequence's flat, O(1)-
// indexed view - Get(i) is matrix[i / columns][i % columns] - so anything that
// treats a matrix as one sorted run (BinarySearch, say) never needs to know it
// is two-dimensional. Parallels ArraySequence, one dimension up.
internal readonly struct MatrixSequence<Element>(Element[][] matrix) : IRandomAccessSequence<Element>
{
    public int Length => matrix.Length * matrix[0].Length;

    public Element Get(int index) => matrix[index / matrix[0].Length][index % matrix[0].Length];
}
