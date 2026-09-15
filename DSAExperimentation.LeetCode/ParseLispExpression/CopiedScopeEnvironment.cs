namespace DSAExperimentation.LeetCode.ParseLispExpression;

// The naive baseline arm's environment: a plain Dictionary copied whole on every
// nested "let" (an easy-to-reach-for interpreter design), paying O(scope size)
// extra work per "let" on top of evaluating it. Written without this repo's own
// primitives, as a baseline is meant to be.
internal sealed class CopiedScopeEnvironment : IScopeEnvironment<Dictionary<string, long>>
{
    public static CopiedScopeEnvironment Instance { get; } = new();

    private CopiedScopeEnvironment()
    {
    }

    public Dictionary<string, long> Root() => new();

    public Dictionary<string, long> Extend(Dictionary<string, long> parent) => new(parent);

    public void Bind(Dictionary<string, long> scope, string name, long value) => scope[name] = value;

    // A copied scope holds every name the enclosing "let" handed down, so one
    // indexer hit answers a lookup - and a name nothing bound fails out of the
    // dictionary rather than being walked for.
    public long ResolveName(string name, Dictionary<string, long> scope) => scope[name];
}
