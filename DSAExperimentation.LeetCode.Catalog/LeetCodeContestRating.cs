namespace DSAExperimentation.LeetCode.Catalog;

// LeetCode itself publishes only Easy/Medium/Hard. The numeric rating comes from zerotrac's
// leetcode_problem_rating project (MIT), which fits an Elo-style model to contest results: a
// question rated 1600 is one a 1600-rated contestant solves about half the time. Only questions
// that appeared in a weekly or biweekly contest have one.
/// <summary>A question's difficulty rating as measured from contest results, for a question that appeared in a contest.</summary>
/// <param name="Rating">The Elo-style rating: the contest rating at which half of contestants solve it.</param>
/// <param name="ContestSlug">The contest the question appeared in, such as "weekly-contest-475".</param>
/// <param name="ProblemIndex">The question's position in that contest, such as "Q4".</param>
public sealed record LeetCodeContestRating(double Rating, string ContestSlug, string ProblemIndex);
