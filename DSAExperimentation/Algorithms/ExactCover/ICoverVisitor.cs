namespace DSAExperimentation.Algorithms.ExactCover;

// What a search does with each exact cover it finds, and whether it goes on: keep the first and
// stop, count them all, or build a puzzle's answer from each. The visitor is a struct passed by
// reference, so the search compiles its code in and what it records stays in the caller's copy.
/// <summary>Receives each exact cover a search finds, and says whether to stop.</summary>
public interface ICoverVisitor
{
    /// <summary>Receives one cover.</summary>
    /// <param name="cover">
    /// The cover's rows: any rows chosen in advance first, then those the search chose. The span is
    /// the search's own buffer, valid only during this call.
    /// </param>
    /// <returns><see langword="true"/> to stop the search; otherwise false.</returns>
    bool ShouldStopAfter(ReadOnlySpan<int> cover);
}
