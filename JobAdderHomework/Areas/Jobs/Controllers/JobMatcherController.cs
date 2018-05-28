using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using JobAdderHomework.Areas.Jobs.Models;
using JobAdderHomework.Services;

namespace JobAdderHomework.Areas.Jobs.Controllers
{
    public class JobMatcherController : Controller
    {
        private readonly double minimalThreshold = 0.66;
        private readonly double betterThreshold = 0.34;

        private const string BASE_ADDRESS = "http://private-76432-jobadder1.apiary-mock.com/";
        private readonly IJobsService _JobsService;
        private readonly ICandidatesService _CandidatesService;


        //public JobMatcherController(IJobsService jobsService, ICandidatesService candidatesService)
        //{
        //    _JobsService = jobsService ?? throw new ArgumentNullException(nameof(jobsService));
        //    _CandidatesService = candidatesService ?? throw new ArgumentNullException(nameof(candidatesService));
        //}

        public JobMatcherController()
        {
            _JobsService = new JobsService(BASE_ADDRESS);
            _CandidatesService = new CandidateService(BASE_ADDRESS);
        }

        public async Task<ActionResult> Index()
        {
            var model = new JobOverviewModel();
            model.Candidates = await _CandidatesService.GetCandidates();
            model.Jobs = await _JobsService.GetJobs();
            return View(model);
        }


        public async Task<ActionResult> FindMatch(int jobId)
        {
            var model = new JobMatcherModel();
            if (jobId <= 0)
            {
                ViewBag.Message = "Invalid job ID.";
                return View(model);
            }
            var job = (await _JobsService.GetJobs())[jobId];
            model.JobId = jobId;
            model.JobName = job.Name;
            model.CompanyName = job.Company;
            model.Skills = job.Skills;

            var candidates = await _CandidatesService.GetCandidates();
            job.WeightedSkills = InitWeightedSkills(job.Skills, true);         

            foreach (var candidate in candidates)
            {
                candidate.Value.WeightedSkills = InitWeightedSkills(candidate.Value.SkillTags);
            }

            var first = candidates.OrderByDescending(c => job.WeightedSkills.IntersectByName(c.Value.WeightedSkills).Sum(s => s.Value)).First();
            var selectedCandidate = new Candidate();

            selectedCandidate = first.Value;
            ViewBag.Message = "Match found.";

            model.Candidate = new JobMatcherModel.ClosestMatchingCandidate() { Id = selectedCandidate.CandidateId, Name = selectedCandidate.Name, Skills = selectedCandidate.SkillTags };
            return View(model);
        }


        private Dictionary<string, double> InitWeightedSkills(string rawSkills, bool forJD=false)
        {
            var result = new Dictionary<string, double>();
            var skills = rawSkills.Split(',').Select(s => s.Trim()).Distinct().ToArray();
            var count = skills.Count();
            for (int i = 0; i < count; i++)
            {
                var rawWeight = (double)(count - i) / count;
                var weightToAdd = rawWeight >= minimalThreshold ? 1 : (rawWeight >= betterThreshold ? minimalThreshold : betterThreshold);
                result.Add(skills[i], (forJD ? (count - i) * rawWeight : rawWeight));
            }
            return result;
        }
    }

    public static class Extensions
    {
        public static Dictionary<string, double> IntersectByName(this Dictionary<string, double> dict, Dictionary<string, double> dictToIntersect)
        {
            var keys = dict.Keys.Intersect(dictToIntersect.Keys);
            return keys.Select(key => new { Key = key, Value = dict[key] * dictToIntersect[key] }).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}