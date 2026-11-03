namespace DSAExperimentation.LeetCode.Catalog;

// None of these is LeetCode's judge suite, which no public API exposes. They differ in who
// produced the expected output, which is what a failing case's reader needs to know first.
/// <summary>Where a case came from, and so who vouches for its expected output.</summary>
public enum LeetCodeCaseSource
{
    /// <summary>One of the question's published examples; the output is the one its description states.</summary>
    Example,

    /// <summary>
    /// A case from LeetCodeDataset (newfacade, Apache-2.0): a generated input, with the output its reference
    /// Python solution computed.
    /// </summary>
    Dataset,

    /// <summary>An input run through LeetCode's own Run button, with the output LeetCode's reference solution gave.</summary>
    LeetCodeRun,
}
