namespace DSAExperimentation.Tests.LeetCodeCatalog;

// One of LeetCode's own publicly documented example inputs (the "Run"-button
// sample cases, from the question's own exampleTestcases field) - NOT the private
// judge test suite LeetCode actually grades submissions against, which no public
// API exposes. RawExpectedOutput is best-effort, scraped from the question's own
// prose description where an Output line could be matched to its Input line one
// for one - null when that scrape didn't find a corresponding value.
internal sealed record LeetCodeTestCase(string RawInput, string? RawExpectedOutput);
