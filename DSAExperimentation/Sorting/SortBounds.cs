namespace DSAExperimentation.Sorting;

// Groups the [Low, High] range MergeSort's recursion narrows - the same "group related
// parameters into a type" recipe ARCHITECTURE.md §6 already applies elsewhere in this repo.
// Immutable and passed by value, unlike Searching.SearchRange: MergeSort's recursion splits into
// two independent, non-shared sub-ranges per call rather than narrowing one shared range in
// place, so there is nothing here for a mutable, ref-passed cursor to buy.
internal readonly record struct SortBounds(int Low, int High);
