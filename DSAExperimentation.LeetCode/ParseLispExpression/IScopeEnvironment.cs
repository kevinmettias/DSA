namespace DSAExperimentation.LeetCode.ParseLispExpression;

// LeetCode 736's one axis of variation, named: the evaluator above tokenizes
// identically, switches on "add"/"mult" identically and binds a "let"'s pairs
// with the same loop either way, so what a scope IS and how a nested "let"
// extends it is the entire difference between the two arms. An environment has
// exactly four decision points, so each arm implements those four and shares
// the rest of the evaluator rather than restating it.
internal interface IScopeEnvironment<TScope>
{
    // The empty scope the whole expression starts in, before any "let" has
    // extended it. The chain arm answers with an empty node rather than a null
    // chain head, which is why no method here has to spell "no scope" twice.
    TScope Root();

    // The child scope a nested "let" body sees, wrapped around the scope the
    // "let" itself was written in. Nothing is copied in the chain arm, which is
    // the whole O(1)-extra-per-"let" argument.
    TScope Extend(TScope parent);

    // Records one binding in that child scope, and is called only after the
    // bound expression has been evaluated against it - so "let x 1 y (add x 1)"
    // sees x while computing y, and a name bound twice in one "let" rebinds
    // rather than mutating the scope the enclosing expression still reads.
    void Bind(TScope scope, string name, long value);

    // A token that is not a decimal literal: a name, which throws when no scope
    // in the environment holds it. This is the one step the two arms genuinely
    // disagree on - an indexer hit against an outward walk through Next.
    long ResolveName(string name, TScope scope);
}
