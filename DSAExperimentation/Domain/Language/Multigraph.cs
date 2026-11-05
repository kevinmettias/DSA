namespace DSAExperimentation.Domain.Language;

// Letters that together spell one sound, as sh spells the sound that begins "ship": two letters are
// a digraph, three a trigraph. Many languages that write in letters have them, English's ch and sh,
// German's sch, Spanish's ll, so the type is the languages' general one and each language's set is
// its own (IMultigraphSet). The letters are lowercase, and a set compares them with case set aside.
/// <summary>Letters that together spell one sound: a digraph, a trigraph.</summary>
/// <param name="Letters">The letters, lowercase, in the order they are written.</param>
public readonly record struct Multigraph(string Letters)
{
    /// <summary>How many letters spell the sound: 2 for a digraph, 3 for a trigraph.</summary>
    public int Length => Letters.Length;
}
