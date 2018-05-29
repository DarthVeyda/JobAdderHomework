# JobAdderHomework


## How to use

The project is an ASP.Net MVC website. The Jobs page has lists of all available jobs and candidates for easier reference. Clicking on the job description opens a page with details for the best matching candidate.

## Requirements

* .Net Framework 4.6.2
* Visual Studio to compile and run the project and the tests;
* Internet connection to fetch the test data and download Nuget packages

## Assumptions I made

1. The search is supposed return one and exactly one candidate ("the closest available match", even if objectively it isn't very close).
2. In the list of required skills for the job the first several skills are significantly more important than the rest; candidate's skills are distributed more evenly. I used quadratic and linear function, respectively, when assigning weights to the job/candidate's skills.
3. Candidate with two "Important" skills is a better match than a candidate who only has one "Very Important" skill. 
4. The "aphra"/"ahpra" mismatch in the testing data is a typo.
5. The test data from the sandbox doesn't change between requests (so it makes sense to cache it after retrieving it for the first time).

## The matching algorithm

1. For each candidate, get the skills that are in the job requirements (if any).
2. For every matching skill, multiply the job weight and the candidate weight (i.e. to balance "Critical requirement for the job" with "Candidate mentions it near the end of the list").
3. Add up all the products.
4. Candidate with the maximum sum of the products is the closest match.

