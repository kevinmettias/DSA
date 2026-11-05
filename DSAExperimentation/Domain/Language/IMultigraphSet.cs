using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language;

// A language's multigraphs, as a letter set is its vowels: which letter groups spell one sound is
// the language's fact, so the sets are presets, and MultigraphSet reads one into a table once.
/// <summary>A named set of a language's multigraphs.</summary>
public interface IMultigraphSet
{
    /// <summary>The multigraphs, each once.</summary>
    static abstract ImmutableArray<Multigraph> Multigraphs { get; }
}
