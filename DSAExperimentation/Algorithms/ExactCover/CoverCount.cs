namespace DSAExperimentation.Algorithms.ExactCover;

// The visitor that counts every cover and never stops the search, for a puzzle that asks how many
// answers there are. A count can pass an int's range (the 19-queens puzzle has 4,968,057,848
// solutions), so it is a long.
/// <summary>Counts every cover a search finds.</summary>
internal struct CoverCount : ICoverVisitor
{
    /// <summary>Gets how many covers have been seen.</summary>
    public long Count { get; private set; }

    /// <summary>Counts the cover and lets the search go on.</summary>
    /// <param name="cover">The cover's rows, which are not read.</param>
    /// <returns>Always <see langword="false"/>: every cover is counted.</returns>
    public bool ShouldStopAfter(ReadOnlySpan<int> cover)
    {
        Count++;

        return false;
    }
}
