namespace DSAExperimentation.LeetCode.Harness;

// One problem's whole executable surface behind a type-erased face: which
// strategies exist, which cases they must satisfy, which workloads a benchmark
// should measure, and how to run any of those combinations.
//
// The erasure is deliberate and it is the ONLY erasure in this design. A single
// test harness and a single benchmark harness cannot be generic over 1100
// different TInput/TOutput pairs, so something has to hand them a uniform face -
// but that face is this abstract class, not the registration API. Everything a
// problem actually writes goes through LeetCodeProblem.For<TInput, TOutput>,
// where the compiler still checks that a case's input matches the strategy's
// parameter and that its expected value matches the strategy's return type. A
// mismatched case is a build error at the registration, not a cast that blows up
// at run time - which is exactly the property a naive object[] harness gives up.
//
// This lives in DSAExperimentation.LeetCode rather than beside the catalog in
// DSAExperimentation.Tests because it is the one thing BOTH harnesses must see,
// and Tests and Benchmarks reference this project without referencing each other.
// Question METADATA (topics, difficulty, acceptance) deliberately does not appear
// here: it is a fetched snapshot keyed by TitleSlug, and the test project joins
// it on by slug, so a cached-fixture refresh never touches a registration.
internal abstract class LeetCodeProblem
{
    public abstract string TitleSlug { get; }

    public abstract IReadOnlyList<string> StrategyNames { get; }

    public abstract IReadOnlyList<string> CaseNames { get; }

    // The (strategy, workload) pairs that are actually worth measuring - NOT the
    // cross product of the two. Every strategy must satisfy every case, because a
    // case is a correctness claim about the problem; a workload is not, and some
    // are valid for only some arms. Path With Maximum Probability is the example
    // that forced this: its exhaustive-DFS arm is exponential in the vertex count,
    // so the 400-vertex workload its Dijkstra arm exists to be measured on would
    // simply never finish on the other one.
    public abstract IReadOnlyList<LeetCodeArm> WorkloadArms { get; }

    public static LeetCodeProblemBuilder<TInput, TOutput> For<TInput, TOutput>(string titleSlug)
        => new(titleSlug);

    public abstract LeetCodeRunOutcome RunCase(string strategyName, string caseName);

    // Returns the measured region as a closure over an ALREADY-PREPARED input, so
    // a benchmark's [GlobalSetup] can resolve the arm once and the timed call is
    // one delegate invocation with no lookup, no parsing and no boxing of the
    // input inside it. That single indirection is constant across every arm of a
    // comparison, which is what keeps the relative numbers these benchmarks exist
    // to produce trustworthy even though it is not a direct static call.
    public abstract Func<object?> BindWorkload(string strategyName, string workloadName);
}
