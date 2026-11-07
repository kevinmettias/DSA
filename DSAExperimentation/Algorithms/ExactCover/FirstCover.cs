namespace DSAExperimentation.Algorithms.ExactCover;

// The visitor that keeps the first cover found and stops there, for a puzzle that wants one
// answer. The cover is copied out, since the span is the search's buffer. A fresh visitor is the
// default value, which has seen no cover.
/// <summary>Keeps the first cover a search finds, and stops it.</summary>
internal struct FirstCover : ICoverVisitor
{
    private int[]? _rows;

    /// <summary>Gets the first cover's rows; empty until one is found.</summary>
    public readonly int[] Rows => _rows ?? [];

    /// <summary>Keeps a copy of the cover and stops the search.</summary>
    /// <param name="cover">The cover's rows.</param>
    /// <returns>Always <see langword="true"/>: one cover is all that is wanted.</returns>
    public bool ShouldStopAfter(ReadOnlySpan<int> cover)
    {
        _rows = cover.ToArray();

        return true;
    }
}
