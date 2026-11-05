namespace DSAExperimentation.LeetCode;

// Names the LeetCode question a solution class answers, which enrolls the class in the catalog harness
// (ARCHITECTURE 17.13): a benchmark class is generated for it at build time that times every strategy
// over every case the LeetCode catalog holds for the question, and its answers are judged against the
// catalog's. What it carries are the problem's own facts - which question, whether its answer's order
// is free, the type stating a promise a generated case can break, and whether its strategies may
// rewrite their inputs - never a workload or a size, which are measurement decisions and stay in the
// benchmarks (17.7). It is data: nothing in this tier reads the catalog.
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class LeetCodeQuestionAttribute(string titleSlug, LeetCodeAnswerOrder answerOrder) : Attribute
{
    // The question's slug on leetcode.com, which keys it in the catalog.
    public string TitleSlug { get; } = titleSlug;

    public LeetCodeAnswerOrder AnswerOrder { get; } = answerOrder;

    // A static class whose IsKeptBy takes a strategy's inputs and says whether a case keeps a promise
    // the statement makes in prose, as TwoSumPromise does; null when the statement makes none that a
    // generated case can break.
    public Type? Promise { get; set; }

    // Whether the strategies may rewrite the inputs they are handed, as relinking a list or compacting
    // an array in place does. A harness then hands every call inputs of its own, which the generated
    // benchmark does by copying them before each iteration.
    public bool RewritesInput { get; set; }

    // Whether the strategies answer by rewriting their first input, as an in-place question asks (Move
    // Zeroes moves the zeros within nums): each returns nothing, and its answer is that input as it leaves
    // it. A harness hands every call inputs of its own, as for RewritesInput.
    public bool AnswersInPlace { get; set; }

    // How far a real-valued answer may stand from LeetCode's and still be accepted, as the statement says
    // ("Answers within 10^-5 of the actual answer will be accepted"); 0 when it is compared as written.
    public double Tolerance { get; set; }

    // A static class whose IsAccepted takes a strategy's inputs, its answer and the answer the catalog records,
    // and says whether the statement accepts that answer where it lets several stand ("return any of
    // them"), as LongestPalindromicSubstringAnswerCheck does; null when the recorded answer is the one to
    // give, by the order rule and the tolerance above.
    public Type? AnswerCheck { get; set; }
}
