using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Storage;

/// <summary>A question's signature corrected by hand: the signature its judge actually has, and why.</summary>
/// <param name="Signature">The signature the snapshot records for the question in place of LeetCode's metaData.</param>
/// <param name="Reason">Why LeetCode's metaData is replaced, and what shows the judge's true shape.</param>
internal sealed record SignatureCorrection(LeetCodeSignature Signature, string Reason);
