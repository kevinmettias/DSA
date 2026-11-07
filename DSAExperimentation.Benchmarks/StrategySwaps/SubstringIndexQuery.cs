namespace DSAExperimentation.Benchmarks.StrategySwaps;

// The operations SubstringIndexBenchmarks times, one per run: the three every substring index
// answers, and building the index, which is where they differ most.
/// <summary>An operation the three substring indexes all answer.</summary>
public enum SubstringIndexQuery
{
    /// <summary>Whether each pattern occurs.</summary>
    HasSubstring,

    /// <summary>How many times each pattern occurs.</summary>
    CountOccurrences,

    /// <summary>The longest substring that occurs twice.</summary>
    FindLongestRepeat,

    /// <summary>Building the index from the text.</summary>
    Build,
}
