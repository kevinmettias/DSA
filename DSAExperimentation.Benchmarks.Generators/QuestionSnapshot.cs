namespace DSAExperimentation.Benchmarks.Generators;

// What the generator reads from one question's snapshot file, keyed by the slug its name carries: the types
// a function question declares for its inputs (LeetCodeShape), or a design question's constructor and
// methods (DesignShape), null for a function question.
internal sealed class QuestionSnapshot(string slug, IReadOnlyList<string> declared, DesignShape? design)
{
    public string Slug { get; } = slug;

    public IReadOnlyList<string> Declared { get; } = declared;

    public DesignShape? Design { get; } = design;

    public static QuestionSnapshot Of(string path, string json) =>
        new(Path.GetFileNameWithoutExtension(path), LeetCodeShape.DeclaredTypesIn(json), DesignShape.In(json));
}
