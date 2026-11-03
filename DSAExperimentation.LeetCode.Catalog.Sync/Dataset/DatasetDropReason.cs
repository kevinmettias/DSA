namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// Why the importer left out one of LeetCodeDataset's cases. Each is a reason the case would test
// something other than the question as LeetCode poses it, so the import report counts them
// rather than leaving the reader to guess what was lost.
internal enum DatasetDropReason
{
    // The dataset's reference solution raised an error or timed out, so its "output" is a message.
    ReferenceFailed,

    // The input is not a list of "name = value" assignments.
    InputNotSplit,

    // The assignments name different inputs from the question's signature.
    NamesDiffer,

    // An input or the output is outside the Python literal grammar PythonLiteral reads.
    NotTranslatable,

    // The reference solution returned None for a question whose answer cannot be empty: it fell off
    // the end, which means the input broke one of the question's promises.
    NoAnswer,

    // An input is not of the type its parameter declares, such as a 1.5 in an integer tree.
    NotOfDeclaredType,

    // An input lies outside one of the question's stated bounds.
    OutOfBounds,

    // The inputs repeat an example's or an earlier case's.
    Duplicate,
}
