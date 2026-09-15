using DSAExperimentation.DataStructures.HashMap;
using RepoScopeNode = DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode<DSAExperimentation.DataStructures.HashMap.HashMap<string, long>>;

namespace DSAExperimentation.LeetCode.ParseLispExpression;

// This repo's own HashMap<string,long>, chained one node per "let" via this
// repo's own SinglyLinkedListNode<T>.Value/Next doubling as an environment-chain
// link: O(1) extra per "let" rather than copying the whole enclosing scope.
// Variable lookup walks Next from the innermost scope outward until a
// HashMap.TryGetValue hits, which is exactly how "let" shadowing is supposed to
// resolve - an inner "let x ..." hides an outer x for the rest of its own body
// without mutating it.
internal sealed class ScopeChainEnvironment : IScopeEnvironment<RepoScopeNode>
{
    public static ScopeChainEnvironment Instance { get; } = new();

    private ScopeChainEnvironment()
    {
    }

    // An empty node rather than a null chain head: a lookup answers "not bound
    // anywhere" either way, and this way the evaluator never carries a second,
    // empty spelling of "no scope" through every one of its signatures.
    public RepoScopeNode Root() => new(new HashMap<string, long>());

    public RepoScopeNode Extend(RepoScopeNode parent) => new(new HashMap<string, long>()) { Next = parent };

    public void Bind(RepoScopeNode scope, string name, long value) => scope.Value.Set(name, value);

    public long ResolveName(string name, RepoScopeNode scope)
    {
        for (RepoScopeNode? node = scope; node is not null; node = node.Next)
        {
            if (node.Value.TryGetValue(name, out var value))
            {
                return value;
            }
        }

        throw new InvalidOperationException($"Unbound variable '{name}'.");
    }
}
